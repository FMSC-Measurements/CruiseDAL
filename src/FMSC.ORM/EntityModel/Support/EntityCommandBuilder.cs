using Backpack.SqlBuilder;
using FMSC.ORM.Core.SQL;
using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace FMSC.ORM.EntityModel.Support
{
    public class EntityCommandBuilder
    {
        private SqlSelectBuilder _legacySelectBuilder;

        public EntityDescription EntityDescription { get; set; }

        public EntityCommandBuilder(EntityDescription entDesc)
        {
            EntityDescription = entDesc;

            if (EntityDescription.Source != null)
            {
                _legacySelectBuilder = MakeSelectCommand(null);
            }
        }

        protected IDbDataParameter MakeParameter(FieldInfo field, object value, IDbCommand command)
        {
            var param = command.CreateParameter();
            param.ParameterName = "@" + field.Name.ToLower();
            param.Value = value;
            return param;
        }

        #region build select

        //public DbCommand BuildSelectCommand(DbProviderFactoryAdapter provider, SelectClause clause)
        //{
        //    Debug.Assert(_selectCommand != null);

        //    this._selectCommand.Clause = clause;

        //    string query = _selectCommand.ToSQL();

        //    DbCommand command = provider.CreateCommand(query);
        //    return command;

        //}

        public SqlSelectBuilder MakeSelectCommand(TableOrSubQuery source)
        {
            var selectBuilder = new SqlSelectBuilder();
            selectBuilder.Source = source ?? EntityDescription.Source;

            foreach (var field in EntityDescription.Fields)
            {
                var resultColExpr = MakeResultColumnExpression(field);
                selectBuilder.ResultColumns.Add(resultColExpr);
            }

            return selectBuilder;
        }

        protected string MakeResultColumnExpression(FieldInfo field)
        {
            var sqlExpression = field.SQLExpression;
            var alias = field.Alias;
            if (!string.IsNullOrEmpty(sqlExpression))
            {
                System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(alias));
                return sqlExpression + " AS " + alias;
            }
            else if (alias != null)
            {
                return alias;
            }
            else
            {
                var sourceName = EntityDescription.SourceName;
                if (string.IsNullOrEmpty(sourceName)) { return field.Name; }
                else { return sourceName + "." + field.Name; }
            }
        }

        public string BuildSelectLegacy(string selection)
        {
            Debug.Assert(_legacySelectBuilder != null);

            this._legacySelectBuilder.WhereClause = new LegacyWhereClausePlaceholder() { Index = 0 };

            return String.Format(_legacySelectBuilder.ToString(), selection);
        }

        //protected void InitializeLegacySelectCommand()
        //{
        //    if (_selectCommandFormat == null)
        //    {
        //        var sb = new StringBuilder();
        //        sb.AppendLine("SELECT");

        //        bool first = true;
        //        foreach (FieldAttribute field in EntityDescription.Fields)
        //        {
        //            string colExpr = field.GetResultColumnExpression();

        //            if (!first)
        //            {
        //                sb.AppendLine("," + Environment.NewLine);
        //            }
        //            else
        //            {
        //                first = false;
        //            }
        //            sb.Append(" " + colExpr);

        //        }

        //        sb.AppendLine(" FROM " + EntityDescription.SourceName);
        //        sb.AppendLine(" {0};");//insert placeholder and close out command

        //        _selectCommandFormat = sb.ToString();
        //    }
        //}

        #endregion build select

        #region build insert

        public void BuildInsertCommand(IDbCommand command, object data, object keyValue, OnConflictOption option)
        {
            Debug.Assert(data != null);

            var columnNames = new List<string>();
            var valueExpressions = new List<string>();

            if (keyValue != null)
            {
                var keyField = EntityDescription.Fields.PrimaryKeyField;
                if (keyField != null)
                {
                    var param = MakeParameter(keyField, keyValue, command);
                    command.Parameters.Add(param);

                    columnNames.Add(keyField.Name);
                    valueExpressions.Add(param.ParameterName);
                }
                else
                {
                    throw new InvalidOperationException("keyData provided but type has no key field");
                }
            }

            foreach (var field in EntityDescription.Fields.GetPersistedFields(false, PersistanceFlags.OnInsert))
            {
                object value = field.GetFieldValueOrDefault(data) ?? DBNull.Value;
                var param = MakeParameter(field, value, command);
                command.Parameters.Add(param);

                columnNames.Add(field.Name);
                valueExpressions.Add(param.ParameterName);
            }

            var builder = new SqlInsertCommand
            {
                TableName = EntityDescription.SourceName,
                ConflictOption = option,
                ColumnNames = columnNames,
                ValueExpressions = valueExpressions
            };

            command.CommandText = builder.ToString();
        }

        #endregion build insert

        #region build update

        public void BuildUpdateCommand(IDbCommand command, object data, object keyValue, OnConflictOption option)
        {
            Debug.Assert(data != null);
            Debug.Assert(EntityDescription.Fields.PrimaryKeyField != null);

            var columnNames = new List<string>();
            var columnExpressions = new List<string>();
            foreach (var field in EntityDescription.Fields.GetPersistedFields(false, PersistanceFlags.OnUpdate))
            {
                object value = field.GetFieldValueOrDefault(data) ?? DBNull.Value;
                var param = MakeParameter(field, value, command);
                command.Parameters.Add(param);

                columnNames.Add(field.Name);
                columnExpressions.Add(param.ParameterName);
            }

            var keyField = EntityDescription.Fields.PrimaryKeyField;
            var param2 = MakeParameter(keyField, keyValue, command);
            command.Parameters.Add(param2);

            var where = new WhereClause(keyField.Name + " = " + param2.ParameterName);

            var expression = new SqlUpdateCommand()
            {
                TableName = EntityDescription.SourceName,
                ColumnNames = columnNames,
                ValueExpressions = columnExpressions,
                ConflictOption = option,
                Where = where
            };

            command.CommandText = expression.ToString();
        }

        #endregion build update

        #region build delete

        public void BuildSQLDeleteCommand(IDbCommand command, object data)
        {
            var keyFieldInfo = EntityDescription.Fields.PrimaryKeyField
                ?? throw new ORMException("type doesn't have primary key field");

            object keyValue = keyFieldInfo.GetFieldValue(data)
                ?? throw new ORMException("key value can not be null");

            var keyParam = MakeParameter(keyFieldInfo, keyValue, command);
            command.Parameters.Add(keyParam);

            command.CommandText = string.Format(@"DELETE FROM {0} WHERE {1} = {2};",
                EntityDescription.SourceName,
                keyFieldInfo.Name,
                keyParam.ParameterName);
        }

        #endregion build delete
    }
}