using FMSC.ORM.Core;
using System;
using System.Collections.Generic;
using System.Data;

namespace FMSC.ORM.EntityModel.Support
{
    public class EntityInflator
    {
        private Dictionary<string, int> OrdinalLookup { get; set; }

        protected EntityDescription EntityDescription { get; }

        public EntityInflator(EntityDescription entity)
        {
            EntityDescription = entity;
        }

        /// <summary>
        /// Prepares the DataObjectDiscription instance to read data from <paramref name="reader"/>
        /// use before calling ReadData
        /// </summary>
        /// <param name="reader"></param>
        public void CheckOrdinals(System.Data.IDataReader reader)
        {
            var fieldCount = reader.FieldCount;
            var ordinalLookup = new Dictionary<string, int>(fieldCount);

            for (int i = 0; i < fieldCount; i++)
            {
                var fieldName = reader.GetName(i).ToLower(System.Globalization.CultureInfo.InvariantCulture);
                try
                {
                    ordinalLookup.Add(fieldName, i);
                }
                catch
                {
                    //ignore exception when adding duplicate or null value
                }
            }

            OrdinalLookup = ordinalLookup;
        }

        public void ReadData(System.Data.IDataReader reader, Object obj)
        {
            var ordinalLookup = OrdinalLookup;
            foreach (var field in EntityDescription.Fields)
            {
                if (ReadField(reader, ordinalLookup, field, out var value))
                {
                    field.SetFieldValue(obj, value);
                }
            }

            var keyField = EntityDescription.Fields.PrimaryKeyField;
            if (keyField != null)
            {
                if (ReadField(reader, ordinalLookup, keyField, out var value))
                {
                    keyField.SetFieldValue(obj, value);
                }
            }

            if (obj is IPersistanceTracking)
            {
                ((IPersistanceTracking)obj).OnRead();
            }
        }

        public object ReadPrimaryKey(System.Data.IDataReader reader)
        {
            var keyField = EntityDescription.Fields.PrimaryKeyField;

            if (keyField != null && ReadField(reader, OrdinalLookup, keyField, out var value))
            { return value; }
            else
            { return null; }
        }

        private static bool ReadField(IDataReader reader, Dictionary<string, int> ordinalMapping, FieldInfo field, out object value)
        {
            try
            {
                var fieldName = (field.Alias ?? field.Name).ToLower(System.Globalization.CultureInfo.InvariantCulture);
                int ordinal = -1;

                if (ordinalMapping.TryGetValue(fieldName, out ordinal))
                {
                    var runtimeType = field.RunTimeType;
                    object dbValue = null;
                    try
                    {
                        dbValue = reader.GetValue(ordinal);
                    }
                    catch (FormatException) when (runtimeType == typeof(string))
                    { dbValue = reader.GetString(ordinal); }
                    catch (FormatException) when (runtimeType == typeof(DateTime) || runtimeType == typeof(DateTime?))
                    {
                        // HACK if date time is not in ISO8601 then System.Data.Sqlite
                        // will fail when calling GetValue because it is attempting to convert it to a datetime
                        // the solution is to attempt to read it as a string and let the conversion in ProcessValue
                        // convert the string to a DateTime
                        dbValue = reader.GetString(ordinal);
                    }

                    try
                    {
                        value = ValueMapper.ProcessValue(runtimeType, dbValue);
                    }
                    catch (FormatException) when (runtimeType == typeof(Guid) || runtimeType == typeof(Guid?))
                    {
                        var bytes = new byte[16];
                        reader.GetBytes(ordinal, 0, bytes, 0, 16);
                        value = ValueMapper.ProcessValue(runtimeType, bytes);
                    }

                    return true;
                }
                else
                {
                    value = null;
                    return false;
                }
            }
            catch (Exception e)
            {
                throw new ORMException("Error in ReadData: field info = " + field.ToString(), e);
            }
        }
    }
}