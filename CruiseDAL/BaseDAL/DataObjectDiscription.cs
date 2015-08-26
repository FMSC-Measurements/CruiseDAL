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


namespace CruiseDAL
{
    //public delegate void DataObjectPropertySetter(DataObject obj, Object value);
    //public delegate Object DataObjectPropertyGetter(DataObject obj);

    internal class DataObjectInfo
    {
        internal class FieldInfo
        {
            //public PropertyInfo _propInfo;
            public FieldAttribute _fieldAttr;
            public Type _propType;
            public int _ordinal = -1; //cached dataReader ordinal
            public MethodInfo _getter;
            public MethodInfo _setter;

            public override string ToString()
            {
                return String.Format("PropType({0}); DBType({1}), FieldName({2}), Alias({3}), MapExpression({4})",
                    _propType.Name, _fieldAttr.DataType, _fieldAttr.FieldName?? "null", _fieldAttr.Alias ?? "null", _fieldAttr.MapExpression ?? "null");
            }
        }

        public DataObjectInfo(Type type)
        {
            _dataObjectType = type;

            object[] tAttrs = type.GetCustomAttributes(typeof(TableAttribute), true);
            _tableAttr = (tAttrs.Length > 0) ? (TableAttribute)tAttrs[0] : null;

            if (_tableAttr != null)
            {
                this.TableName = _tableAttr.TableName;
                this.JoinCommand = string.Format(" {0} ",_tableAttr.JoinCommand);
            }

            PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            
            Properties = new Dictionary<string,FieldInfo>(props.Length,  StringComparer.Create(System.Globalization.CultureInfo.CurrentCulture, true));
            foreach (PropertyInfo p in props)
            {
                FieldAttribute fa = (FieldAttribute)Attribute.GetCustomAttribute(p, typeof(FieldAttribute));
                if (fa != null)
                {
                    FieldInfo fi = new FieldInfo();
                        fi._fieldAttr = fa;
                        //fi._propInfo = p;
                        fi._getter = p.GetGetMethod();
                        fi._setter = p.GetSetMethod();
                        fi._propType = p.PropertyType;
                        Properties.Add(p.Name, fi);
                }
                //object[] fAttrs = p.GetCustomAttributes(typeof(FieldAttribute),
                //object[] fAttrs = p.GetCustomAttributes(typeof(FieldAttribute), true);
                //if (fAttrs.Length > 0)
                //{
                //    FieldInfo fi = new FieldInfo();
                //    fi._fieldAttr = fAttrs[0] as FieldAttribute;
                //    //fi._propInfo = p;
                //    fi._getter = p.GetGetMethod();
                //    fi._setter = p.GetSetMethod();
                //    fi._propType = p.PropertyType;
                //    Properties.Add(p.Name, fi);
                //}
            }

            PropertyInfo rowIDProp = type.GetProperty("rowID");
            FieldInfo finfo = new FieldInfo();
            //finfo._propInfo = rowIDProp;
            finfo._getter = rowIDProp.GetGetMethod();
            finfo._setter = rowIDProp.GetSetMethod();
            finfo._propType = rowIDProp.PropertyType;
            Properties.Add("rowID", finfo);

        }

        const int BYTE_READ_LENGTH = 1024;//TODO onece we start reading byte data figure out our byte read length... should this be done at runtime?

        private Type _dataObjectType;
        private TableAttribute _tableAttr;
        private string _selectCommandFormat;
        private string _updateCommandFormat;
        private string _insertCommandFormatString;
        private int? _rowidOrd;

        internal Dictionary<String, FieldInfo> Properties { get; set; }

        public String TableName { get; private set; }
        public String JoinCommand { get; private set; }

        internal string GetSelectCommandFormat()
        {
            if(_selectCommandFormat == null)
            {
                StringBuilder sb = new StringBuilder("SELECT ");
                foreach(FieldInfo fi in Properties.Values)
                {
                    if (fi._fieldAttr == null) { continue; }
                    String colExpression = null;
                    if(!string.IsNullOrEmpty(fi._fieldAttr.MapExpression))
                    {
                        colExpression = fi._fieldAttr.MapExpression;
                    }
                    else if(!string.IsNullOrEmpty(fi._fieldAttr.FieldName))
                    {
                        colExpression = (TableName + "." + fi._fieldAttr.FieldName);
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
                        sb.Append(colExpression + ",");
                    }

                }
                sb.AppendFormat(null, " {0}.rowID as tableRowID FROM {0}{1}{2};", TableName, JoinCommand, "{0}");
                _selectCommandFormat = sb.ToString();
            }

            return _selectCommandFormat;
        }

        //TODO use this method in place of switch statment in ReadData
        private object GetValueByType(Type type, IDataReader reader, int ord)
        {
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
                Debug.WriteLine(String.Format("InvalidCastException in GetValueByType, Value = ({0}, {1}) ReadToType = ({2})", value.ToString(), value.GetType().ToString(), type.ToString()));
                if (type.IsInstanceOfType(value))
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

        private void SetFieldValue(DataObject dataObject, FieldInfo field, object value)
        {
            if (field == null || field._setter == null)
            {
                return;
            }
            try
            {

                field._setter.Invoke(dataObject, new Object[] { value, });
            }
            catch
            {
                throw new ORMException(String.Format("unable to set value; Value = {0}; FieldInfo = {1}", value, field));
            }
        }


        public void ReadData(System.Data.IDataReader reader, DataObject dataObject)
        {
            foreach(FieldInfo fi in Properties.Values)
            {
                try
                {
                    if (fi._fieldAttr == null) { continue; }
                    object value = null;
                    Type type = fi._propType;
                    if (fi._propType.IsEnum)
                    {
                        value = GetEnum(reader, fi._ordinal, type);
                    }
                    else
                    {
                        if (reader.IsDBNull(fi._ordinal))
                        {
                            value = (type.IsValueType) ? Activator.CreateInstance(type) : null;
                        }
                        else
                        {
                            value = GetValueByType(type, reader, fi._ordinal);
                        }
                        //switch (fi._propType.Name)
                        //{
                        //    case "Int32":
                        //        {
                        //            value = GetInt32(reader, fi._ordinal);
                        //            break;
                        //        }
                        //    case "Int64":
                        //        {
                        //            value = GetInt64(reader, fi._ordinal);
                        //            break;
                        //        }
                        //    case "Double":
                        //        {
                        //            value = GetDouble(reader, fi._ordinal);
                        //            break;
                        //        }
                        //    case "Single":
                        //        {
                        //            value = GetFloat(reader, fi._ordinal);
                        //            break;
                        //        }
                        //    case "Boolean":
                        //        {
                        //            value = GetBool(reader, fi._ordinal);
                        //            break;
                        //        }
                        //    case "String":
                        //        {
                        //            value = GetString(reader, fi._ordinal);
                        //            break;
                        //        }
                        //    case "Byte[]":
                        //        {
                        //            value = GetByte(reader, fi._ordinal);
                        //            break;
                        //        }
                        //    default:
                        //        {
                        //            value = reader.GetValue(fi._ordinal);
                        //            if (value == DBNull.Value)
                        //            { value = null; }
                        //            break;
                        //        }
                        //}//end switch
                    }
                    SetFieldValue(dataObject, fi, value);
                }
                catch (Exception e)
                {
                    throw new ORMException("Error in ReadData: field info = " + fi.ToString(), e);
                }
            }//end foreach

            if (_rowidOrd != null)
            {
                dataObject.rowID = GetRowID(reader, _rowidOrd.Value);

            }
            dataObject.IsPersisted = true;
            dataObject.IsValidated = false;
        }

        public long ReadID(System.Data.IDataReader reader)
        {
            if(_rowidOrd != null)
            {
                return DataObject.GetID(_dataObjectType, GetRowID(reader, _rowidOrd.Value));
            }

            return -1;
        }

        public void StartRead(System.Data.IDataReader reader)
        {
            foreach (FieldInfo fi in Properties.Values)
            {
                if (fi._fieldAttr == null ||fi._ordinal != -1) { continue; }
                fi._ordinal = reader.GetOrdinal(fi._fieldAttr.Alias ?? fi._fieldAttr.FieldName);
            }
            this._rowidOrd = reader.GetOrdinal("tableRowID");
        }

        private string GetInsertCommandFormatString()
        {
            //check if we already have a cached insert command
            if(_insertCommandFormatString == null)
            {
                //create first part of insert command, leaving placeholder for onConflect option
                StringBuilder sb = new StringBuilder(String.Format("INSERT OR {0} INTO {1}( ", "{0}",this.TableName));

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


        public SQLiteCommand CreateSQLInsert(DataObject data, string createdBy,bool setRowID, OnConflictOption option)
        {

            string commandText = string.Format(GetInsertCommandFormatString(), option.ToString(),
                (setRowID) ? ", rowID" : String.Empty,
                (setRowID) ? ", @rowID" : String.Empty);
            SQLiteCommand command = new SQLiteCommand(commandText);
            foreach(FieldInfo fi in Properties.Values)
            {
                if (fi._fieldAttr == null) { continue; }
                if(fi._fieldAttr.IsPersisted )
                {
                    object value = fi._getter.Invoke(data, null);
                    if(fi._propType.IsEnum)
                    {
                        value = value.ToString();
                    }
                    command.Parameters.Add(new SQLiteParameter("@" + fi._fieldAttr.FieldName, value));
                }
                if (fi._fieldAttr.SpecialFieldType == SepcialFieldType.CreatedBy)
                {
                    command.Parameters.Add(new SQLiteParameter("@" + fi._fieldAttr.FieldName, createdBy));
                }
            }

            if (setRowID)
            {
                command.Parameters.AddWithValue("@rowID", data.rowID);
            }
            
            return command;
        }

        private string GetUpdateCommandFormatString()
        {
            if(_updateCommandFormat == null)
            {
                //create first part of update command with place holder for OnConflictOption. 
                StringBuilder sb = new StringBuilder(String.Format("UPDATE OR {0} {1} SET ", "{0}", this.TableName));

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

        public SQLiteCommand CreateSQLUpdate(DataObject data, string ModifiedBy, OnConflictOption option)
        {
            string commandText = string.Format(GetUpdateCommandFormatString(), option.ToString());
            SQLiteCommand command = new SQLiteCommand(commandText);
            foreach(FieldInfo fi in Properties.Values)
            {
                if (fi._fieldAttr == null) { continue; }
                if (fi._fieldAttr.IsPersisted )
                {
                    object value = fi._getter.Invoke(data, null);
                    if(fi._propType.IsEnum)
                    {
                        value = value.ToString();
                    }
                    command.Parameters.Add(new SQLiteParameter("@" + fi._fieldAttr.FieldName, value));
                }
                if (fi._fieldAttr.SpecialFieldType == SepcialFieldType.ModifiedBy)
                {
                    command.Parameters.Add(new SQLiteParameter("@" + fi._fieldAttr.FieldName, ModifiedBy));
                }
            }

            command.Parameters.Add(new SQLiteParameter("@rowID", data.rowID));

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

    }

        
    

}