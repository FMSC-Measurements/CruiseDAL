using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;

using FMSC.ORM.Core.SQL;
using FMSC.ORM.Core.EntityAttributes;
using System.Text;


namespace FMSC.ORM.Core.EntityModel
{
    public class EntityCommandBuilder
    {
        SQLSelectBuilder _selectCommand;
        string _selectCommandFormat;


        public EntityDescription EntityDescription { get; set; }

        public EntityCommandBuilder(EntityDescription entDesc)
        {
            EntityDescription = entDesc;

            _selectCommand = MakeSelectCommand();
            InitializeLegacySelectCommand();

        }


        #region build select 
        public DbCommand BuildSelectCommand(DbProviderFactoryAdapter provider, SelectClause clause)
        {
            Debug.Assert(_selectCommand != null);

            this._selectCommand.Clause = clause;

            string query = _selectCommand.ToSQL();

            DbCommand command = provider.CreateCommand(query);
            return command;

        }

        public SQLSelectBuilder MakeSelectCommand()
        {
            SQLSelectBuilder selectBuilder = new SQLSelectBuilder();
            selectBuilder.Source = EntityDescription.Source;

            //order fields by ordinal
            List<FieldAttribute> fields = new List<FieldAttribute>(EntityDescription.Fields);
            fields.Sort(CompareFieldsByOrdinal);

            foreach(FieldAttribute field in fields)
            {
                selectBuilder.ResultColumns.Add(field.GetResultColumnExpression(EntityDescription.SourceName));
            }

            return selectBuilder;
        }

        public DbCommand BuildSelectLegacy(DbProviderFactoryAdapter provider, string selection)
        {
            Debug.Assert(_selectCommandFormat != null);

            string commandText = string.Format(_selectCommandFormat, selection);

            DbCommand command = provider.CreateCommand(commandText);
            return command;
        }

        protected void InitializeLegacySelectCommand()
        {
            if (_selectCommandFormat == null)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("SELECT");

                bool first = true;
                foreach (FieldAttribute field in EntityDescription.Fields)
                {
                    string colExpr; 

                    if(string.IsNullOrEmpty(field.SQLExpression))
                    {
                        colExpr = EntityDescription.SourceName + "." + field.FieldName;
                    }
                    else
                    {
                        colExpr = field.SQLExpression + " AS " + EntityDescription.SourceName + "." + field.FieldName;
                    }

                    if (!first)
                    {
#if NetCF
                        sb.Append(",\r\n");
#else
                        sb.Append("," + Environment.NewLine);
#endif
                    }
                    else
                    {
                        first = false;
                    }
                    sb.Append(" " + colExpr);

                }

                sb.AppendLine(" FROM " + EntityDescription.SourceName);
                sb.AppendLine(" {0};");//insert placeholder and close out command

                _selectCommandFormat = sb.ToString();
            }
        }


        #endregion

        #region build insert
        public DbCommand BuildInsertCommand(DbProviderFactoryAdapter provider, object data, SQL.OnConflictOption option)
        {
            Debug.Assert(data != null);

            DbCommand command = provider.CreateCommand();

            List<String> columnNames = new List<string>();
            List<String> valueExpressions = new List<string>();

            var keyField = EntityDescription.Fields.PrimaryKeyField;
            if (keyField != null)
            {
                var keyValue = keyField.GetFieldValueOrDefault(data);

                if (keyValue != null)
                {
                    columnNames.Add(keyField.FieldName);
                    valueExpressions.Add(keyField.SQLPramName);

                    var pram = provider.CreateParameter(keyField.SQLPramName, keyValue);
                    command.Parameters.Add(pram);
                }
            }
                 
            foreach (FieldAttribute field in EntityDescription.Fields.GetPersistedFields(false, PersistanceFlags.OnInsert))
            {
                columnNames.Add(field.FieldName);
                valueExpressions.Add(field.SQLPramName);

                object value = field.GetFieldValueOrDefault(data);
                DbParameter pram = provider.CreateParameter(field.SQLPramName, value);
                command.Parameters.Add(pram);
            }
            
            SQLInsertCommand builder = new SQLInsertCommand()
            {
                TableName = EntityDescription.SourceName,
                ConflictOption = option,
                ColumnNames = columnNames,
                ValueExpressions = valueExpressions
            };

            command.CommandText = builder.ToString();

            return command;
        }


        //private string GetInsertCommandFormatString()
        //{
        //    //check if we already have a cached insert command
        //    if (_insertCommandFormatString == null)
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        //create first part of insert command, leaving placeholder for onConflect option
        //        sb.AppendFormat(null, "INSERT OR {{0}} INTO {0}( ", EntityDescription.SourceName);
        //        //build the column names section of the insert command 
        //        bool first = true;
        //        foreach (EntityFieldInfo fi in EntityDescription.Fields.Values)
        //        {
        //            if (fi._fieldAttr == null) { continue; }
        //            if (fi._fieldAttr.IsPersisted || fi._fieldAttr.SpecialFieldType == SepcialFieldType.CreatedBy)
        //            {
        //                if (!first) //if not first entry in list add comma befor entry
        //                { sb.Append(','); }
        //                else
        //                { first = false; }
        //                sb.AppendFormat(null, " {0}", fi._fieldAttr.FieldName);
        //            }
        //        }

        //        sb.Append("{1}");//insert place holder so we can add rowID

        //        //build the values section of the insert command 
        //        sb.Append(" ) VALUES (");
        //        first = true;
        //        foreach (EntityFieldInfo fi in EntityDescription.Fields.Values)
        //        {
        //            if (fi._fieldAttr == null) { continue; }
        //            if (fi._fieldAttr.IsPersisted || fi._fieldAttr.SpecialFieldType == SepcialFieldType.CreatedBy)
        //            {
        //                if (!first) //if not first entry in list add comma befor entry
        //                { sb.Append(','); }
        //                else
        //                { first = false; }
        //                sb.AppendFormat(null, " @{0}", fi._fieldAttr.FieldName);
        //            }
        //        }

        //        sb.Append(" {2}");//insert place holder for value of rowID
        //        sb.Append(");");

        //        //add command that will return the rowID of the row we just inserted 
        //        sb.Append("\r\nSELECT last_insert_rowid() AS id;");


        //        _insertCommandFormatString = sb.ToString();//cache the insert command 
        //    }
        //    return _insertCommandFormatString;
        //}
        #endregion

        #region build update
        public DbCommand BuildUpdateCommand(DbProviderFactoryAdapter provider, object data, SQL.OnConflictOption option)
        {
            Debug.Assert(data != null);
            Debug.Assert(EntityDescription.Fields.PrimaryKeyField != null);

            DbCommand command = provider.CreateCommand();


            List<string> columnNames = new List<string>();
            List<string> columnExpressions = new List<string>();
            foreach (FieldAttribute field in EntityDescription.Fields.GetPersistedFields(true, PersistanceFlags.OnUpdate))
            {
                columnNames.Add(field.FieldName);
                columnExpressions.Add(field.SQLPramName);

                object value = field.GetFieldValueOrDefault(data);
                DbParameter pram = provider.CreateParameter(field.SQLPramName, value);
                command.Parameters.Add(pram);
            }

            PrimaryKeyFieldAttribute keyField = EntityDescription.Fields.PrimaryKeyField;
            object keyValue = keyField.GetFieldValueOrDefault(data);
            DbParameter p = provider.CreateParameter(keyField.SQLPramName, keyValue);
            command.Parameters.Add(p);

            WhereClause where = new WhereClause(keyField.FieldName + " = " + keyField.SQLPramName);

            SQLUpdateCommand expression = new SQLUpdateCommand()
            {
                TableName = EntityDescription.SourceName,
                ColumnNames = columnNames,
                ValueExpressions = columnExpressions,
                ConflictOption = option,
                Where = where
            };

            command.CommandText = expression.ToString();

            return command;
        }

        #endregion

        #region build delete

        public DbCommand BuildSQLDeleteCommand(DbProviderFactoryAdapter provider, object data)
        {
            PrimaryKeyFieldAttribute keyFieldInfo = EntityDescription.Fields.PrimaryKeyField;

            if (keyFieldInfo == null) { throw new InvalidOperationException("type doesn't have primary key field"); }
            object keyValue = keyFieldInfo.GetFieldValue(data);

            return this.BuildSQLDeleteCommand(provider, keyFieldInfo.FieldName, keyValue);
        }

        protected DbCommand BuildSQLDeleteCommand(DbProviderFactoryAdapter provider, string keyFieldName, object keyValue)
        {
            Debug.Assert(keyValue != null);
            Debug.Assert(!string.IsNullOrEmpty(keyFieldName));

            string query = string.Format(@"DELETE FROM {0} WHERE {1} = @keyValue;", EntityDescription.SourceName, keyFieldName);

            var command = provider.CreateCommand(query);

            command.Parameters.Clear();
            command.Parameters.Add(provider.CreateParameter("@keyValue", keyValue));
            return command;
        }
        #endregion

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
