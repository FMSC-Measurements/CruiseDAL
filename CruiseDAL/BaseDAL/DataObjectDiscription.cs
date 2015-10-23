using System;
using System.Reflection;
using System.Collections.Generic;

using System.Text;
using System.Data;
using System.Diagnostics;

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
        protected EntityDescription()
        {
            Properties = new Dictionary<string, EntityFieldInfo>(StringComparer.Create(System.Globalization.CultureInfo.CurrentCulture, true));
        }


        public EntityDescription(Type type)
        {
            try
            {
                EntityType = type;
                _typeID = type.GetHashCode();
                //_typeID = _typeID | _typeID << 32;

                object[] tAttrs = type.GetCustomAttributes(typeof(SQLEntityAttribute), true);
                if (tAttrs.Length > 0)
                {
                    _entityAttr = (SQLEntityAttribute)tAttrs[0];
                    this.IsCached = _entityAttr.IsCached;
                    this.ReadSource = _entityAttr.ReadSource;
                    this.TableName = _entityAttr.TableName;
                    this.JoinCommand = " " + _entityAttr.JoinCommand + " ";
                    _readFromView = _entityAttr.ReadSource != _entityAttr.TableName;
                }



                foreach (PropertyInfo p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    AddProperty(p);
                }

                foreach (PropertyInfo p in type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    AddProperty(p);
                }

                PropertyInfo rowIDProp = type.GetProperty("rowID");
                if (rowIDProp != null)
                {
                    AddProperty(rowIDProp);
                }
            }
            catch (Exception e)
            {
                throw new ORMException("Unable to create EntityDescription for " + type.Name, e);
            }
        }

        

        const int BYTE_READ_LENGTH = 1024;//TODO onece we start reading byte data figure out our byte read length... should this be done at runtime?

        public Type EntityType { get; private set; }
        private int _typeID;
        private SQLEntityAttribute _entityAttr;
        private bool _readFromView;
        private string _selectCommandFormat;
        private string _updateCommandFormat;
        private string _insertCommandFormatString;
        private EntityFieldInfo _rowidField;

        public Dictionary<String, EntityFieldInfo> Properties { get; protected set; }

        public String ReadSource { get; private set; }
        public String TableName { get; private set; }
        public String JoinCommand { get; private set; }
        public bool IsCached { get; private set; }

        private void AddProperty(PropertyInfo pi)
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
            Properties.Add(pi.Name, fi);
        }

        

        private object GetValueByType(Type type, IDataReader reader, int ord)
        {
            if (type.IsEnum)
            {
                return GetEnum(reader, ord, type);
            }
            else if (type.Equals(typeof(Guid)))
            {
                return GetGuid(reader, ord);
            }
            else if (reader.IsDBNull(ord))
            {
                return (type.IsValueType) ? Activator.CreateInstance(type) : null;
            }

            TypeCode tc = Type.GetTypeCode(type);
            try
            {
                switch (tc)
                {
                    case TypeCode.Boolean:
                        {
                            return reader.GetBoolean(ord);
                        }
                    case TypeCode.Byte:
                        {
                            return GetByte(reader, ord);
                        }
                    case TypeCode.DateTime:
                        {
                            return reader.GetDateTime(ord);
                        }
                    case TypeCode.Double:
                        {
                            return reader.GetDouble(ord);
                        }
                    case TypeCode.Decimal:
                        {
                            return reader.GetDecimal(ord);
                        }
                    case TypeCode.Int16:
                        {
                            return reader.GetInt16(ord);
                        }
                    case TypeCode.Int32:
                        {
                            return reader.GetInt32(ord);
                        }
                    case TypeCode.Int64:
                        {
                            return reader.GetInt64(ord);
                        }
                    case TypeCode.Single:
                        {
                            return reader.GetFloat(ord);
                        }
                    case TypeCode.String:
                        {
                            return reader.GetString(ord);
                        }
                        
                    default:
                        {
                            return reader.GetValue(ord);
                        }
                }
            }
            catch (InvalidCastException)
            {
                
                Object value = reader.GetValue(ord);
                Debug.WriteLine("InvalidCastException in GetValueByType" +
                    " ExpectedType:" + type.Name + " " +
                    " DOType:" + this.EntityType.Name +
                    " Value = " + value.ToString() + ":" + value.GetType().Name);

                if (tc == TypeCode.String)
                {
                    return value.ToString();
                }
                else if (type.IsInstanceOfType(value))
                {
                    return value;
                }
                else
                {
                    if (type.IsValueType)
                    {
                        return Activator.CreateInstance(type);
                    }
                    else
                    {
                        return null;
                    }
                }

            }
        }

        

        public void ReadData(System.Data.IDataReader reader, Object dataObject)
        {
            foreach(EntityFieldInfo fi in Properties.Values)
            {
                try
                {
                    if (fi._ordinal < 0) { continue; }
                    object value = null;
                    Type type = fi.DataType;
                    value = GetValueByType(type, reader, fi._ordinal);
                    fi.SetFieldValue(dataObject, value);
                }
                catch (Exception e)
                {
                    throw new ORMException("Error in ReadData: field info = " + fi.ToString(), e);
                }
            }

            
            if (dataObject is DataObject)
            {
                if (_rowidField != null)
                {
                    ((DataObject)dataObject).rowID = GetRowID(reader, _rowidField._ordinal);
                }
                ((DataObject)dataObject).IsPersisted = true;
                ((DataObject)dataObject).IsValidated = false;
            }
        }

        public long ReadID(System.Data.IDataReader reader)
        {
            if(_rowidField != null)
            {
                return GetID(GetRowID(reader, _rowidField._ordinal));
            }

            return -1;
        }

        /// <summary>
        /// Creates a unique DataObject ID based on DataObject type and rowID from database
        /// </summary>
        /// <param name="type"></param>
        /// <param name="rowID"></param>
        /// <returns></returns>
        public long GetID( long? rowID)
        {
            Debug.Assert(rowID != null);
            unchecked
            {
                return _typeID ^ (long)rowID * 31;
            }
        }

        /// <summary>
        /// Prepares the DataObjectDiscription instance to read data from <paramref name="reader"/>
        /// use before calling ReadData
        /// </summary>
        /// <param name="reader"></param>
        public void StartRead(System.Data.IDataReader reader)
        {
            this.StartRead(reader, false);
        }

        public void StartRead(System.Data.IDataReader reader, bool refreshOrdnals)
        {
            foreach (EntityFieldInfo fi in Properties.Values)
            {
                fi._ordinal = reader.GetOrdinal(fi._fieldAttr.Alias ?? fi._fieldAttr.FieldName);
            }

            if (_rowidField != null)
            {
                this._rowidField._ordinal = reader.GetOrdinal("tableRowID");
            }
        }

        internal string GetSelectCommandFormat()
        {
            if (_selectCommandFormat == null)
            {
                StringBuilder sb = new StringBuilder("SELECT ");

                bool first = true;
                foreach (EntityFieldInfo fi in Properties.Values)
                {
                    if (fi._fieldAttr == null) { continue; }
                    String colExpression = null;

                    if (!string.IsNullOrEmpty(fi._fieldAttr.MapExpression))
                    {
                        colExpression = fi._fieldAttr.MapExpression;
                    }
                    else if (!string.IsNullOrEmpty(fi.FieldName))
                    {
                        colExpression = ((String.IsNullOrEmpty(TableName)) ? String.Empty : TableName + ".") + fi._fieldAttr.FieldName;
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
                sb.AppendFormat(null,"INSERT OR {{0}} INTO {0}( ",this.TableName);                
                //build the column names section of the insert command 
                bool first = true;
                foreach(FieldInfo fi in Properties.Values)
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
                foreach(FieldInfo fi in Properties.Values)
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
            foreach(FieldInfo fi in Properties.Values)
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
                sb.AppendFormat(null,"UPDATE OR {0} {1} SET ", "{0}", this.TableName);

                String[] colExprs = new String[Properties.Values.Count];
                int i = 0;
                foreach(FieldInfo fi in Properties.Values)
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
            foreach(FieldInfo fi in Properties.Values)
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
            DELETE FROM ErrorLog WHERE TableName = '{0}' AND CN_Number = @rowID;", this.TableName);
		
		    SQLiteCommand command = new SQLiteCommand(query);
		    command.Parameters.Add(new SQLiteParameter("@rowID", data.rowID));
		    return command;
		}

        public static object GetEnum(IDataReader reader, int ord, Type eType)
        {
            String s = GetString(reader, ord);
            try
            {                
                return Enum.Parse(eType, s, true);
            }
            catch
            {
                Debug.Write(String.Format("GetEnum failed to parse value ({0}) to {1}", s, eType.Name));
                return Enum.Parse(eType, "0", true);
            }

        }

        public static Int32 GetInt32(IDataReader reader, int ord)
        {
            if (reader.IsDBNull(ord) )
            { //throw new DataException("Trying to read Int32 value, value was null"); 
                return 0;
            }
            else
            {
                try
                {
                    return reader.GetInt32(ord);
                }
                catch (InvalidCastException)
                {
                    object value = reader.GetValue(ord);
                    if (value is Int64)
                    {
#if !WindowsCE
                        Trace.Write(String.Format("{1}: Type expected Int32, Value read: {0} ({2})\r\n",

                        reader.GetValue(ord),
                        string.Empty,
                        reader.GetValue(ord).GetType().Name),
                        "Warning");
#endif
                        return (Int32)value;
                    }
                    else
                    {
#if !WindowsCE
                        Trace.Write(String.Format("{1}: Try read Int32 failed, returned 0, actual: {0} ({2})\r\n",
                            reader.GetValue(ord),
                            string.Empty,
                            reader.GetValue(ord).GetType().Name),
                            "Error");
#endif
                        //Trace.TraceWarning(String.Format("{1}: Try read Int32 failed, returned null, actual: {0} ({2})", reader.GetValue(ord), this.GetType().Name, reader.GetValue(ord).GetType().Name)); 
                        //Logger.Log.V(String.Format("{1}: Try read Int32 failed, returned null, actual: {0} ({2})", reader.GetValue(ord), this.GetType().Name, reader.GetValue(ord).GetType().Name)); 
                        return 0;
                    }
                }
            }
        }

        public static long? GetRowID(IDataReader reader, int ord)
        {
            if (reader.IsDBNull(ord) )
            { //throw new DataException("Trying to read Int64 value, but value was null"); 
                return null;
            }
            else
            {
                try
                {
                    return reader.GetInt64(ord);
                }
                catch (InvalidCastException)
                {

                    if (reader.GetValue(ord) is long) { return (long)reader.GetValue(ord); }
#if !WindowsCE                    
                    Trace.Write(String.Format("{1}: Try read key failed, returned null, actual: {0} ({2})\r\n",
                        reader.GetValue(ord),
                        string.Empty,
                        reader.GetValue(ord).GetType().Name),
                        "Warning");
#endif
                    //Trace.TraceWarning(String.Format("{1}: Try read Int64 failed, returned null, actual: {0} ({2})", reader.GetValue(ord), this.GetType().Name, reader.GetValue(ord).GetType().Name)); 
                    //Logger.Log.V(String.Format("{1}: Try read Int64 failed, returned null, actual: {0} ({2})", reader.GetValue(ord), this.GetType().Name, reader.GetValue(ord).GetType().Name)); 
                    return null;
                }
            }
        }

        public static long GetInt64(IDataReader reader, int ord)
        {
            if (reader.IsDBNull(ord) )
            { //throw new DataException("Trying to read Int64 value, but value was null"); 
                return 0;
            }
            else
            {
                try
                {
                    return reader.GetInt64(ord);
                }
                catch (InvalidCastException)
                {

                    if (reader.GetValue(ord) is long) { return (long)reader.GetValue(ord); }
#if !WindowsCE                    
                    Trace.Write(String.Format("{1}: Try read Int64 failed, returned 0, actual: {0} ({2})\r\n",
                        reader.GetValue(ord),
                        string.Empty,
                        reader.GetValue(ord).GetType().Name),
                        "Warning");
#endif
                    //Trace.TraceWarning(String.Format("{1}: Try read Int64 failed, returned null, actual: {0} ({2})", reader.GetValue(ord), this.GetType().Name, reader.GetValue(ord).GetType().Name)); 
                    //Logger.Log.V(String.Format("{1}: Try read Int64 failed, returned null, actual: {0} ({2})", reader.GetValue(ord), this.GetType().Name, reader.GetValue(ord).GetType().Name)); 
                    return 0;
                }
            }
        }

        public static float GetFloat(IDataReader reader, int ord)
        {
            if (reader.IsDBNull(ord))
            { //throw new DataException("Trying to read float value, but value was null");
                return 0.0F;
            }
            else
            {
                try
                {
                    return reader.GetFloat(ord);
                }
                catch (InvalidCastException)
                {
#if !WindowsCE
                    Trace.Write(String.Format("{1}: Try read Float failed, returned 0.0f, actual: {0} ({2})\r\n",
                        reader.GetValue(ord),
                        string.Empty,
                        reader.GetValue(ord).GetType().Name),
                        "Warning");
#endif
                    //Trace.TraceWarning(String.Format("{1}: Try read Float failed, returned null, actual: {0} ({2})", reader.GetValue(ord), this.GetType().Name, reader.GetValue(ord).GetType().Name)); 
                    //Logger.Log.V(String.Format("{1}: Try read Float failed, returned null, actual: {0} ({2})", reader.GetValue(ord), this.GetType().Name, reader.GetValue(ord).GetType().Name)); 
                    return 0.0f;
                }
            }
        }

        public static Double GetDouble(IDataReader reader, int ord)
        {
            if (reader.IsDBNull(ord))
            { //throw new DataException("Trying to read Double value, value was null"); 
                return 0.0;
            }
            else
            {
                try
                {
                    return reader.GetDouble(ord);
                }
                catch (InvalidCastException)
                {
                    return 0.0;
                } 
            }
        }

        public static string GetString(IDataReader reader, int ord)
        {
            if (reader.IsDBNull(ord)) { return null; }
            else
            {
                string value;
                try
                {
                    value = reader.GetString(ord);
                }
                catch (InvalidCastException)
                {
                    value = reader.GetValue(ord).ToString();
                }
                return value;
            }
        }

        public static bool GetBool(IDataReader reader, int ord)
        {
            if (reader.IsDBNull(ord))
            {
                return false;
            }
            else
            {
                try
                {
                    return reader.GetBoolean(ord);
                }
                catch (InvalidCastException)
                {
                    object value = reader.GetValue(ord);
                    if (value is bool)
                    {
                        return (bool)value;
                    }

                    return false;
                }
            }
        }

        public static byte[] GetByte(IDataReader reader, int ord)
        {
            
            if (reader.IsDBNull(ord))
            {
                return null;
            }
            else
            {
                try
                {
                    byte[] bytes = new Byte[BYTE_READ_LENGTH];
                    reader.GetBytes(ord, 0, bytes, 0, BYTE_READ_LENGTH);
                    return bytes;
                }
                catch (InvalidCastException)
                {
                    return null;
                }
            }
        }

        public static DateTime GetDateTime(IDataReader reader, int ord)
        {
            if (reader.IsDBNull(ord))
            {
                return default(DateTime);
            }
            else
            {
                try
                {
                    return reader.GetDateTime(ord);
                }
                catch (InvalidCastException)
                {
                    return default(DateTime);
                }
            }
        }

        public static Guid GetGuid(IDataReader reader, int ord)
        {
            if (reader.IsDBNull(ord))
            {
                return Guid.Empty;
            }
            else
            {
                try
                {
                    return reader.GetGuid(ord);
                }
                catch (InvalidCastException)
                {
                    return Guid.Empty;
                }
            }
        }

        
    }

        
    

}