using Backpack.SqlBuilder;
using FMSC.ORM.EntityModel;
using FMSC.ORM.EntityModel.Support;
using FMSC.ORM.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;

namespace FMSC.ORM.Core
{
    public static class ConnectionExtentions
    {
        private static ILogger _logger = LoggerProvider.Get();

        #region ExecuteNonQuery

        public static int ExecuteNonQuery(this DbConnection connection, string commandText, object[] parameters = null, DbTransaction transaction = null)
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

        public static int ExecuteNonQuery2(this DbConnection connection, string commandText, object parameterData = null, DbTransaction transaction = null)
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

        public static int ExecuteNonQuery(this DbConnection connection, DbCommand command, DbTransaction transaction = null)
        {
            if (connection == null) { throw new ArgumentNullException("connection"); }
            if (command == null) { throw new ArgumentNullException("command"); }

            command.Connection = connection;
            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            _logger.LogCommand(command);

            return command.ExecuteNonQuery();
        }

        #endregion ExecuteNonQuery

        #region ExecuteReader

        public static DbDataReader ExecuteReader(this DbConnection connection, DbCommand command, DbTransaction transaction = null)
        {
            if (connection == null) { throw new ArgumentNullException("connection"); }
            if (command == null) { throw new ArgumentNullException("command"); }

            command.Connection = connection;
            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            _logger.LogCommand(command);

            return command.ExecuteReader();
        }

        #endregion ExecuteReader

        #region ExecuteScalar

        public static object ExecuteScalar(this DbConnection connection, string commandText, object[] parameters = null, DbTransaction transaction = null)
        {
            if (connection is null) { throw new ArgumentNullException(nameof(connection)); }
            if (commandText is null) { throw new ArgumentNullException(nameof(commandText)); }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.SetParams(parameters);

                command.Transaction = transaction;

                return command.ExecuteScalar();
            }
        }

        public static object ExecuteScalar2(this DbConnection connection, string commandText, object parameterData = null, DbTransaction transaction = null)
        {
            if (connection is null) { throw new ArgumentNullException(nameof(connection)); }
            if (string.IsNullOrEmpty(commandText)) { throw new ArgumentException("commandText can not be null or empty", nameof(commandText)); }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.AddParams(parameterData);

                if (transaction != null)
                {
                    command.Transaction = transaction;
                }

                return command.ExecuteScalar();
            }
        }

        public static T ExecuteScalar<T>(this DbConnection connection, string commandText, object[] parameters = null, DbTransaction transaction = null)
        {
            var result = ExecuteScalar(connection, commandText, parameters, transaction);

            return ValueMapper.ProcessValue<T>(result);
        }

        public static T ExecuteScalar2<T>(this DbConnection connection, string commandText, object parameters = null, DbTransaction transaction = null)
        {
            var result = ExecuteScalar2(connection, commandText, parameters, transaction);

            return ValueMapper.ProcessValue<T>(result);
        }

        #endregion ExecuteScalar

        #region CRUD

        #region QueryScalar

        public static IEnumerable<TResult> QueryScalar<TResult>(this DbConnection connection, string commandText, object[] paramaters = null, DbTransaction transaction = null, IExceptionProcessor exceptionProcessor = null)
        {
            var targetType = typeof(TResult);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.SetParams(paramaters);

                using (var reader = connection.ExecuteReader(command, transaction))
                {
                    while (reader.Read())
                    {
                        TResult value = default(TResult);
                        try
                        {
                            var dbValue = reader.GetValue(0);
                            value = (TResult)ValueMapper.ProcessValue(targetType, dbValue);
                        }
                        catch (Exception e)
                        {
                            if (exceptionProcessor != null)
                            {
                                throw exceptionProcessor.ProcessException(e, connection, commandText, transaction);
                            }
                            else
                            {
                                throw;
                            }
                        }

                        yield return value;
                    }
                }
            }
        }

        public static IEnumerable<TResult> QueryScalar2<TResult>(this DbConnection connection, string commandText, object paramaters = null, DbTransaction transaction = null, IExceptionProcessor exceptionProcessor = null)
        {
            var targetType = typeof(TResult);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.AddParams(paramaters);

                using (var reader = connection.ExecuteReader(command, transaction))
                {
                    while (reader.Read())
                    {
                        TResult value = default(TResult);
                        try
                        {
                            var dbValue = reader.GetValue(0);
                            value = (TResult)ValueMapper.ProcessValue(targetType, dbValue);
                        }
                        catch (Exception e)
                        {
                            if (exceptionProcessor != null)
                            {
                                throw exceptionProcessor.ProcessException(e, connection, commandText, transaction);
                            }
                            else
                            {
                                throw;
                            }
                        }

                        yield return value;
                    }
                }
            }
        }

        #endregion QueryScalar

        public static void Delete(this DbConnection connection, object data, DbTransaction transaction = null)
        {
            if (connection is null) { throw new ArgumentNullException(nameof(connection)); }
            if (data is null) { throw new ArgumentNullException("data"); }

            EntityDescription entityDescription = GlobalEntityDescriptionLookup.Instance.LookUpEntityByType(data.GetType());
            var keyFieldInfo = entityDescription.Fields.PrimaryKeyField;

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

        public static void Update(this DbConnection connection, object data, OnConflictOption option, DbTransaction transaction = null)
        {
            if (data is null) { throw new ArgumentNullException("data"); }

            EntityDescription entityDescription = GlobalEntityDescriptionLookup.Instance.LookUpEntityByType(data.GetType());
            EntityCommandBuilder builder = entityDescription.CommandBuilder;
            var keyField = entityDescription.Fields.PrimaryKeyField;
            object keyData = keyField.GetFieldValue(data);

            Update(connection, data, keyData, option, transaction);
        }

        public static void Update(this DbConnection connection, object data, object keyData, OnConflictOption option, DbTransaction transaction = null)
        {
            if (data is null) { throw new ArgumentNullException("data"); }

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