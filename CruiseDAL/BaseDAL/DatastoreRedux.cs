using System;
using System.Collections.Generic;
using System.Text;

using System.Collections;
using System.Data.Common;

using CruiseDAL.Core;
using CruiseDAL.Core.SQL;
using CruiseDAL.Core.EntityModel;

namespace CruiseDAL.BaseDAL
{
    public class DatastoreRedux
    {
        protected Dictionary<string, EntityDescription> _entityDescriptionLookup = new Dictionary<string, EntityDescription>();

        protected DataStoreContext Context { get; set; }

        public string Path { get; protected set; }
        public bool Exists
        {
            get
            {
                return System.IO.File.Exists(this.Path);
            }
        }

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

        public void Save<T>(T data, SQLConflictOption option) where T : IPersistanceTracking
        {
            Context.Save<T>(data, option);
        }

        public void Save(IEnumerable list)
        {
            this.Save(list, SQLConflictOption.Fail);
        }

        public void Save(IEnumerable list, SQLConflictOption opt);

        public void Insert(object data, SQLConflictOption option)
        {
            Context.Insert(data, option);
        }

        public void Insert(object data, object key, SQLConflictOption option)
        {
            Context.Insert(data, option);
        }

        public void Update(object data, SQLConflictOption option)
        {
            Context.Update(data, option);
        }

        public void Update(object data, object key, SQLConflictOption option)
        {
            Context.Update(data, key, option);
        }

        public void Delete(object data)
        {
            Context.Delete(data);
        }

        public void Delete(string tableName, object key);


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



    }
}
