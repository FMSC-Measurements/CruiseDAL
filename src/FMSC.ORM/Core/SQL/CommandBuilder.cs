using Backpack.SqlBuilder;
using FMSC.ORM.Core;
using FMSC.ORM.EntityModel.Attributes;
using FMSC.ORM.EntityModel.Support;
using System;
using System.Collections.Generic;
using System.Data;

namespace FMSC.ORM.Sql
{
    public class CommandBuilder : ICommandBuilder
    {
        public static string GetParameterName(FieldInfo field) => "@" + field.Name;

        public static IDbDataParameter MakeParameter(IDbCommand command, FieldInfo field, object value)
        {
            var param = command.CreateParameter();
            param.ParameterName = GetParameterName(field);
            param.Value = value;
            return param;
        }

        public SqlSelectBuilder BuildSelect(TableOrSubQuery source, IFieldInfoCollection fields)
        {
            if (source is null) { throw new ArgumentNullException(nameof(source)); }
            if (fields is null) { throw new ArgumentNullException(nameof(fields)); }

            var selectBuilder = new SqlSelectBuilder();
            selectBuilder.Source = source;

            foreach (var field in fields)
            {
                var resultColExpr = MakeResultColumnExpression(field, source);
                selectBuilder.ResultColumns.Add(resultColExpr);
            }

            return selectBuilder;
        }

        protected static string MakeResultColumnExpression(FieldInfo field, TableOrSubQuery source)
        {
            var alias = field.Alias;
            if (alias != null)
            {
                return field.SQLExpression + " AS " + alias;
            }
            else
            {
                var sourceName = source.SourceName;
                if (string.IsNullOrEmpty(sourceName)) { return field.Name; }
                else { return sourceName + "." + field.Name; }
            }
        }

        public void BuildInsert(IDbCommand command, object data, string tableName, IFieldInfoCollection fields, OnConflictOption option = OnConflictOption.Default, object keyValue = null)
        {
            if (command is null) { throw new ArgumentNullException(nameof(command)); }
            if (data is null) { throw new ArgumentNullException(nameof(data)); }
            if (fields is null) { throw new ArgumentNullException(nameof(fields)); }
            if (tableName is null) { throw new ArgumentNullException(nameof(tableName)); }

            var columnNames = new List<string>();
            var valueExpressions = new List<string>();

            var pkField = fields.PrimaryKeyField;
            if(pkField != null 
                && (pkField.PersistanceFlags & PersistanceFlags.OnInsert) == PersistanceFlags.OnInsert 
                && keyValue == null)
            {
                keyValue = pkField.GetFieldValueOrDefault(data);
            }

            if (keyValue != null)
            {
                if (pkField is null)
                {
                    throw new InvalidOperationException($"keyValue provided but type has no key field type:{data.GetType().Name}");
                }

                var param = MakeParameter(command, pkField, keyValue);
                command.Parameters.Add(param);

                columnNames.Add(pkField.Name);
                valueExpressions.Add(param.ParameterName);
            }

            foreach (var field in fields)
            {
                if ((field.PersistanceFlags & PersistanceFlags.OnInsert) == PersistanceFlags.OnInsert
                    && field.IsKeyField == false)
                {
                    object value = field.GetFieldValueOrDefault(data) ?? DBNull.Value;
                    var param = MakeParameter(command, field, value);
                    command.Parameters.Add(param);

                    columnNames.Add(field.Name);
                    valueExpressions.Add(param.ParameterName);
                }
            }

            var builder = new SqlInsertCommand
            {
                TableName = tableName,
                ConflictOption = option,
                ColumnNames = columnNames,
                ValueExpressions = valueExpressions
            };

            var keyFieldName = pkField?.Name ?? "RowID";

            command.CommandText = builder.ToString() + $"; SELECT {keyFieldName} FROM { tableName} WHERE RowID = last_insert_rowid();";
        }

        public void BuildUpdate(IDbCommand command,
                                object data,
                                string tableName,
                                IFieldInfoCollection fields,
                                string whereExpression = null,
                                OnConflictOption option = OnConflictOption.Default,
                                object keyValue = null,
                                object extraPrams = null)
        {
            if (command is null) { throw new ArgumentNullException(nameof(command)); }
            if (data is null) { throw new ArgumentNullException(nameof(data)); }
            if (fields is null) { throw new ArgumentNullException(nameof(fields)); }
            if (string.IsNullOrEmpty(tableName)) { throw new ArgumentException("message", nameof(tableName)); }

            var pkField = fields.PrimaryKeyField ?? throw new InvalidOperationException($"Primary Key field required for Update command");

            if (keyValue is null)
            {
                keyValue = pkField.GetFieldValue(data);
            }
            if (keyValue is null)
            { throw new InvalidOperationException($"{pkField.Property.Name} should not be null"); }



            var columnNames = new List<string>();
            var columnExpressions = new List<string>();
            foreach (var field in fields)
            {
                object value = field.GetFieldValueOrDefault(data) ?? DBNull.Value;
                var param = MakeParameter(command, field, value);
                command.Parameters.Add(param);

                if ((field.PersistanceFlags & PersistanceFlags.OnUpdate) == PersistanceFlags.OnUpdate)
                {
                    columnNames.Add(field.Name);
                    columnExpressions.Add(param.ParameterName);
                }
            }

            if(extraPrams != null)
            {
                command.AddParams(extraPrams);
            }

            whereExpression = whereExpression ?? pkField.Name + " = " + GetParameterName(pkField);
            var where = new WhereClause(whereExpression);
            var expression = new SqlUpdateCommand()
            {
                TableName = tableName,
                ColumnNames = columnNames,
                ValueExpressions = columnExpressions,
                ConflictOption = option,
                Where = where
            };

            command.CommandText = expression.ToString();
        }

        public void BuildDelete(IDbCommand command, object data, string tableName, IFieldInfoCollection fields)
        {
            if (command is null) { throw new ArgumentNullException(nameof(command)); }
            if (data is null) { throw new ArgumentNullException(nameof(data)); }
            if (fields is null) { throw new ArgumentNullException(nameof(fields)); }
            if (string.IsNullOrEmpty(tableName)) { throw new ArgumentException("message", nameof(tableName)); }

            var keyFieldInfo = fields.PrimaryKeyField
                ?? throw new ORMException("type doesn't have primary key field");

            object keyValue = keyFieldInfo.GetFieldValue(data)
                ?? throw new ORMException("key value can not be null");

            var keyParam = MakeParameter(command, keyFieldInfo, keyValue);
            command.Parameters.Add(keyParam);

            command.CommandText = string.Format(@"DELETE FROM {0} WHERE {1} = {2};",
                tableName,
                keyFieldInfo.Name,
                keyParam.ParameterName);
        }
    }
}