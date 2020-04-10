using Backpack.SqlBuilder;
using FMSC.ORM.EntityModel;
using FMSC.ORM.EntityModel.Attributes;
using FMSC.ORM.EntityModel.Support;
using FMSC.ORM.Logging;
using FMSC.ORM.Sql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;

namespace FMSC.ORM.Core
{
    public static class ConnectionExtentions
    {
        private static ILogger Logger { get; } = LoggerProvider.Get();
        private static ICommandBuilder DefaultCommandBuilder { get; set; } = new CommandBuilder();
        private static IEntityDescriptionLookup DescriptionLookup { get; set; } = GlobalEntityDescriptionLookup.Instance;

        #region ExecuteNonQuery

        public static int ExecuteNonQuery(this DbConnection connection, string commandText, object[] parameters = null,
                                          DbTransaction transaction = null,
                                          IExceptionProcessor exceptionProcessor = null)
        {
            if (string.IsNullOrEmpty(commandText)) { throw new ArgumentException("command can't be null or empty", "command"); }
            if (connection == null) { throw new ArgumentNullException("connection"); }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.SetParams(parameters);

                try
                {
                    return ConnectionExtentions.ExecuteNonQuery(connection, command, transaction);
                }
                catch (Exception e)
                {
                    if (exceptionProcessor is null) { throw; }
                    else
                    {
                        throw exceptionProcessor.ProcessException(e, connection, commandText, transaction);
                    }
                }
            }
        }

        public static int ExecuteNonQuery2(this DbConnection connection, string commandText, object parameterData = null,
                                           DbTransaction transaction = null,
                                           IExceptionProcessor exceptionProcessor = null)
        {
            if (string.IsNullOrEmpty(commandText)) { throw new ArgumentException("command can't be null or empty", "command"); }
            if (connection == null) { throw new ArgumentNullException("connection"); }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.AddParams(parameterData);

                try
                {
                    return ConnectionExtentions.ExecuteNonQuery(connection, command, transaction);
                }
                catch (Exception e)
                {
                    if (exceptionProcessor is null) { throw; }
                    else
                    { throw exceptionProcessor.ProcessException(e, connection, commandText, transaction); }
                }
            }
        }

        public static int ExecuteNonQuery(this DbConnection connection, DbCommand command,
                                          DbTransaction transaction = null,
                                          IExceptionProcessor exceptionProcessor = null)
        {
            if (connection == null) { throw new ArgumentNullException("connection"); }
            if (command == null) { throw new ArgumentNullException("command"); }

            command.Connection = connection;
            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            Logger.LogCommand(command);

            try
            {
                return command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                if (exceptionProcessor is null) { throw; }
                else { throw exceptionProcessor.ProcessException(e, connection, command.CommandText, transaction); }
            }
        }

        #endregion ExecuteNonQuery

        #region ExecuteReader

        public static DbDataReader ExecuteReader(this DbConnection connection, DbCommand command,
                                                 DbTransaction transaction = null,
                                                 IExceptionProcessor exceptionProcessor = null)
        {
            if (connection == null) { throw new ArgumentNullException("connection"); }
            if (command == null) { throw new ArgumentNullException("command"); }

            command.Connection = connection;
            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            Logger.LogCommand(command);

            try
            {
                return command.ExecuteReader();
            }
            catch (Exception e)
            {
                if (exceptionProcessor is null) { throw; }
                else { throw exceptionProcessor.ProcessException(e, connection, command.CommandText, transaction); }
            }
        }

        #endregion ExecuteReader

        #region ExecuteScalar

        public static object ExecuteScalar(this DbConnection connection, string commandText, object[] parameters = null,
                                           DbTransaction transaction = null,
                                           IExceptionProcessor exceptionProcessor = null)
        {
            if (connection is null) { throw new ArgumentNullException(nameof(connection)); }
            if (commandText is null) { throw new ArgumentNullException(nameof(commandText)); }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.SetParams(parameters);

                if (transaction != null)
                {
                    command.Transaction = transaction;
                }

                try
                {
                    return command.ExecuteScalar();
                }
                catch (Exception e)
                {
                    if (exceptionProcessor is null) { throw; }
                    else
                    { throw exceptionProcessor.ProcessException(e, connection, commandText, transaction); }
                }
            }
        }

        public static object ExecuteScalar2(this DbConnection connection, string commandText,
                                            object parameterData = null, DbTransaction transaction = null,
                                            IExceptionProcessor exceptionProcessor = null)
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

                try
                {
                    return command.ExecuteScalar();
                }
                catch (Exception e)
                {
                    if (exceptionProcessor is null) { throw; }
                    else
                    { throw exceptionProcessor.ProcessException(e, connection, commandText, transaction); }
                }
            }
        }

        public static T ExecuteScalar<T>(this DbConnection connection, string commandText, object[] parameters = null,
                                         DbTransaction transaction = null, IExceptionProcessor exceptionProcessor = null)
        {
            var result = ExecuteScalar(connection, commandText, parameters, transaction, exceptionProcessor);

            return ValueMapper.ProcessValue<T>(result);
        }

        public static T ExecuteScalar2<T>(this DbConnection connection, string commandText, object parameters = null,
                                          DbTransaction transaction = null,
                                          IExceptionProcessor exceptionProcessor = null)
        {
            var result = ExecuteScalar2(connection, commandText, parameters, transaction, exceptionProcessor);

            return ValueMapper.ProcessValue<T>(result);
        }

        #endregion ExecuteScalar

        #region CRUD

        #region QueryScalar

        public static IEnumerable<TResult> QueryScalar<TResult>(this DbConnection connection, string commandText,
                                                                object[] paramaters = null,
                                                                DbTransaction transaction = null,
                                                                IExceptionProcessor exceptionProcessor = null)
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

        public static IEnumerable<TResult> QueryScalar2<TResult>(this DbConnection connection, string commandText,
                                                                 object paramaters = null,
                                                                 DbTransaction transaction = null,
                                                                 IExceptionProcessor exceptionProcessor = null)
        {
            var targetType = typeof(TResult);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.AddParams(paramaters);

                using (var reader = connection.ExecuteReader(command, transaction: transaction, exceptionProcessor: exceptionProcessor))
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

        #region QueryGeneric
        public static IEnumerable<GenericEntity> QueryGeneric(this DbConnection connection, string commandText, DbTransaction transaction = null, IExceptionProcessor exceptionProcessor = null)
        {
            return connection.QueryGeneric2(commandText, (object)null, transaction: transaction, exceptionProcessor: exceptionProcessor);
        }

        public static IEnumerable<GenericEntity> QueryGeneric2(this DbConnection connection, string commandText, object paramaters, DbTransaction transaction = null, IExceptionProcessor exceptionProcessor = null)
        {

            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.AddParams(paramaters);

                using (var reader = connection.ExecuteReader(command, transaction: transaction, exceptionProcessor: exceptionProcessor))
                {
                    while (reader.Read())
                    {
                        var fieldCount = reader.FieldCount;
                        var fields = new string[fieldCount];
                        for (int i = 0; i < fieldCount; i++)
                        {
                            fields[i] = reader.GetName(i);
                        }

                        var data = new GenericEntity(fieldCount);

                        try
                        {
                            foreach (var x in fields.Select((field, i) => new { field, i }))
                            {

                                var value = reader.GetValue(x.i);
                                data.Add(x.field, value);
                            }
                        }
                        catch (Exception e)
                        {
                            if (exceptionProcessor is null) { throw; }
                            else
                            { throw exceptionProcessor.ProcessException(e, connection, commandText, transaction); }
                        }

                        yield return data;

                    }
                }
            }


        }
        #endregion 

        public static object Insert(this DbConnection connection, object data, string tableName = null,
                                    EntityDescription entityDescription = null, DbTransaction transaction = null,
                                    OnConflictOption option = OnConflictOption.Default,
                                    ICommandBuilder commandBuilder = null, IExceptionProcessor exceptionProcessor = null,
                                    object keyValue = null)
        {
            if (data is null) { throw new ArgumentNullException(nameof(data)); }

            entityDescription = entityDescription ?? DescriptionLookup.LookUpEntityByType(data.GetType());
            tableName = tableName ?? entityDescription.SourceName;

            return Insert(connection, data, tableName, entityDescription.Fields,
                transaction: transaction,
                option: option,
                commandBuilder: commandBuilder,
                exceptionProcessor: exceptionProcessor,
                keyValue: keyValue);
        }

        internal static object Insert(this DbConnection connection, object data, string tableName,
                                      IFieldInfoCollection fields, DbTransaction transaction = null,
                                      OnConflictOption option = OnConflictOption.Default,
                                      ICommandBuilder commandBuilder = null,
                                      IExceptionProcessor exceptionProcessor = null, object keyValue = null)
        {
            if (data == null) { throw new ArgumentNullException("data"); }

            commandBuilder = commandBuilder ?? DefaultCommandBuilder;

            if (data is IPersistanceTracking ptData) { ptData.OnInserting(); }


            using (var command = connection.CreateCommand())
            {
                commandBuilder.BuildInsert(command, data, tableName, fields,
                    option: option,
                    keyValue: keyValue);

                try
                {
                    var id = command.ExecuteScalar();

                    var pkField = fields.PrimaryKeyField;
                    if (pkField != null)
                    {
                        var value = ValueMapper.ProcessValue(pkField.RunTimeType, id);
                        pkField.SetFieldValue(data, value);

                        return value;
                    }
                    else
                    { return id; }
                }
                catch (Exception ex)
                {
                    if (exceptionProcessor is null)
                    { throw; }
                    else { throw exceptionProcessor.ProcessException(ex, connection, (string)null, transaction); }
                }
            }
        }

        public static void Update(this DbConnection connection, object data, string tableName = null,
                                  EntityDescription entityDescription = null,
                                  OnConflictOption option = OnConflictOption.Default, DbTransaction transaction = null,
                                  IExceptionProcessor exceptionProcessor = null, object keyValue = null)
        {
            if (data is null) { throw new ArgumentNullException("data"); }

            entityDescription = entityDescription ?? DescriptionLookup.LookUpEntityByType(data.GetType());
            tableName = tableName ?? entityDescription.SourceName;

            Update(connection, data, tableName, entityDescription.Fields,
                option: option,
                transaction: transaction,
                exceptionProcessor: exceptionProcessor,
                keyValue: keyValue);
        }

        internal static void Update(this DbConnection connection, object data, string tableName,
                                  IFieldInfoCollection fields, OnConflictOption option = OnConflictOption.Default,
                                  DbTransaction transaction = null, ICommandBuilder commandBuilder = null,
                                  IExceptionProcessor exceptionProcessor = null, object keyValue = null)
        {
            if (data is null) { throw new ArgumentNullException("data"); }

            commandBuilder = commandBuilder ?? DefaultCommandBuilder;

            if (data is IPersistanceTracking)
            {
                ((IPersistanceTracking)data).OnUpdating();
            }

            using (var command = connection.CreateCommand())
            {
                commandBuilder.BuildUpdate(command, data, tableName, fields, option, keyValue);

                try
                {
                    var changes = ExecuteNonQuery(connection, command, transaction);
                    if (option != OnConflictOption.Ignore)
                    {
                        Debug.Assert(changes > 0, "update command resulted in no changes");
                    }
                }
                catch (Exception e)
                {
                    if (exceptionProcessor != null)
                    { exceptionProcessor.ProcessException(e, connection, command.CommandText, transaction); }
                    else { throw; }
                }
            }

            if (data is IPersistanceTracking)
            {
                ((IPersistanceTracking)data).OnUpdated();
            }
        }

        public static void Delete(this DbConnection connection, object data, string tableName = null,
                                  EntityDescription entityDescription = null, DbTransaction transaction = null,
                                  ICommandBuilder commandBuilder = null, IExceptionProcessor exceptionProcessor = null)
        {
            if (data is null) { throw new ArgumentNullException(nameof(data)); }

            entityDescription = entityDescription ?? DescriptionLookup.LookUpEntityByType(data.GetType());
            tableName = tableName ?? entityDescription.SourceName;

            Delete(connection, data, tableName, entityDescription.Fields, transaction, commandBuilder);
        }

        internal static void Delete(this DbConnection connection, object data, string tableName,
                                  IFieldInfoCollection fields, DbTransaction transaction = null,
                                  ICommandBuilder commandBuilder = null, IExceptionProcessor exceptionProcessor = null)
        {
            if (connection is null) { throw new ArgumentNullException(nameof(connection)); }
            if (data is null) { throw new ArgumentNullException("data"); }

            commandBuilder = commandBuilder ?? DefaultCommandBuilder;

            lock (data)
            {
                if (data is IPersistanceTracking)
                {
                    ((IPersistanceTracking)data).OnDeleting();
                }

                using (var command = connection.CreateCommand())
                {
                    commandBuilder.BuildDelete(command, data, tableName, fields);

                    try
                    {
                        ExecuteNonQuery(connection, command, transaction);
                    }
                    catch (Exception e)
                    {
                        if (exceptionProcessor is null) throw;
                        else
                        { exceptionProcessor.ProcessException(e, connection, command.CommandText, transaction); }
                    }
                }

                if (data is IPersistanceTracking)
                {
                    ((IPersistanceTracking)data).OnDeleted();
                }
            }
        }

        #endregion CRUD

        public static long GetLastInsertRowID(this DbConnection connection, DbTransaction transaction = null,
                                              IExceptionProcessor exceptionProcessor = null)
        {
            if (connection is null) { throw new ArgumentNullException(nameof(connection)); }

            var cmdText = "SELECT last_insert_rowid();";
            try
            {
                return connection.ExecuteScalar<long>(cmdText, (object[])null, transaction);
            }
            catch (Exception e)
            {
                if (exceptionProcessor != null)
                { throw exceptionProcessor.ProcessException(e, connection, cmdText, transaction); }
                else { throw; }
            }
        }

    }
}