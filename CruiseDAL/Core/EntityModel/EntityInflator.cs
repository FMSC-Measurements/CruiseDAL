
using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Reflection;

using CruiseDAL.Core.EntityAttributes;

namespace CruiseDAL.Core.EntityModel
{
    public class EntityInflator
    {
        const int BYTE_READ_LENGTH = 1024;//TODO once we start reading byte data figure out our byte read length... should this be done at runtime?

        protected EntityDescription EntityDescription { get; set; }

        public EntityInflator(EntityDescription entity)
        {
            EntityDescription = entity;

            _constructor = EntityDescription.EntityType.GetConstructor(new Type[] { });
        }

        ConstructorInfo _constructor;

        /// <summary>
        /// Prepares the DataObjectDiscription instance to read data from <paramref name="reader"/>
        /// use before calling ReadData
        /// </summary>
        /// <param name="reader"></param>
        public void CheckOrdinals(System.Data.IDataReader reader)
        {
            foreach (FieldAttribute field in EntityDescription.Fields)
            {
                field.Ordinal = reader.GetOrdinal(field.FieldName);
            }

            PrimaryKeyFieldAttribute keyField = EntityDescription.Fields.PrimaryKeyField;
            if(keyField != null)
            {
                keyField.Ordinal = reader.GetOrdinal(keyField.FieldName);
            }
        }

        public object CreateInstanceOfEntity()
        {
            return Activator.CreateInstance(EntityDescription.EntityType);
        }


        public void ReadData(System.Data.IDataReader reader, Object obj)
        {
            if (obj is ISupportInitialize)
            {
                ((ISupportInitialize)obj).BeginInit();
            }

            foreach (FieldAttribute field in EntityDescription.Fields)
            {
                try
                {
                    if (field.Ordinal < 0) { continue; }

                    object value = GetValueByType(field.RunTimeType, reader, field.Ordinal);
                    field.SetFieldValue(obj, value);
                }
                catch (Exception e)
                {
                    throw new ORMException("Error in ReadData: field info = " + field.ToString(), e);
                }
            }

            PrimaryKeyFieldAttribute keyField = EntityDescription.Fields.PrimaryKeyField;
            if (keyField != null && keyField.Ordinal != -1)
            {
                object value = GetValueByType(keyField.RunTimeType, reader, keyField.Ordinal);
                keyField.SetFieldValue(obj, value);
            }

            if (obj is ISupportInitialize)
            {
                ((ISupportInitialize)obj).EndInit();
            }

        }

        public object ReadPrimaryKey(System.Data.IDataReader reader)
        {
            PrimaryKeyFieldAttribute keyField = EntityDescription.Fields.PrimaryKeyField;
            if (keyField != null && keyField.Ordinal != -1)
            {
                object value = GetValueByType(keyField.RunTimeType, reader, keyField.Ordinal);
                return value;
            }
            else
            {
                return null;
            }
        }

//        public static long? GetRowID(IDataReader reader, int ord)
//        {
//            if (reader.IsDBNull(ord))
//            { //throw new DataException("Trying to read Int64 value, but value was null"); 
//                return null;
//            }
//            else
//            {
//                try
//                {
//                    return reader.GetInt64(ord);
//                }
//                catch (InvalidCastException)
//                {

//                    if (reader.GetValue(ord) is long) { return (long)reader.GetValue(ord); }
//#if !WindowsCE                    
//                    Trace.Write(String.Format("{1}: Try read key failed, returned null, actual: {0} ({2})\r\n",
//                        reader.GetValue(ord),
//                        string.Empty,
//                        reader.GetValue(ord).GetType().Name),
//                        "Warning");
//#endif
//                    //Trace.TraceWarning(String.Format("{1}: Try read Int64 failed, returned null, actual: {0} ({2})", reader.GetValue(ord), this.GetType().Name, reader.GetValue(ord).GetType().Name)); 
//                    //Logger.Log.V(String.Format("{1}: Try read Int64 failed, returned null, actual: {0} ({2})", reader.GetValue(ord), this.GetType().Name, reader.GetValue(ord).GetType().Name)); 
//                    return null;
//                }
//            }
//        }

        public static Int32 GetInt32(IDataReader reader, int ord)
        {
            if (reader.IsDBNull(ord))
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

        public static long GetInt64(IDataReader reader, int ord)
        {
            if (reader.IsDBNull(ord))
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

        public static object GetEnum(IDataReader reader, int ord, Type eType)
        {
            String s = GetString(reader, ord);
            try
            {
                if (String.IsNullOrEmpty(s))
                {
                    return Enum.Parse(eType, "0", true);
                }
                return Enum.Parse(eType, s, true);
            }
            catch
            {
                Debug.Write(String.Format("GetEnum failed to parse value ({0}) to {1}", s, eType.Name));
                return Enum.Parse(eType, "0", true);
            }

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
                    " DOType:" + this.EntityDescription.EntityType.Name +
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
    }
}
