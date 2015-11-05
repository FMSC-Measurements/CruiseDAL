using System;
using System.Reflection;
using System.Collections.Generic;

using System.Text;
using System.Data;
using System.Diagnostics;
using FMSC.ORM.Core.EntityAttributes;

#if ANDROID
using Mono.Data.Sqlite;
using SQLiteCommand = Mono.Data.Sqlite.SqliteCommand;
using SQLiteParameter = Mono.Data.Sqlite.SqliteParameter;
#else
using System.Data.SQLite;
#endif


namespace FMSC.ORM.Core.EntityModel
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


        public EntityDescription(Type type) : this()
        {
            EntityType = type;
            Initialize();

            this.Inflator = new EntityInflator(this);
            this.CommandBuilder = new EntityCommandBuilder(this);
        }

        protected void Initialize()
        {
            try
            {
                //read Entity attribute
                object[] tAttrs = EntityType.GetCustomAttributes(typeof(SQLEntityAttribute), true);
                _entityAttr = (SQLEntityAttribute)tAttrs[0];

                RegesterFields();
            }
            catch (Exception e)
            {
                throw new ORMException("Unable to initialize EntityDescription for " + EntityType.Name, e);
            }
        }

        protected void RegesterFields()
        {
            //find public properties
            foreach (PropertyInfo p in EntityType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                RegesterPublicProperty(p);
            }

            //find private properties
            foreach (PropertyInfo p in EntityType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                RegesterNonPublicProperty(p);
            }
        }

        protected void RegesterPublicProperty(PropertyInfo property)
        {
            BaseFieldAttribute fieldAttr = (BaseFieldAttribute)Attribute.GetCustomAttribute(property, typeof(BaseFieldAttribute));
            try
            {
                if (fieldAttr == null)
                {
                    //TODO handle public property without attribute if we want automatic fields
                    //catch FieldAccesabilityException for automatic fields
                    return;
                }
                if (fieldAttr is IgnoreFieldAttribute) { return; }
                if (fieldAttr is FieldAttribute)
                {
                    Fields.AddField(property, (FieldAttribute)fieldAttr);
                }
            }
            catch(Exception e)
            {
                throw new ORMException("Unable to register property: " + property.Name, e);
            }
        }

        protected void RegesterNonPublicProperty(PropertyInfo property)
        {
            BaseFieldAttribute fieldAttr = (BaseFieldAttribute)Attribute.GetCustomAttribute(property, typeof(BaseFieldAttribute));
            try
            {
                if (fieldAttr == null) { return; } //don't allow non public properties to be automatic fields
                if (fieldAttr is IgnoreFieldAttribute) { return; }
                if (fieldAttr is FieldAttribute)
                {
                    Fields.AddField(property, (FieldAttribute)fieldAttr);
                }
            }
            catch (Exception e)
            {
                throw new ORMException("Unable to register property: " + property.Name, e);
            }
        }
    }
}