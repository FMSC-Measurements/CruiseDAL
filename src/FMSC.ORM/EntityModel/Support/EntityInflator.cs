using FMSC.ORM.Core;
using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Data;

namespace FMSC.ORM.EntityModel.Support
{
    public class EntityInflator
    {
        private const int BYTE_READ_LENGTH = 1024;//TODO once we start reading byte data figure out our byte read length... should this be done at runtime?

        //ConstructorInfo _constructor;

        private Dictionary<string, int> OrdinalMapping { get; set; }

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

            for (int i = 0; i < fieldCount; i++)
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

        public void ReadData(System.Data.IDataReader reader, Object obj)
        {
            var ordinalMapping = OrdinalMapping;
            foreach (FieldAttribute field in EntityDescription.Fields)
            {
                if (ReadField(reader, ordinalMapping, field, out var value))
                {
                    field.SetFieldValue(obj, value);
                }
            }

            PrimaryKeyFieldAttribute keyField = EntityDescription.Fields.PrimaryKeyField;
            if (keyField != null)
            {
                if (ReadField(reader, ordinalMapping, keyField, out var value))
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

            if (keyField != null && ReadField(reader, OrdinalMapping, keyField, out var value))
            { return value; }
            else
            { return null; }
        }

        private static bool ReadField(IDataReader reader, Dictionary<string, int> ordinalMapping, FieldAttribute field, out object value)
        {
            try
            {
                var fieldName = field.NameOrAlias.ToLowerInvariant();
                int ordinal = -1;

                if (ordinalMapping.TryGetValue(fieldName, out ordinal))
                {
                    value = ValueMapper.ProcessValue(field.RunTimeType, reader.GetValue(ordinal));
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