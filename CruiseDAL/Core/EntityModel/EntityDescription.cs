using System;
using System.Reflection;
using System.Collections.Generic;

using System.Text;
using System.Data;
using System.Diagnostics;
using CruiseDAL.Core.EntityAttributes;

#if ANDROID
using Mono.Data.Sqlite;
using SQLiteCommand = Mono.Data.Sqlite.SqliteCommand;
using SQLiteParameter = Mono.Data.Sqlite.SqliteParameter;
#else
using System.Data.SQLite;
#endif


namespace CruiseDAL.Core.EntityModel
{
    public class EntityDescription
    {
        private SQLEntityAttribute _entityAttr;

        public Type EntityType { get; private set; }
        public String SourceName
        {
            get { return _entityAttr.SourceName; }
        }

        public FieldAttributeCollection Fields { get; set; }

        //public Dictionary<String, ReferenceAttribute> ReferenceFields { get; set; }

        public EntityCommandBuilder CommandBuilder { get; set; }
        public EntityInflator Inflator { get; set; }

        protected EntityDescription()
        {
            Fields = new FieldAttributeCollection();
        }


        public EntityDescription(Type type, DbProviderFactoryAdapter providerFactory) : this()
        {
            EntityType = type;

            this.Inflator = new EntityInflator(this);
            this.CommandBuilder = new EntityCommandBuilder(this, providerFactory);
        }

        protected void Initialize()
        {
            try
            {
                //read Entity attribute
                object[] tAttrs = EntityType.GetCustomAttributes(typeof(SQLEntityAttribute), true);
                _entityAttr = (SQLEntityAttribute)tAttrs[0];

                //find public properties
                foreach (PropertyInfo p in EntityType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    FieldAttribute fieldAttr = (FieldAttribute)Attribute.GetCustomAttribute(p, typeof(FieldAttribute));
                    if (fieldAttr != null)
                    {
                        Fields.AddField(p, fieldAttr);
                    }
                    else
                    {
                        Fields.AddField(p);
                    }
                }

                //find private properties
                foreach (PropertyInfo p in EntityType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    FieldAttribute fieldAttr = (FieldAttribute)Attribute.GetCustomAttribute(p, typeof(FieldAttribute));
                    if (fieldAttr != null)
                    {
                        Fields.AddField(p, fieldAttr);
                    }
                    else
                    {
                        Fields.AddField(p);
                    }
                }
            }
            catch (Exception e)
            {
                throw new ORMException("Unable to create EntityDescription for " + EntityType.Name, e);
            }
        }
    }
}