using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace FMSC.ORM.EntityModel.Support
{
    public class EntityInflator
    {
        private const int BYTE_READ_LENGTH = 1024;//TODO once we start reading byte data figure out our byte read length... should this be done at runtime?

        //ConstructorInfo _constructor;

        Dictionary<string, int> OrdinalMapping { get; set; }

        protected EntityDescription EntityDescription { get; set; }

        public EntityInflator(EntityDescription entity)
        {
            EntityDescription = entity;

            //_constructor = EntityDescription.EntityType.GetConstructor(new Type[] { });
        }

        /// <summary>
        /// Prepares the DataObjectDiscription instance to read data from <paramref name="reader"/>
        /// use before calling ReadData
        /// </summary>
        /// <param name="reader"></param>
        public void CheckOrdinals(System.Data.IDataReader reader)
        {
            var fieldCount = reader.FieldCount;
            var ordinalMapping = new Dictionary<string, int>(fieldCount);

            for(int i = 0; i < fieldCount; i++)
            {
                var fieldName = reader.GetName(i).ToLowerInvariant();
                try
                {
                    ordinalMapping.Add(fieldName, i);
                }
                catch
                {
                    //ignore exception when adding duplicate or null value
                }
            }

            OrdinalMapping = ordinalMapping;
        }

        //public object CreateInstanceOfEntity()
        //{
        //    return Activator.CreateInstance(EntityDescription.EntityType);
        //}

        public void ReadData(System.Data.IDataReader reader, Object obj)
        {
            var ordinalMapping = OrdinalMapping;
            foreach (FieldAttribute field in EntityDescription.Fields)
            {
                try
                {
                    var fieldName = field.NameOrAlias.ToLowerInvariant();
                    int ordinal = -1;

                    if(ordinalMapping.TryGetValue(fieldName, out ordinal))
                    {
                        object value = GetValueByType(field.RunTimeType, reader, ordinal);
                        field.SetFieldValue(obj, value);
                    }
                }
                catch (Exception e)
                {
                    throw new ORMException("Error in ReadData: field info = " + field.ToString(), e);
                }
            }

            PrimaryKeyFieldAttribute keyField = EntityDescription.Fields.PrimaryKeyField;
            if (keyField != null)
            {
                object value = ReadPrimaryKey(reader);
                keyField.SetFieldValue(obj, value);
            }

            if (obj is IPersistanceTracking)
            {
                ((IPersistanceTracking)obj).OnRead();
            }
        }

        public object ReadPrimaryKey(IDataReader reader)
        {
            var keyField = EntityDescription.Fields.PrimaryKeyField;
            if (keyField == null) { return null; }
            var fieldName = keyField.NameOrAlias.ToLowerInvariant();

            int ordinal = -1;
            if(OrdinalMapping.TryGetValue(fieldName.ToLowerInvariant(), out ordinal))
            {
                return GetValueByType(keyField.RunTimeType, reader, ordinal);
            }
            else
            {
                throw new ORMException("field " + fieldName + " not found on datareader");
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
                if (type.IsValueType) { return Activator.CreateInstance(type); }
                else { return null; }
            }

            // check our target type and see if it is a nullable type
            // if so get the underlying type
            var underlyingType = (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                ? Nullable.GetUnderlyingType(type) : null; 

            // get typecode for our target type or underlying type if target type is nullable
            TypeCode tc = Type.GetTypeCode(underlyingType ?? type);


            try
            {
                var value = GetValueByTypeCode(tc, reader, ord);

                if (underlyingType != null)
                {
                    return Convert.ChangeType(value, tc
                        , System.Globalization.CultureInfo.CurrentCulture);
                }

                return value;
            }
            catch (InvalidCastException)
            {
                var value = reader.GetValue(ord);
                var valueType = value.GetType();

                if (type.IsAssignableFrom(valueType))
                {
                    return value;
                }

                Debug.WriteLine("InvalidCastException in GetValueByType" +
                    " ExpectedType:" + type.Name + " " +
                    " DOType:" + this.EntityDescription.EntityType.Name +
                    " Value = " + value.ToString() + ":" + value.GetType().Name);

                try
                {
                    return Convert.ChangeType(value, tc
                        , System.Globalization.CultureInfo.CurrentCulture);
                }
                catch (Exception)
                {
                    Debug.Fail("Unable to read value");
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
                catch (Exception)
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

        private byte[] GetByte(IDataReader reader, int ord)
        {
            throw new NotImplementedException();
        }

        private object GetValueByTypeCode(TypeCode tc, IDataReader reader, int ord)
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
                        try
                        {
                            return reader.GetString(ord);
                        }
                        catch (InvalidCastException)
                        {
                            return reader.GetValue(ord).ToString();
                        }
                    }
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    {
                        return Convert.ChangeType(reader.GetValue(ord), tc
                            , System.Globalization.CultureInfo.CurrentCulture);
                    }
                default:
                    {
                        return reader.GetValue(ord);
                    }
            }
        }
    }
}