using System;
using System.Data.Common;
using System.Data.SQLite;

using FMSC.ORM.Core;
using System.Collections.Generic;
using FMSC.ORM.Core.SQL;

namespace FMSC.ORM.SQLite
{
    public class SQLiteDataStoreContext : DataStoreContext
    {
        public SQLiteDataStoreContext()
        { }


        public override List<ColumnInfo> GetTableInfo(string tableName)
        {
            List<ColumnInfo> colList = new List<ColumnInfo>();
            lock (_readOnlyConnectionSyncLock)
            {

                using (DbCommand command = this.CreateCommand("PRAGMA table_info(" + tableName + ");"))
                {
                    using (DbConnection conn = OpenReadOnlyConnection())
                    {
                        command.Connection = conn;

                        try
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                int nameOrd = reader.GetOrdinal("name");
                                int dbTypeOrd = reader.GetOrdinal("type");
                                int pkOrd = reader.GetOrdinal("pk");
                                int notNullOrd = reader.GetOrdinal("notnull");
                                int defaultValOrd = reader.GetOrdinal("dflt_value");
                                while (reader.Read())
                                {
                                    ColumnInfo colInfo = new ColumnInfo();
                                    colInfo.Name = reader.GetString(nameOrd);
                                    colInfo.DBType = reader.GetString(dbTypeOrd);
                                    colInfo.IsPK = reader.GetBoolean(pkOrd);
                                    colInfo.IsRequired = reader.GetBoolean(notNullOrd);
                                    if (!reader.IsDBNull(defaultValOrd))
                                    {
                                        colInfo.Default = reader.GetString(defaultValOrd);
                                    }
                                    colList.Add(colInfo);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            throw this.ThrowExceptionHelper(conn, command, e);
                        }
                        finally
                        {
                            ReleaseReadOnlyConnection();
                        }
                    }
                }
                return colList;
            }
        }

        public override bool HasForeignKeyErrors(string table_name)
        {
            bool hasErrors = false;
            string comStr;
            if (string.IsNullOrEmpty(table_name))
            {
                comStr = "PRAGMA foreign_key_check;";
            }
            else
            {
                comStr = "PRAGMA foreign_key_check(" + table_name + ");";
            }
            lock (_readOnlyConnectionSyncLock)
            {
                using (DbCommand command = this.CreateCommand(comStr))
                {
                    using (DbConnection conn = OpenReadOnlyConnection())
                    {
                        command.Connection = conn;

                        try
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                hasErrors = reader.Read();
                            }
                        }
                        catch (Exception e)
                        {
                            throw this.ThrowExceptionHelper(conn, command, e);
                        }
                        finally
                        {
                            ReleaseReadOnlyConnection();
                        }
                    }
                }
                return hasErrors;
            }
        }

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
