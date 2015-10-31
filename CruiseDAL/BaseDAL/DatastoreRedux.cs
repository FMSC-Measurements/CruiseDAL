using System;
using System.Collections.Generic;
using System.Text;

using System.Collections;
using System.Data.Common;

using CruiseDAL.Core.EntityModel;
using CruiseDAL.Core.SQL;

namespace CruiseDAL.Core
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

        DbConnectionStringBuilder GetConnectionStringBuilder()
        {
            DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
            builder.Add("Data Source", this.Path);

            return builder;

        }

        EntityDescription LookUpEntityByType(Type t)
        {
            string name = t.Name;
            if(!_entityDescriptionLookup.ContainsKey(name))
            {
                _entityDescriptionLookup.Add(name, new EntityDescription(t, DbProviderFactoryAdapter.Instance));
            }

            return _entityDescriptionLookup[t.Name];
        }
        #endregion

        #region CRUD
        public void Save<T>(T data, Core.SQL.OnConflictOption option) where T : IPersistanceTracking
        {
            Context.Save<T>(data, option);
        }

        public void Save(IEnumerable list)
        {
            this.Save(list, Core.SQL.OnConflictOption.Fail);
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

        public void Update(object data, object key, Core.SQL.OnConflictOption option)
        {
            Context.Update(data, key, option);
        }

        public void Delete(object data)
        {
            Context.Delete(data);
        }



        public Int64 GetRowCount(string tableName, string selection, params Object[] selectionArgs)
        {
            string query = string.Format("SELECT Count(1) FROM {0} {1};", tableName, selection);
            return Context.ExecuteScalar<Int64>(query);
        }

        [Obsolete("use Read<T>(string selection, params Object[] selectionArgs) instead")]
        public List<T> Read<T>(string tableName, string selection, params Object[] selectionArgs) where T : new()
        {
            return Context.Read<>(selection, selectionArgs);
        }

        public List<T> Read<T>(string selection, params Object[] selectionArgs) where T : new()
        {
            return Context.Read<T>(selection, selectionArgs);
        }

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
            return Context.ReadSingleRow<T>(selection, selectionArgs);
        }

        public T ReadSingleRow<T>(string selection, params Object[] selectionArgs) where T : new()
        {
            return Context.ReadSingleRow<T>(selection, selectionArgs);
        }

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
        protected abstract void BuildDBFile();

        protected abstract string GetCreateSQL();

        public abstract void LogMessage(string message, string level);

        public void ChangeRowID(DataObject data, long newRowID, OnConflictOption option)
        {
            throw new NotImplementedException();
        }

        public void Save(IEnumerable list, Core.SQL.OnConflictOption opt);
        #endregion


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
