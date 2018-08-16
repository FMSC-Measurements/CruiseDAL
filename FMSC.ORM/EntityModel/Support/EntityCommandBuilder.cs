using FMSC.ORM.Core;
using FMSC.ORM.Core.SQL;
using FMSC.ORM.EntityModel.Attributes;
using Backpack.SqlBuilder;
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
                //InitializeLegacySelectCommand();
            }
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

            //order fields by ordinal
            var fields = new List<FieldAttribute>(EntityDescription.Fields);
            fields.Sort(CompareFieldsByOrdinal);

            foreach (FieldAttribute field in fields)
            {
                selectBuilder.ResultColumns.Add(field.GetResultColumnExpression());
            }

            return selectBuilder;
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

        public void BuildInsertCommand(IDbCommand command, object data, object keyData, OnConflictOption option)
        {
            Debug.Assert(data != null);

            var columnNames = new List<string>();
            var valueExpressions = new List<string>();

            if (keyData != null)
            {
                var keyField = EntityDescription.Fields.PrimaryKeyField;
                if (keyField != null)
                {
                    columnNames.Add(keyField.Name);
                    valueExpressions.Add(keyField.SQLPramName);

                    var pram = command.CreateParameter();
                    pram.ParameterName = keyField.SQLPramName;
                    pram.Value = keyData;
                    command.Parameters.Add(pram);
                }
                else
                {
                    throw new InvalidOperationException("keyData provided but type has no key field");
                }
            }

            foreach (FieldAttribute field in EntityDescription.Fields.GetPersistedFields(false, PersistanceFlags.OnInsert))
            {
                columnNames.Add(field.Name);
                valueExpressions.Add(field.SQLPramName);

                object value = field.GetFieldValueOrDefault(data);

                var pram = command.CreateParameter();
                pram.ParameterName = field.SQLPramName;
                pram.Value = value ?? DBNull.Value;
                command.Parameters.Add(pram);
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

        public void BuildUpdateCommand(IDbCommand command, object data, object keyData, OnConflictOption option)
        {
            Debug.Assert(data != null);
            Debug.Assert(EntityDescription.Fields.PrimaryKeyField != null);

            var columnNames = new List<string>();
            var columnExpressions = new List<string>();
            foreach (FieldAttribute field in EntityDescription.Fields.GetPersistedFields(false, PersistanceFlags.OnUpdate))
            {
                columnNames.Add(field.Name);
                columnExpressions.Add(field.SQLPramName);

                object value = field.GetFieldValueOrDefault(data);

                var pram = command.CreateParameter();
                pram.ParameterName = field.SQLPramName;
                pram.Value = value ?? DBNull.Value;
                command.Parameters.Add(pram);
            }

            PrimaryKeyFieldAttribute keyField = EntityDescription.Fields.PrimaryKeyField;

            var p = command.CreateParameter();
            p.ParameterName = keyField.SQLPramName;
            p.Value = keyData;
            command.Parameters.Add(p);

            var where = new WhereClause(keyField.Name + " = " + keyField.SQLPramName);

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
            PrimaryKeyFieldAttribute keyFieldInfo = EntityDescription.Fields.PrimaryKeyField;

            if (keyFieldInfo == null) { throw new InvalidOperationException("type doesn't have primary key field"); }
            object keyValue = keyFieldInfo.GetFieldValue(data);

            BuildSQLDeleteCommand(command, keyFieldInfo.Name, keyValue);
        }

        protected void BuildSQLDeleteCommand(IDbCommand command, string keyFieldName, object keyValue)
        {
            Debug.Assert(keyValue != null);
            Debug.Assert(!string.IsNullOrEmpty(keyFieldName));

            string query = string.Format(@"DELETE FROM {0} WHERE {1} = @keyValue;", EntityDescription.SourceName, keyFieldName);
            command.CommandText = query;

            var param = command.CreateParameter();
            param.ParameterName = "@keyValue";
            param.Value = keyValue;
            command.Parameters.Add(param);
        }

        #endregion build delete

        /// <summary>
        /// compares fields by ordinal where -1 is always high
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected int CompareFieldsByOrdinal(FieldAttribute x, FieldAttribute y)
        {
            if (x.Ordinal == y.Ordinal) { return 0; }
            if (x.Ordinal == -1) { return 1; }
            if (y.Ordinal == -1) { return -1; }
            if (x.Ordinal > y.Ordinal) { return 1; }
            else { return -1; }
        }
    }
}