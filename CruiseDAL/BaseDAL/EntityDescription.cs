using System;
using System.Reflection;
using System.Collections.Generic;

using System.Text;
using System.Data;
using System.Diagnostics;
using CruiseDAL.BaseDAL.EntityAttributes;

#if ANDROID
using Mono.Data.Sqlite;
using SQLiteCommand = Mono.Data.Sqlite.SqliteCommand;
using SQLiteParameter = Mono.Data.Sqlite.SqliteParameter;
#else
using System.Data.SQLite;
#endif


namespace CruiseDAL.BaseDAL
{
    public class EntityDescription
    {
        private SQLEntityAttribute _entityAttr;

        public Type EntityType { get; private set; }
        public String SourceName
        {
            get { return _entityAttr.SourceName; }
        }

        public Dictionary<String, EntityFieldInfo> Fields { get; protected set; }
        public EntityFieldInfo KeyField { get; protected set; }

        public Dictionary<String, > References { get; set; }


        protected EntityDescription()
        {
            Fields = new Dictionary<string, EntityFieldInfo>(StringComparer.Create(System.Globalization.CultureInfo.CurrentCulture, true));
        }


        public EntityDescription(Type type) : this()
        {
            try
            {
                EntityType = type;


                object[] tAttrs = type.GetCustomAttributes(typeof(SQLEntityAttribute), true);
                _entityAttr = (SQLEntityAttribute)tAttrs[0];


                foreach (PropertyInfo p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    RegesterFieldInfo(p);
                }

                foreach (PropertyInfo p in type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    RegesterFieldInfo(p);
                }

            }
            catch (Exception e)
            {
                throw new ORMException("Unable to create EntityDescription for " + type.Name, e);
            }
        }


        

        private void RegesterFieldInfo(PropertyInfo pi)
        {
            FieldAttribute fa = (FieldAttribute)Attribute.GetCustomAttribute(pi, typeof(FieldAttribute));
            EntityFieldInfo fi;
            if (fa != null)
            {
                fi = new EntityFieldInfo(pi, fa);
            }
            else
            {
                fi = new EntityFieldInfo(pi);
            }

            if (fa is PrimaryKeyFieldAttribute)
            {
                this.KeyField = fi;
            }
            else
            {
                Fields.Add(pi.Name, fi);
            }

            
        }

        
        


        internal string GetSelectCommandFormat()
        {
            if (_selectCommandFormat == null)
            {
                StringBuilder sb = new StringBuilder("SELECT ");

                bool first = true;
                foreach (EntityFieldInfo fi in Fields.Values)
                {
                    if (fi._fieldAttr == null) { continue; }
                    String colExpression = null;

                    if (!string.IsNullOrEmpty(fi._fieldAttr.MapExpression))
                    {
                        colExpression = fi._fieldAttr.MapExpression;
                    }
                    else if (!string.IsNullOrEmpty(fi.FieldName))
                    {
                        colExpression = ((String.IsNullOrEmpty(SourceName)) ? String.Empty : SourceName + ".") + fi._fieldAttr.FieldName;
                    }

                    if (!string.IsNullOrEmpty(fi._fieldAttr.Alias) && !string.IsNullOrEmpty(colExpression))
                    {
                        sb.AppendFormat(null, " {0} AS {1},", colExpression, fi._fieldAttr.Alias);
                    }
                    else if (!string.IsNullOrEmpty(fi._fieldAttr.Alias))
                    {
                        sb.Append(fi._fieldAttr.Alias + ",");
                    }
                    else if (!string.IsNullOrEmpty(colExpression))
                    {
                        if (first) { first = false; }
                        else { sb.Append(","); }
                        sb.Append(colExpression);
                    }

                }
                if (_rowidField != null)
                {
                    sb.AppendFormat(null, ", {0}.rowID as tableRowID FROM {0}{1}{2};", ReadSource, JoinCommand, "{0}");
                }
                else
                {
                    sb.AppendFormat(null, "  FROM {0}{1}{2};", ReadSource, JoinCommand, "{0}");
                }
                _selectCommandFormat = sb.ToString();
            }

            return _selectCommandFormat;
        }

        private string GetInsertCommandFormatString()
        {
            //check if we already have a cached insert command
            if(_insertCommandFormatString == null)
            {
                StringBuilder sb = new StringBuilder();
                //create first part of insert command, leaving placeholder for onConflect option
                sb.AppendFormat(null,"INSERT OR {{0}} INTO {0}( ",this.SourceName);                
                //build the column names section of the insert command 
                bool first = true;
                foreach(EntityFieldInfo fi in Fields.Values)
                {
                    if (fi._fieldAttr == null) { continue; }
                    if(fi._fieldAttr.IsPersisted || fi._fieldAttr.SpecialFieldType == SepcialFieldType.CreatedBy)
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
                foreach(EntityFieldInfo fi in Fields.Values)
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

        public SQLiteCommand CreateSQLInsert(DataObject data, string createdBy,bool usePresetRowID, OnConflictOption option)
        {

            string commandText = string.Format(GetInsertCommandFormatString(), option.ToString(),
                (usePresetRowID) ? ", rowID" : String.Empty,
                (usePresetRowID) ? ", @rowID" : String.Empty);
            SQLiteCommand command = new SQLiteCommand(commandText);
            foreach(FieldInfo fi in Fields.Values)
            {
                if (fi._fieldAttr == null) { continue; }
                if(fi._fieldAttr.IsPersisted )
                {
                    object value = this.GetPropertyValue(fi, data);
                    command.Parameters.Add(new SQLiteParameter("@" + fi._fieldAttr.FieldName, value));
                }
                if (fi._fieldAttr.SpecialFieldType == SepcialFieldType.CreatedBy)
                {
                    command.Parameters.Add(new SQLiteParameter("@" + fi._fieldAttr.FieldName, createdBy));
                }
            }

            if (usePresetRowID)
            {
                command.Parameters.AddWithValue("@rowID", data.rowID);
            }
            
            return command;
        }

        private string GetUpdateCommandFormatString()
        {
            if(_updateCommandFormat == null)
            {
                StringBuilder sb = new StringBuilder();
                //create first part of update command with place holder for OnConflictOption. 
                sb.AppendFormat(null,"UPDATE OR {0} {1} SET ", "{0}", this.SourceName);

                String[] colExprs = new String[Fields.Values.Count];
                int i = 0;
                foreach(FieldInfo fi in Fields.Values)
                {
                    if (fi._fieldAttr == null) { continue; }
                    if(fi._fieldAttr.IsPersisted || fi._fieldAttr.SpecialFieldType == SepcialFieldType.ModifiedBy)
                    {
                        colExprs[i] = string.Format(" {0} = @{0}", fi._fieldAttr.FieldName);
                        i++;
                    }
                }
                sb.Append(string.Join(", ", colExprs,0, i));

                sb.Append("\r\n WHERE rowid = @rowID;");
                _updateCommandFormat = sb.ToString();
            }
            return _updateCommandFormat;
        }

        public SQLiteCommand CreateSQLUpdate(DataObject data, object recordKey, string ModifiedBy, OnConflictOption option)
        {            
            string commandText = string.Format(GetUpdateCommandFormatString(), option.ToString());
            SQLiteCommand command = new SQLiteCommand(commandText);
            foreach(FieldInfo fi in Fields.Values)
            {
                if (fi._fieldAttr == null) { continue; }
                if (fi._fieldAttr.IsPersisted )
                {
                    object value = this.GetPropertyValue(fi, data);
                    command.Parameters.Add(new SQLiteParameter("@" + fi._fieldAttr.FieldName, value));
                }
                if (fi._fieldAttr.SpecialFieldType == SepcialFieldType.ModifiedBy)
                {
                    command.Parameters.Add(new SQLiteParameter("@" + fi._fieldAttr.FieldName, ModifiedBy));
                }
            }

            command.Parameters.Add(new SQLiteParameter("@rowID", recordKey));

            return command;
        }

        public SQLiteCommand CreateSQLDelete(DataObject data)
		{
		    string query = string.Format(@"DELETE FROM {0} WHERE rowID = @rowID;
            DELETE FROM ErrorLog WHERE TableName = '{0}' AND CN_Number = @rowID;", this.SourceName);
		
		    SQLiteCommand command = new SQLiteCommand(query);
		    command.Parameters.Add(new SQLiteParameter("@rowID", data.rowID));
		    return command;
		}

    }

        
    

}