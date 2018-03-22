using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace FMSC.ORM.Core
{
    public class ConnectionManager : IConnectionManager
    {
        protected static Logger _logger = new Logger();

        public DbProviderFactory Provider { get; set; }
        protected IExceptionProcessor ExceptionProcessor { get; set; }

        private int _holdConnection;
        private object _connectionSyncLock = new object();
        public object ConnectionSyncLock { get { return _connectionSyncLock; } }
        public IDbConnection PersistentConnection { get; protected set; }

        protected bool _transactionCanceled = false;
        private object _transactionSyncLock = new object();
        public object TransactionSyncLock { get { return _transactionSyncLock; } }
        public IDbTransaction CurrentTransaction { get; protected set; }
        public int TransactionDepth { get; protected set; }

        #region Transaction methods

        public void BeginTransaction()
        {
            lock (TransactionSyncLock)
            {
                if (TransactionDepth == 0)
                {
                    Debug.Assert(CurrentTransaction == null);

                    var connection = OpenConnection();

                    try
                    {
                        var newTransaction = connection.BeginTransaction();
                        CurrentTransaction = newTransaction;
                    }
                    catch (Exception ex)
                    {
                        ReleaseConnection();
                        throw ExceptionProcessor.ProcessException(ex, connection, (string)null, CurrentTransaction);
                    }

                    _transactionCanceled = false;

                    this.EnterConnectionHold();
                    OnTransactionStarted();
                }
                TransactionDepth++;
            }
        }

        public void CommitTransaction()
        {
            lock (TransactionSyncLock)
            {
                var transactionDepth = TransactionDepth;

                OnTransactionEnding();

                if (transactionDepth > 0)
                {
                    transactionDepth--;
                    if (transactionDepth == 0)//transaction depth was 1
                    {
                        ReleaseTransaction();
                    }
                    TransactionDepth = transactionDepth;
                }
                else// transactionDepth <= 0
                {
                    Debug.Fail("Transaction depth is " + transactionDepth.ToString());
                }
            }
        }

        public void RollbackTransaction()
        {
            lock (TransactionSyncLock)
            {
                var transactionDepth = TransactionDepth;

                OnTransactionCanceling();
                _transactionCanceled = true;

                if (transactionDepth > 0)
                {
                    transactionDepth--;
                    if (transactionDepth == 0)
                    {
                        ReleaseTransaction();
                    }
                    TransactionDepth = transactionDepth;
                }
                else
                {
                    Debug.Fail("Transaction depth is " + transactionDepth.ToString());
                }
            }
        }

        private void ReleaseTransaction()
        {
            OnTransactionReleasing();

            try
            {
                if (CurrentTransaction != null)
                {
                    if (_transactionCanceled)
                    {
                        CurrentTransaction.Rollback();
                    }
                    else
                    {
                        CurrentTransaction.Commit();
                    }
                }
                else
                { Debug.Fail("_currentTransaction is null"); }
            }
            catch (Exception ex)
            {
                throw ExceptionProcessor.ProcessException(ex, PersistentConnection, (string)null, CurrentTransaction);
            }
            finally
            {
                if (CurrentTransaction != null)
                { CurrentTransaction.Dispose(); }
                CurrentTransaction = null;
                ExitConnectionHold();
                ReleaseConnection();
            }
        }

        #endregion Transaction methods

        #region Connection methods

        public void EnterConnectionHold()
        {
            System.Threading.Interlocked.Increment(ref _holdConnection);
        }

        public void ExitConnectionHold()
        {
            Debug.Assert(_holdConnection > 0);
            System.Threading.Interlocked.Decrement(ref _holdConnection);
        }

        public virtual IDbConnection OpenConnection()
        {
            lock (ConnectionSyncLock)
            {
                IDbConnection conn;
                if (_holdConnection == 0)
                {
                    conn = Provider.CreateConnection();
                }
                else
                {
                    Debug.Assert(PersistentConnection != null);
                    conn = PersistentConnection;
                }

                try
                {
                    if (conn.State == System.Data.ConnectionState.Broken)
                    {
                        conn.Close();
                    }

                    if (conn.State == System.Data.ConnectionState.Closed)
                    {
                        conn.Open();
                        PersistentConnection = conn;
                        OnConnectionOpened();
                    }

                    EnterConnectionHold();

                    return conn;
                }
                catch (Exception e)
                {
                    throw new ConnectionException("failed to open connection", e);
                }
            }
        }

        protected void ReleaseConnection()
        {
            ReleaseConnection(false);
        }

        protected virtual void ReleaseConnection(bool force)
        {
            lock (ConnectionSyncLock)
            {
                if (_holdConnection > 0)
                {
                    if (force)
                    {
                        _holdConnection = 0;
                    }
                    else
                    {
                        ExitConnectionHold();
                    }
                    if (_holdConnection == 0)
                    {
                        Debug.Assert(PersistentConnection != null);
                        PersistentConnection.Dispose();
                        PersistentConnection = null;
                    }
                }
            }
        }

        #endregion Connection methods

        #region events and logging

        //public event EventHandler ConnectionOpened;

        protected virtual void OnConnectionOpened()
        {
            _logger.WriteLine("Connection opened", Logger.DB_CONTROL);
        }

        protected virtual void OnTransactionStarted()
        {
            _logger.WriteLine("Transaction Started", Logger.DB_CONTROL);
        }

        protected virtual void OnTransactionEnding()
        {
            _logger.WriteLine("Transaction Ending", Logger.DB_CONTROL);
        }

        protected virtual void OnTransactionCanceling()
        {
            _logger.WriteLine("Transaction Canceling", Logger.DB_CONTROL);
        }

        protected virtual void OnTransactionReleasing()
        {
            _logger.WriteLine("Transaction Releasing", Logger.DB_CONTROL);
        }

        #endregion events and logging

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}