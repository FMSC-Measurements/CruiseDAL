using System;
using System.Collections.Generic;
using System.Text;

using System.Collections;
using System.Data.Common;

using FMSC.ORM.Core.EntityModel;
using FMSC.ORM.Core.SQL;

namespace FMSC.ORM.Core
{
    public abstract class DatastoreRedux
    {
        protected Dictionary<string, EntityDescription> _entityDescriptionLookup = new Dictionary<string, EntityDescription>();

        protected DataStoreContext Context { get; set; }

        public string Path { get; protected set; }


        #region sugar
        DbCommand CreateCommand(string commandText)
        {
            return DbProviderFactoryAdapter.Instance.CreateCommand(commandText);
        }

        DbParameter CreateParameter(string name, object value)
        {
            return DbProviderFactoryAdapter.Instance.CreateParameter(name, value);
        }

        //DbConnectionStringBuilder GetConnectionStringBuilder()
        //{
        //    throw new NotImplementedException();
        //    //DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
        //    //builder.Add("Data Source", this.Path);

        //    //return builder;

        //}

        public EntityDescription LookUpEntityByType(Type t)
        {
            string name = t.Name;
            if(!_entityDescriptionLookup.ContainsKey(name))
            {
                _entityDescriptionLookup.Add(name, new EntityDescription(t, DbProviderFactoryAdapter.Instance));
            }

            return _entityDescriptionLookup[t.Name];
        }
        #endregion

        #region transaction management 
        public void BeginTransaction()
        {
            this.Context.BeginTransaction();
        }

        [Obsolete]
        public void EndTransaction()
        {
            this.Context.CommitTransaction();
        }

        public void CommitTransaction()
        {
            this.Context.CommitTransaction();
        }

        [Obsolete]
        public void CancelTransaction()
        {
            this.Context.RollbackTransaction();
        }

        public void RollbackTransaction()
        {
            this.Context.RollbackTransaction();
        }
        #endregion

        #region CRUD
        public void Save<T>(T data, Core.SQL.OnConflictOption option) where T : IPersistanceTracking
        {
            Context.Save<T>(data, option);
        }

        
        
        public void Insert(object data, Core.SQL.OnConflictOption option)
        {
            Context.Insert(data, option);
        }

        public void Insert(object data, object key, Core.SQL.OnConflictOption option)
        {
            Context.Insert(data, option);
        }

        public void Update(object data, Core.SQL.OnConflictOption option)
        {
            Context.Update(data, option);
        }


        public void Delete(object data)
        {
            Context.Delete(data);
        }



        


        #region read methods
        [Obsolete("use Read<T>(string selection, params Object[] selectionArgs) instead")]
        public List<T> Read<T>(string tableName, string selection, params Object[] selectionArgs) where T : new()
        {
            return Context.Read<T>(selection, selectionArgs);
        }

        public List<T> Read<T>(string selection, params Object[] selectionArgs) where T : new()
        {
            return Context.Read<T>(selection, selectionArgs);
        }

        [Obsolete("use ReadSingleRow<T>(object primaryKey) instead")]
        public T ReadSingleRow<T>(string tableName, long? rowID) where T : new()
        {
            return Context.ReadSingleRow<T>(rowID);
        }

        public T ReadSingleRow<T>(object primaryKey) where T : new()
        {
            return Context.ReadSingleRow<T>(primaryKey);
        }

        [Obsolete]
        public T ReadSingleRow<T>(string tableName, string selection, params Object[] selectionArgs) where T : new()
        {
            throw new NotImplementedException();
            //return Context.ReadSingleRow<T>(selection, selectionArgs);
        }

        public T ReadSingleRow<T>(string selection, params Object[] selectionArgs) where T : new()
        {
            throw new NotImplementedException();
            //return Context.ReadSingleRow<T>(selection, selectionArgs);
        }
        #endregion

        #region query methods
        public List<T> Query<T>(string selectCommand, params Object[] selectionArgs) where T : new()
        {
            DbCommand command = this.CreateCommand(selectCommand);

            //Add selection Arguments to command parameter list
            if (selectionArgs != null)
            {
                foreach (object obj in selectionArgs)
                {
                    command.Parameters.Add(this.CreateParameter(null, obj));
                }
            }

            EntityDescription entityType = LookUpEntityByType(typeof(T));
            return Context.Query<T>(command, entityType);
        }

        public T QuerySingleRecord<T>(string selectCommand, params Object[] selectionArgs) where T : new()
        {
            DbCommand command = this.CreateCommand(selectCommand);

            //Add selection Arguments to command parameter list
            if (selectionArgs != null)
            {
                foreach (object obj in selectionArgs)
                {
                    command.Parameters.Add(this.CreateParameter(null, obj));
                }
            }

            EntityDescription entityType = LookUpEntityByType(typeof(T));
            return Context.QuerySingleRecord<T>(command, entityType);
        }
        #endregion

        #endregion

        #region general purpose command execution
        public int? Execute(string command, params object[] parameters)
        {
            return Context.Execute(command, parameters);
        }


        public object ExecuteScalar(string query, params object[] parameters)
        {
            return Context.ExecuteScalar(query, parameters);
        }

        public T ExecuteScalar<T>(string query, params object[] parameters)
        {
            return Context.ExecuteScalar<T>(query, parameters);
        }
        #endregion

        #region not implemented 

        
        #endregion




        public Int64 GetRowCount(string tableName, string selection, params Object[] selectionArgs)
        {
            string query = string.Format("SELECT Count(1) FROM {0} {1};", tableName, selection);
            return Context.ExecuteScalar<Int64>(query);
        }

        public string GetTableSQL(String tableName)
        {
            return (String)this.ExecuteScalar("SELECT sql FROM Sqlite_master WHERE name = ? COLLATE NOCASE and type = 'table';", tableName);
        }

        public string[] GetTableUniques(String tableName)
        {
            String tableSQL = this.GetTableSQL(tableName);
            System.Text.RegularExpressions.Match match =
                System.Text.RegularExpressions.Regex.Match(tableSQL, @"(?<=^\s+UNIQUE\s\()[^\)]+(?=\))", System.Text.RegularExpressions.RegexOptions.Multiline);
            if (match != null && match.Success)
            {
                String[] a = match.Value.Split(new char[] { ',', ' ', '\r', '\n' });
                int numNotEmpty = 0;
                foreach (string s in a)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        numNotEmpty++;
                    }
                }
                string[] b = new string[numNotEmpty];
                for (int i = 0, j = 0; i < a.Length; i++)
                {
                    if (!string.IsNullOrEmpty(a[i]))
                    {
                        b[j] = a[i];
                        j++;
                    }
                }

                return b;
                //return match.Value.Split(new char[]{',',' ','\r','\n'},  StringSplitOptions.RemoveEmptyEntries);

            }
            return new string[0];

        }
    }
}
