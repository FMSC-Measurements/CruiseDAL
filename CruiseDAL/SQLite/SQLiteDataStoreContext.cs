using System;
using System.Data.Common;
using System.Data.SQLite;

using CruiseDAL.Core;

namespace CruiseDAL.SQLite
{
    public class SQLiteDataStoreContext : DataStoreContext
    {
        public SQLiteDataStoreContext()
        { }

        protected override Exception ThrowExceptionHelper(DbConnection conn, DbCommand comm, Exception innerException)
        {
            Exception newEx;
            if (innerException is SQLiteException)
            {
                SQLiteException ex = innerException as SQLiteException;
                switch (ex.ResultCode)
                {
                    case SQLiteErrorCode.Corrupt:
                    case SQLiteErrorCode.NotADb:
                    case SQLiteErrorCode.Perm:
                    case SQLiteErrorCode.IoErr:
                    case SQLiteErrorCode.CantOpen:
                    case SQLiteErrorCode.Full:
                    case SQLiteErrorCode.Auth:
                        {
                            newEx = new FileAccessException(ex.ResultCode.ToString(), innerException);
                            break;
                        }
                    case SQLiteErrorCode.ReadOnly:
                        {
                            newEx = new ReadOnlyException(null, innerException);
                            break;
                        }
                    case SQLiteErrorCode.Constraint:
                        {
                            if (innerException.Message.IndexOf("UNIQUE constraint failed") >= 0)
                            {
                                newEx = new UniqueConstraintException(null, innerException);
                            }
                            else
                            {
                                newEx = new ConstraintException(null, innerException);
                            }
                            break;
                        }
                    default:
                        {
                            newEx = new SQLException(null, innerException);
                            break;
                        }
                }
            }
            else
            {
                newEx = new ORMException(null, innerException);
            }


            if (conn != null)
            {
                try
                {
                    newEx.Data.Add("ConnectionString", conn.ConnectionString);
                    newEx.Data.Add("ConnectionState", conn.State);
                }
                catch (ObjectDisposedException)
                {
                    newEx.Data.Add("ConnectionState", "Disposed");
                }
            }

            if(comm != null)
            {
                newEx.Data.Add("CommandText", comm.CommandText);
            }

            return newEx;
        }


    }
}
