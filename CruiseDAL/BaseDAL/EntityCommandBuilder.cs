using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;

namespace CruiseDAL.BaseDAL
{
    class EntityCommandBuilder
    {
        string _insertCommandFormatString;
        string _updateCommandFormat;

        public EntityDescription EntityDescription { get; set; }
        private DatastoreBase _dataStore;

        public EntityCommandBuilder(EntityDescription entDesc, DatastoreBase dataStore)
        {
            _dataStore = dataStore;
            EntityDescription = entDesc;
        }

        public DbCommand BuildInsertCommand(object data, OnConflictOption option)
        {
            string commandText = string.Format(GetInsertCommandFormatString(), option.ToString());
            DbCommand command = _dataStore.CreateCommand(commandText);
            foreach (EntityFieldInfo fi in EntityDescription.Fields.Values)
            {
                if (fi._fieldAttr.IsPersisted)
                {
                    object value = fi.GetFieldValue(data);
                    command.Parameters.Add(_dataStore.CreateParameter("@" + fi._fieldAttr.FieldName, value));
                }
                if (fi._fieldAttr.SpecialFieldType == SepcialFieldType.CreatedBy)
                {
                    command.Parameters.Add(_dataStore.CreateParameter("@" + fi._fieldAttr.FieldName, _dataStore.User));
                }
            }

            //if (usePresetRowID)
            //{
            //    command.Parameters.AddWithValue("@rowID", data.rowID);
            //}

            return command;
            
        }

        private string GetInsertCommandFormatString()
        {
            //check if we already have a cached insert command
            if (_insertCommandFormatString == null)
            {
                StringBuilder sb = new StringBuilder();
                //create first part of insert command, leaving placeholder for onConflect option
                sb.AppendFormat(null, "INSERT OR {{0}} INTO {0}( ", EntityDescription.SourceName);
                //build the column names section of the insert command 
                bool first = true;
                foreach (EntityFieldInfo fi in EntityDescription.Fields.Values)
                {
                    if (fi._fieldAttr == null) { continue; }
                    if (fi._fieldAttr.IsPersisted || fi._fieldAttr.SpecialFieldType == SepcialFieldType.CreatedBy)
                    {
                        if (!first) //if not first entry in list add comma befor entry
                        { sb.Append(','); }
                        else
                        { first = false; }
                        sb.AppendFormat(null, " {0}", fi._fieldAttr.FieldName);
                    }
                }

                sb.Append("{1}");//insert place holder so we can add rowID

                //build the values section of the insert command 
                sb.Append(" ) VALUES (");
                first = true;
                foreach (EntityFieldInfo fi in EntityDescription.Fields.Values)
                {
                    if (fi._fieldAttr == null) { continue; }
                    if (fi._fieldAttr.IsPersisted || fi._fieldAttr.SpecialFieldType == SepcialFieldType.CreatedBy)
                    {
                        if (!first) //if not first entry in list add comma befor entry
                        { sb.Append(','); }
                        else
                        { first = false; }
                        sb.AppendFormat(null, " @{0}", fi._fieldAttr.FieldName);
                    }
                }

                sb.Append(" {2}");//insert place holder for value of rowID
                sb.Append(");");

                //add command that will return the rowID of the row we just inserted 
                sb.Append("\r\nSELECT last_insert_rowid() AS id;");


                _insertCommandFormatString = sb.ToString();//cache the insert command 
            }
            return _insertCommandFormatString;
        }

        public DbCommand BuildUpdateCommand(object data, object recordKey, string ModifiedBy, OnConflictOption option)
        {
            string commandText = string.Format(GetUpdateCommandFormatString(), option.ToString());
            DbCommand command = _dataStore.CreateCommand(commandText);
            foreach (EntityFieldInfo fi in EntityDescription.Fields.Values)
            {
                if (fi._fieldAttr == null) { continue; }
                if (fi._fieldAttr.IsPersisted)
                {
                    object value = fi.GetFieldValue(data);
                    command.Parameters.Add(_dataStore.CreateParameter("@" + fi._fieldAttr.FieldName, value));
                }
                if (fi._fieldAttr.SpecialFieldType == SepcialFieldType.ModifiedBy)
                {
                    command.Parameters.Add(_dataStore.CreateParameter("@" + fi._fieldAttr.FieldName, ModifiedBy));
                }
            }

            command.Parameters.Add(_dataStore.CreateParameter("@rowID", recordKey));

            return command;
        }

        private string GetUpdateCommandFormatString()
        {
            if (_updateCommandFormat == null)
            {
                StringBuilder sb = new StringBuilder();
                //create first part of update command with place holder for OnConflictOption. 
                sb.AppendFormat(null, "UPDATE OR {0} {1} SET ", "{0}", EntityDescription.SourceName);

                String[] colExprs = new String[EntityDescription.Fields.Values.Count];
                int i = 0;
                foreach (EntityFieldInfo fi in EntityDescription.Fields.Values)
                {
                    if (fi._fieldAttr == null) { continue; }
                    if (fi._fieldAttr.IsPersisted || fi._fieldAttr.SpecialFieldType == SepcialFieldType.ModifiedBy)
                    {
                        colExprs[i] = string.Format(" {0} = @{0}", fi._fieldAttr.FieldName);
                        i++;
                    }
                }
                sb.Append(string.Join(", ", colExprs, 0, i));

                sb.Append("\r\n WHERE rowid = @rowID;");
                _updateCommandFormat = sb.ToString();
            }
            return _updateCommandFormat;
        }

        public SQLiteCommand CreateSQLDelete(DataObject data)
        {
            string query = string.Format(@"DELETE FROM {0} WHERE rowID = @rowID;
            DELETE FROM ErrorLog WHERE TableName = '{0}' AND CN_Number = @rowID;", this.TableName);

            SQLiteCommand command = new SQLiteCommand(query);
            command.Parameters.Add(new SQLiteParameter("@rowID", data.rowID));
            return command;
        }

    }
}
