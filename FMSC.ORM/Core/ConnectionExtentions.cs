using Backpack.SqlBuilder;
using FMSC.ORM.EntityModel;
using FMSC.ORM.EntityModel.Attributes;
using FMSC.ORM.EntityModel.Support;
using System;
using System.Data.Common;
using System.Diagnostics;

namespace FMSC.ORM.Core
{
    public static class ConnectionExtentions
    {
        private static Logger _logger = new Logger();

        #region ExecuteNonQuery

        public static int ExecuteNonQuery(this DbConnection connection, string commandText, object[] parameters, DbTransaction transaction)
        {
            if (string.IsNullOrEmpty(commandText)) { throw new ArgumentException("command can't be null or empty", "command"); }
            if (connection == null) { throw new ArgumentNullException("connection"); }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.SetParams(parameters);

                return ConnectionExtentions.ExecuteNonQuery(connection, command, transaction);
            }
        }

        public static int ExecuteNonQuery2(this DbConnection connection, string commandText, object parameterData, DbTransaction transaction)
        {
            if (string.IsNullOrEmpty(commandText)) { throw new ArgumentException("command can't be null or empty", "command"); }
            if (connection == null) { throw new ArgumentNullException("connection"); }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.AddParams(parameterData);

                return ConnectionExtentions.ExecuteNonQuery(connection, command, transaction);
            }
        }

        public static int ExecuteNonQuery(this DbConnection connection, DbCommand command, DbTransaction transaction)
        {
            if (connection == null) { throw new ArgumentNullException("connection"); }
            if (command == null) { throw new ArgumentNullException("command"); }

            command.Connection = connection;
            command.Transaction = transaction;

            _logger.LogCommand(command);

            return command.ExecuteNonQuery();
        }

        #endregion ExecuteNonQuery

        #region ExecuteReader

        public static DbDataReader ExecuteReader(this DbConnection connection, string commandText, object[] paramaters, DbTransaction transaction)
        {
            if (string.IsNullOrEmpty(commandText)) { throw new ArgumentException("command can't be null or empty", "command"); }
            if (connection == null) { throw new ArgumentNullException("connection"); }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.SetParams(paramaters);

                return ExecuteReader(connection, command, transaction);
            }
        }

        public static DbDataReader ExecuteReader2(this DbConnection connection, string commandText, object paramaterData, DbTransaction transaction)
        {
            if (string.IsNullOrEmpty(commandText)) { throw new ArgumentException("command can't be null or empty", "command"); }
            if (connection == null) { throw new ArgumentNullException("connection"); }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.AddParams(paramaterData);

                return ExecuteReader(connection, command, transaction);
            }
        }

        public static DbDataReader ExecuteReader(this DbConnection connection, DbCommand command, DbTransaction transaction)
        {
            if (connection == null) { throw new ArgumentNullException("connection"); }
            if (command == null) { throw new ArgumentNullException("command"); }

            command.Connection = connection;
            command.Transaction = transaction;

            _logger.LogCommand(command);

            return command.ExecuteReader();
        }

        #endregion ExecuteReader

        #region ExecuteScalar

        public static object ExecuteScalar(this DbConnection connection, string commandText, object[] parameters, DbTransaction transaction)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.SetParams(parameters);

                command.Transaction = transaction;

                return command.ExecuteScalar();
            }
        }

        public static object ExecuteScalar2(this DbConnection connection, string commandText, object parameterData, DbTransaction transaction)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.AddParams(parameterData);

                command.Transaction = transaction;

                return command.ExecuteScalar();
            }
        }

        public static T ExecuteScalar<T>(this DbConnection connection, string commandText, object[] parameters, DbTransaction transaction)
        {
            var result = ExecuteScalar(connection, commandText, parameters, transaction);

            if (result == null || result is DBNull)
            {
                return default(T);
            }
            else if (result is T)
            {
                return (T)result;
            }
            else
            {
                Type targetType = typeof(T);
                targetType = Nullable.GetUnderlyingType(targetType) ?? targetType;

                if (result is IConvertible)
                {
                    return (T)Convert.ChangeType(result, targetType
                        , System.Globalization.CultureInfo.CurrentCulture);
                }
                else
                {
                    try
                    {
                        return (T)result;
                    }
                    catch (InvalidCastException)
                    {
#if NetCF
                        throw;
#else
                        return (T)Activator.CreateInstance(targetType, result);
#endif
                    }
                }
            }
        }

        #endregion ExecuteScalar

        #region CRUD

        public static void Delete(this DbConnection connection, object data, DbTransaction transaction = null)
        {
            if (data == null) { throw new ArgumentNullException("data"); }

            EntityDescription entityDescription = GlobalEntityDescriptionLookup.Instance.LookUpEntityByType(data.GetType());
            PrimaryKeyFieldAttribute keyFieldInfo = entityDescription.Fields.PrimaryKeyField;

            if (keyFieldInfo == null) { throw new InvalidOperationException("type doesn't have primary key field"); }

            EntityCommandBuilder builder = entityDescription.CommandBuilder;

            lock (data)
            {
                if (data is IPersistanceTracking)
                {
                    ((IPersistanceTracking)data).OnDeleting();
                }

                using (var command = connection.CreateCommand())
                {
                    builder.BuildSQLDeleteCommand(command, data);

                    ExecuteNonQuery(connection, command, transaction);
                }

                if (data is IPersistanceTracking)
                {
                    ((IPersistanceTracking)data).OnDeleted();
                }
            }
        }

        //public static object Insert(this DbConnection connection, object data, DbTransaction transaction, OnConflictOption option)
        //{
        //    if (data == null) { throw new ArgumentNullException("data"); }

        //    EntityDescription entityDescription = GlobalEntityDescriptionLookup.Instance.LookUpEntityByType(data.GetType());
        //    var keyField = entityDescription.Fields.PrimaryKeyField;
        //    object keyData = (keyField != null) ? keyField.GetFieldValue(data) : null;
        //    return Insert(connection, data, keyData, transaction, option);
        //}

        //public static object Insert(this DbConnection connection, object data, object keyData, DbTransaction transaction, OnConflictOption option)
        //{
        //    if (data == null) { throw new ArgumentNullException("data"); }

        //    EntityDescription entityDescription = GlobalEntityDescriptionLookup.Instance.LookUpEntityByType(data.GetType());
        //    EntityCommandBuilder builder = entityDescription.CommandBuilder;

        //    if (data is IPersistanceTracking)
        //    {
        //        ((IPersistanceTracking)data).OnInserting();
        //    }

        //    using (var command = connection.CreateCommand())
        //    {
        //        builder.BuildInsertCommand(command, data, keyData, option);

        //        var changes = ExecuteNonQuery(connection, command, transaction);
        //        if (changes == 0)   //command did not result in any changes to the database
        //        {
        //            return null;    //so do not try to get the rowid or call OnInsertedData
        //        }
        //        else
        //        {
        //            var primaryKeyField = entityDescription.Fields.PrimaryKeyField;
        //            if (primaryKeyField != null)
        //            {
        //                if (primaryKeyField.KeyType == KeyType.RowID)
        //                {
        //                    keyData = GetLastInsertRowID(connection, transaction);
        //                }
        //                else
        //                {
        //                    keyData = GetLastInsertKeyValue(connection, entityDescription.SourceName, primaryKeyField.Name, transaction);
        //                }

        //                primaryKeyField.SetFieldValue(data, keyData);
        //            }

        //            if (data is IPersistanceTracking)
        //            {
        //                ((IPersistanceTracking)data).OnInserted();
        //            }

        //            return keyData;
        //        }
        //    }
        //}

        public static void Update(this DbConnection connection, object data, DbTransaction transaction, OnConflictOption option)
        {
            if (data == null) { throw new ArgumentNullException("data"); }

            EntityDescription entityDescription = GlobalEntityDescriptionLookup.Instance.LookUpEntityByType(data.GetType());
            EntityCommandBuilder builder = entityDescription.CommandBuilder;
            var keyField = entityDescription.Fields.PrimaryKeyField;
            object keyData = keyField.GetFieldValue(data);

            Update(connection, data, keyData, transaction, option);
        }

        public static void Update(this DbConnection connection, object data, object keyData, DbTransaction transaction, OnConflictOption option)
        {
            if (data == null) { throw new ArgumentNullException("data"); }

            EntityDescription entityDescription = GlobalEntityDescriptionLookup.Instance.LookUpEntityByType(data.GetType());
            EntityCommandBuilder builder = entityDescription.CommandBuilder;

            if (data is IPersistanceTracking)
            {
                ((IPersistanceTracking)data).OnUpdating();
            }

            using (var command = connection.CreateCommand())
            {
                builder.BuildUpdateCommand(command, data, keyData, option);

                var changes = ExecuteNonQuery(connection, command, transaction);
                if (option != OnConflictOption.Ignore)
                {
                    Debug.Assert(changes > 0, "update command resulted in no changes");
                }
            }

            if (data is IPersistanceTracking)
            {
                ((IPersistanceTracking)data).OnUpdated();
            }
        }

        #endregion CRUD
    }
}