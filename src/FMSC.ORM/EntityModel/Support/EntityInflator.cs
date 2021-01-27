using FMSC.ORM.Core;
using FMSC.ORM.Util;
using System;
using System.Collections.Generic;
using System.Data;

namespace FMSC.ORM.EntityModel.Support
{
    // TODO optimize this class some more. use a hash to see if the look up needs to be recreated?

    public class EntityInflator
    {
        private IDictionary<string, int> OrdinalLookup { get; set; }

        public EntityInflator(IDataReader reader)
        {
            OrdinalLookup = GetOrdinalLookup(reader);
        }

        public static IDictionary<string, int> GetOrdinalLookup(IDataReader reader)
        {
            //HACK with microsoft.data.sqlite calling GetOrdinal throws exception if reader is empty
            //if(reader is DbDataReader && ((DbDataReader)reader).HasRows == false) { return; }


            var fieldCount = reader.FieldCount;
            var fields = new string[fieldCount];
            var ordinalLookup = new Dictionary<string, int>(fieldCount);

            for (int i = 0; i < fieldCount; i++)
            {
                var fieldName = reader.GetName(i).ToLower(System.Globalization.CultureInfo.InvariantCulture);
                fields[i] = fieldName;
                if (ordinalLookup.ContainsKey(fieldName) == false)
                {
                    ordinalLookup.Add(fieldName, i);
                }
            }
            var hash = fields.CombineHashs();
            return ordinalLookup;
        }

        public void ReadData(System.Data.IDataReader reader, Object obj, EntityDescription discription)
        {
            ReadData(reader, obj, discription, OrdinalLookup);
        }

        public void ReadData(System.Data.IDataReader reader, Object obj, EntityDescription discription, IDictionary<string, int> ordinalLookup)
        {
            var fields = discription.Fields;
            foreach (var field in fields)
            {
                if (ReadField(reader, ordinalLookup, field, out var value))
                {
                    field.SetFieldValue(obj, value);
                }
            }

            var keyField = fields.PrimaryKeyField;
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

        public object ReadPrimaryKey(System.Data.IDataReader reader, EntityDescription discription)
        {
            return ReadPrimaryKey(reader, OrdinalLookup, discription);
        }

        public object ReadPrimaryKey(System.Data.IDataReader reader, IDictionary<string, int> ordinalMapping, EntityDescription discription)
        {
            var keyField = discription.Fields.PrimaryKeyField;

            if (keyField != null && ReadField(reader, OrdinalLookup, keyField, out var value))
            { return value; }
            else
            { return null; }
        }

        private static bool ReadField(IDataReader reader, IDictionary<string, int> ordinalMapping, FieldInfo field, out object value)
        {
            try
            {
                var fieldName = (field.Alias ?? field.Name).ToLower(System.Globalization.CultureInfo.InvariantCulture);

                if (ordinalMapping.TryGetValue(fieldName, out var ordinal))
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