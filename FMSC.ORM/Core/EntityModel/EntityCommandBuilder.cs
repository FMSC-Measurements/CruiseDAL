﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;

using FMSC.ORM.Core.SQL;
using FMSC.ORM.Core.EntityAttributes;
using System.Text;

#if NetCF
using FMSC.ORM.NetCF;
#endif

namespace FMSC.ORM.Core.EntityModel
{
    public class EntityCommandBuilder
    {
        SQLSelectExpression _selectCommand;
        string _selectCommandFormat;

        SQLInsertCommand _insertCommand;
        //SQLSelectExpression _afterInsertCommand;

        SQLUpdateCommand _updateCommand; 

        //string _insertCommandFormatString;
        //string _updateCommandFormat;

        public EntityDescription EntityDescription { get; set; }
        private DbProviderFactoryAdapter _providerFactory;

        public EntityCommandBuilder(EntityDescription entDesc, DbProviderFactoryAdapter providerFactory)
        {
            _providerFactory = providerFactory;
            EntityDescription = entDesc;

            InitializeSelectCommand();

            if (EntityDescription.Fields.PrimaryKeyField != null)
            {
                InitializeInsertCommand();
                InitializeUpdateCommand();
            }
        }


        #region build select 
        public DbCommand BuildSelectCommand(WhereClause where)
        {
            Debug.Assert(_selectCommand != null);

            this._selectCommand.Where = where;
            string query = _selectCommand.ToString();

            DbCommand command = _providerFactory.CreateCommand(query);
            return command;

        }

        protected void InitializeSelectCommand()
        {
            SQLSelectExpression expression = new SQLSelectExpression();
            expression.TableOrSubQuery = EntityDescription.SourceName;

            //order fields by ordinal
            List<FieldAttribute> fields = new List<FieldAttribute>(EntityDescription.Fields);
            fields.Sort(CompareFieldsByOrdinal);

            List<string> columnExpressions = new List<string>();

            foreach (FieldAttribute fi in fields)
            {
                String colExpression = null;

                if (!string.IsNullOrEmpty(fi.SQLExpression))
                {
                    colExpression = fi.SQLExpression + " AS " + fi.FieldName;
                }
                else
                {
                    colExpression = fi.FieldName;
                }

                columnExpressions.Add(colExpression);
            }
            expression.ResultColumns = columnExpressions;
            _selectCommand = expression;
        }

        public DbCommand BuildSelectLegacy(string selection)
        {
            Debug.Assert(_selectCommandFormat != null);

            string commandText = string.Format(_selectCommandFormat, selection);

            DbCommand command = _providerFactory.CreateCommand(commandText);
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
                    if (!string.IsNullOrEmpty(field.SQLExpression))
                    {
                        if (first) { sb.Append(", "); }
                        sb.AppendLine(field.SQLExpression + " AS " + field.FieldName);
                    }
                    else
                    {
                        if (first) { sb.Append(", "); }
                        sb.AppendLine(field.FieldName);
                    }
                }

                sb.AppendLine("FROM " + EntityDescription.SourceName);
                sb.AppendLine("{0};");//insert placeholder and close out command

                _selectCommandFormat = sb.ToString();
            }
        }


        #endregion

        #region build insert
        public DbCommand BuildInsertCommand(object data, SQL.OnConflictOption option)
        {
            Debug.Assert(data != null);
            Debug.Assert(_insertCommand != null);

            _insertCommand.ConflictOption = option;

            DbCommand command = _providerFactory.CreateCommand(_insertCommand.ToString());

            foreach(FieldAttribute field in EntityDescription.Fields.GetPersistedFields())
            {
                if (field is InfrastructureFieldAttribute
                    && (((InfrastructureFieldAttribute)field).PersistMode & PersistMode.OnInsert) != PersistMode.OnInsert)
                {
                    continue;
                }

                object value = field.GetFieldValueOrDefault(data);

                DbParameter pram = _providerFactory.CreateParameter(field.SQLPramName, value);
                command.Parameters.Add(pram);
            }

            return command;


            //string commandText = string.Format(GetInsertCommandFormatString(), option.ToString());
            //DbCommand command = _dataStore.CreateCommand(commandText);
            //foreach (EntityFieldInfo fi in EntityDescription.Fields.Values)
            //{
            //    if (fi.IsPersisted)
            //    {
            //        object value = fi.GetFieldValue(data);
            //        command.Parameters.Add(_dataStore.CreateParameter("@" + fi._fieldAttr.FieldName, value));
            //    }
            //    if (fi._fieldAttr.SpecialFieldType == SepcialFieldType.CreatedBy)
            //    {
            //        command.Parameters.Add(_dataStore.CreateParameter("@" + fi._fieldAttr.FieldName, _dataStore.User));
            //    }
            //}

            ////if (usePresetRowID)
            ////{
            ////    command.Parameters.AddWithValue("@rowID", data.rowID);
            ////}

            //return command;
            
        }

        protected void InitializeInsertCommand()
        {
            SQLInsertCommand expression = new SQLInsertCommand();
            expression.TableName = EntityDescription.SourceName;

            List<String> columnNames = new List<string>();
            List<String> valueExpressions = new List<string>();
            foreach (FieldAttribute field in EntityDescription.Fields.GetPersistedFields())
            {
                //if field is not persisted on insert populated skip
                if (field is InfrastructureFieldAttribute
                    && (((InfrastructureFieldAttribute)field).PersistMode & PersistMode.OnInsert) != PersistMode.OnInsert)
                {
                    continue;
                }

                columnNames.Add(field.FieldName);
                valueExpressions.Add(field.SQLPramName);
            }

            expression.ColumnNames = columnNames;
            expression.ValueExpressions = valueExpressions;

            _insertCommand = expression;
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
        public DbCommand BuildUpdateCommand(object data, SQL.OnConflictOption option)
        {
            Debug.Assert(data != null);
            Debug.Assert(_updateCommand != null);


            _updateCommand.ConflictOption = option;

            DbCommand command = _providerFactory.CreateCommand(_updateCommand.ToString());

            foreach (FieldAttribute field in EntityDescription.Fields.GetPersistedFields())
            {
                if (field is InfrastructureFieldAttribute
                    && (((InfrastructureFieldAttribute)field).PersistMode & PersistMode.OnUpdate) != PersistMode.OnUpdate)
                {
                    continue;
                }

                object value = field.GetFieldValueOrDefault(data);

                DbParameter pram = _providerFactory.CreateParameter(field.SQLPramName, value);
                command.Parameters.Add(pram);
            }

            PrimaryKeyFieldAttribute keyField = EntityDescription.Fields.PrimaryKeyField;
            object keyValue = keyField.GetFieldValueOrDefault(data);
            DbParameter p = _providerFactory.CreateParameter(keyField.SQLPramName, keyField);
            command.Parameters.Add(p);


            //string commandText = string.Format(GetUpdateCommandFormatString(), option.ToString());
            //DbCommand command = _dataStore.CreateCommand(commandText);
            //foreach (EntityFieldInfo fi in EntityDescription.Fields.Values)
            //{
            //    if (fi._fieldAttr.IsPersisted)
            //    {
            //        object value = fi.GetFieldValue(data);
            //        command.Parameters.Add(_dataStore.CreateParameter("@" + fi._fieldAttr.FieldName, value));
            //    }
            //    //if (fi._fieldAttr.SpecialFieldType == SepcialFieldType.ModifiedBy)
            //    //{
            //    //    command.Parameters.Add(_dataStore.CreateParameter("@" + fi._fieldAttr.FieldName, ModifiedBy));
            //    //}
            //}

            //command.Parameters.Add(_dataStore.CreateParameter("@rowID", recordKey));

            return command;
        }

        protected void InitializeUpdateCommand()
        {
            SQLUpdateCommand expression = new SQLUpdateCommand();
            expression.TableName = EntityDescription.SourceName;

            List<string> columnNames = new List<string>();
            List<string> columnExpressions = new List<string>();


            foreach (FieldAttribute field in EntityDescription.Fields.GetPersistedFields())
            {
                if (field is InfrastructureFieldAttribute
                    && (((InfrastructureFieldAttribute)field).PersistMode & PersistMode.OnUpdate) != PersistMode.OnUpdate)
                {
                    continue;
                }

                columnNames.Add(field.FieldName);
                columnExpressions.Add(field.SQLPramName);
            }

            expression.ColumnNames = columnNames;
            expression.ValueExpressions = columnExpressions;

            PrimaryKeyFieldAttribute keyField = EntityDescription.Fields.PrimaryKeyField;
            WhereClause where = new WhereClause(keyField.FieldName + " = " + keyField.SQLPramName);
            expression.Where = where;

            _updateCommand = expression;
        }

        //private string GetUpdateCommandFormatString()
        //{
        //    if (_updateCommandFormat == null)
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        //create first part of update command with place holder for OnConflictOption. 
        //        sb.AppendFormat(null, "UPDATE OR {0} {1} SET ", "{0}", EntityDescription.SourceName);

        //        String[] colExprs = new String[EntityDescription.Fields.Values.Count];
        //        int i = 0;
        //        foreach (EntityFieldInfo fi in EntityDescription.Fields.Values)
        //        {
        //            if (fi.IsPersisted == false) { continue; }

        //            colExprs[i] = string.Format(" {0} = @{0}", fi._fieldAttr.FieldName);
        //            i++;

        //        }
        //        sb.Append(string.Join(", ", colExprs, 0, i));

        //        sb.Append("\r\n WHERE rowid = @rowID;");
        //        _updateCommandFormat = sb.ToString();
        //    }
        //    return _updateCommandFormat;
        //}
        #endregion

        #region build delete
        //protected DbCommand GetSQLDeleteCommand()
        //{
        //    string query = string.Format(@"DELETE FROM {0} WHERE rowID = @rowID;
        //    DELETE FROM ErrorLog WHERE TableName = '{0}' AND CN_Number = @rowID;", EntityDescription.SourceName);
        //    //TODO remove dependency on ErrorLogTable

        //    //string query = string.Format(@"DELETE FROM {0} WHERE rowID = @rowID;", EntityDescription.SourceName);

        //    return _providerFactory.CreateCommand(query);
        //}

        //public DbCommand BuildSQLDeleteCommand(object data)
        //{
        //    var keyValue = EntityDescription.Fields.PrimaryKeyField.GetFieldValue(data);
        //    var command = GetSQLDeleteCommand();

        //    command.Parameters.Clear();
        //    command.Parameters.Add(_providerFactory.CreateParameter("@rowID", keyValue));
        //    return command;
        //}

        protected DbCommand GetSQLDeleteCommand(string keyFieldName)
        {
            string query = string.Format(@"DELETE FROM {0} WHERE {1} = @keyValue;", EntityDescription.SourceName, keyFieldName);

            return _providerFactory.CreateCommand(query);
        }

        public DbCommand BuildSQLDeleteCommand(string fieldName, object keyValue)
        {
            Debug.Assert(keyValue != null);
            Debug.Assert(!string.IsNullOrEmpty(fieldName));

            var command = GetSQLDeleteCommand(fieldName);

            command.Parameters.Clear();
            command.Parameters.Add(_providerFactory.CreateParameter("@keyValue", keyValue));
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