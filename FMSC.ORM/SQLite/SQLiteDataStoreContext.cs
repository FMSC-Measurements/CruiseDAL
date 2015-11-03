using System;
using System.Data.Common;
using System.Data.SQLite;

using FMSC.ORM.Core;

namespace FMSC.ORM.SQLite
{
    public class SQLiteDataStoreContext : DataStoreContext
    {
        public SQLiteDataStoreContext()
        { }

        protected override Exception ThrowExceptionHelper(DbConnection conn, DbCommand comm, Exception innerException)
        {
            if (innerException is SQLiteException)
            {
                SQLException sqlEx;
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
                            return new FileAccessException(ex.ResultCode.ToString(), innerException);
                        }
                    case SQLiteErrorCode.ReadOnly:
                        {
                            sqlEx = new ReadOnlyException(null, innerException);
                            break;
                        }
                    case SQLiteErrorCode.Constraint:
                        {
                            if (innerException.Message.IndexOf("UNIQUE constraint failed") >= 0)
                            {
                                sqlEx = new UniqueConstraintException(null, innerException);
                            }
                            else
                            {
                                sqlEx = new ConstraintException(null, innerException);
                            }
                            break;
                        }
                    default:
                        {
                            sqlEx = new SQLException(null, innerException);
                            break;
                        }
                }
                if (conn != null)
                {
                    try
                    {
                        sqlEx.ConnectionString = conn.ConnectionString;
                        sqlEx.ConnectionState = conn.State.ToString();
                    }
                    catch (ObjectDisposedException)
                    {
                        sqlEx.ConnectionState = "Disposed";
                    }
                }
                if (comm != null)
                {
                    sqlEx.CommandText = comm.CommandText;
                    //newEx.Data.Add("CommandText", comm.CommandText);
                }

                return sqlEx;
            }
            else
            {
                return new ORMException(null, innerException);
            }
        }


    }
}
