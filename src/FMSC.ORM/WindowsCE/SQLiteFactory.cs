using System;
using System.Data.Common;

#if WindowsCE
namespace System.Data.SQLite
{
    /// <summary>
    /// SQLite implementation of <see cref="DbProviderFactory" />.
    /// </summary>
    internal sealed partial class SQLiteFactory : DbProviderFactory, IDisposable
    {
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public SQLiteFactory()
        {
            //
            // NOTE: Do nothing here now.  All the logging setup related code has
            //       been moved to the new SQLiteLog static class.
            //
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

#region IDisposable Members
        /// <summary>
        /// Cleans up resources (native and managed) associated with the current instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
#endregion

        ///////////////////////////////////////////////////////////////////////////////////////////////

#region IDisposable "Pattern" Members
        private bool disposed;
        private void CheckDisposed() /* throw */
        {
#if true
        if (disposed)
            throw new ObjectDisposedException(typeof(SQLiteFactory).Name);
#endif
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                //if (disposing)
                //{
                //    ////////////////////////////////////
                //    // dispose managed resources here...
                //    ////////////////////////////////////
                //}

                //////////////////////////////////////
                // release unmanaged resources here...
                //////////////////////////////////////

                disposed = true;
            }
        }
#endregion

        ///////////////////////////////////////////////////////////////////////////////////////////////

#region Destructor
        /// <summary>
        /// Cleans up resources associated with the current instance.
        /// </summary>
        ~SQLiteFactory()
        {
            Dispose(false);
        }
#endregion

        ///////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Static instance member which returns an instanced <see cref="SQLiteFactory" /> class.
        /// </summary>
        public static readonly SQLiteFactory Instance = new SQLiteFactory();

        /// <summary>
        /// Creates and returns a new <see cref="SQLiteCommand" /> object.
        /// </summary>
        /// <returns>The new object.</returns>
        public override DbCommand CreateCommand()
        {
            CheckDisposed();
            return new System.Data.SQLite.SQLiteCommand();
        }

        /// <summary>
        /// Creates and returns a new <see cref="SQLiteCommandBuilder" /> object.
        /// </summary>
        /// <returns>The new object.</returns>
        public override DbCommandBuilder CreateCommandBuilder()
        {
            CheckDisposed();
            return new System.Data.SQLite.SQLiteCommandBuilder();
        }

        /// <summary>
        /// Creates and returns a new <see cref="SQLiteConnection" /> object.
        /// </summary>
        /// <returns>The new object.</returns>
        public override DbConnection CreateConnection()
        {
            CheckDisposed();
            return new System.Data.SQLite.SQLiteConnection();
        }

        /// <summary>
        /// Creates and returns a new <see cref="SQLiteConnectionStringBuilder" /> object.
        /// </summary>
        /// <returns>The new object.</returns>
        //public override DbConnectionStringBuilder CreateConnectionStringBuilder()
        //{
        //    CheckDisposed();
        //    return new System.Data.SQLite.SQLiteConnectionStringBuilder();
        //}

        /// <summary>
        /// Creates and returns a new <see cref="SQLiteDataAdapter" /> object.
        /// </summary>
        /// <returns>The new object.</returns>
        public override DbDataAdapter CreateDataAdapter()
        {
            CheckDisposed();
            return new System.Data.SQLite.SQLiteDataAdapter();
        }

        /// <summary>
        /// Creates and returns a new <see cref="SQLiteParameter" /> object.
        /// </summary>
        /// <returns>The new object.</returns>
        public override DbParameter CreateParameter()
        {
            CheckDisposed();
            return new System.Data.SQLite.SQLiteParameter();
        }
    }
}
#endif