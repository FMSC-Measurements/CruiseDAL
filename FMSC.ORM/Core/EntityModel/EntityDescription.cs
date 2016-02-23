using System;
using System.Reflection;
using System.Collections.Generic;

using System.Text;
using System.Data;
using System.Diagnostics;
using FMSC.ORM.Core.EntityAttributes;
using FMSC.ORM.Core.SQL;
using System.Linq;

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
        private EntitySourceAttribute _entityAttr;

        public Type EntityType { get; private set; }
        public String SourceName
        {
            get { return Source.SourceName; }
        }

        public SelectSource Source { get; set; }

        public FieldAttributeCollection Fields { get; set; }

        public Dictionary<string, PropertyAccessor> Properties { get; set; }

        //public Dictionary<String, ReferenceAttribute> ReferenceFields { get; set; }

        public EntityCommandBuilder CommandBuilder { get; set; }
        public EntityInflator Inflator { get; set; }

        protected EntityDescription()
        {
            Fields = new FieldAttributeCollection();
            Properties = new Dictionary<string, PropertyAccessor>();
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
                object[] eAttrs = EntityType.GetCustomAttributes(typeof(EntitySourceAttribute), true);
                var eAttr = eAttrs.FirstOrDefault() as EntitySourceAttribute;


                if(eAttr != null)
                {
                    this.Source = new TableOrSubQuery(
                        eAttr.SourceName
                        , eAttr.Alias)
                    {
                        JoinCommands = eAttr.JoinCommands
                    };
                }

                

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
                RegesterProperty(p, true);
            }

            //find private properties
            //foreach (PropertyInfo p in EntityType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance))
            //{
            //    RegesterProperty(p, false);
            //}
        }

        protected void RegesterProperty(PropertyInfo property, bool isPublic)
        {
            var accessor = new PropertyAccessor(property);

            if (this.Properties.ContainsKey(accessor.Name) == false)
            {
                this.Properties.Add(accessor.Name, accessor);
            }
            else
            {
                Debug.WriteLine("Property hidden:" + property.ToString());
                //Debug.Fail("property already registered: " + accessor.Name);
            }

            BaseFieldAttribute fieldAttr = Attribute.GetCustomAttribute(property, typeof(BaseFieldAttribute)) as BaseFieldAttribute;
            
            if (fieldAttr == null)
            {
                if (isPublic)
                {
                    //TODO handle public property without attribute if we want automatic fields
                    //catch FieldAccesabilityException for automatic fields
                    return;
                }
                else
                {
                    return; //don't allow non public properties to be automatic fields
                }
            }


            if (fieldAttr is IgnoreFieldAttribute) { return; }

            if (fieldAttr is FieldAttribute)
            {
                try
                {
                    ((FieldAttribute)fieldAttr).Property = accessor;
                    Fields.AddField((FieldAttribute)fieldAttr);
                }
                catch (Exception e)
                {
                    throw new ORMException("Unable to register property: " + property.Name, e);
                }
            }
        }

        //protected void RegesterNonPublicProperty(PropertyInfo property)
        //{
        //    BaseFieldAttribute fieldAttr = (BaseFieldAttribute)Attribute.GetCustomAttribute(property, typeof(BaseFieldAttribute));
        //    try
        //    {
        //        if (fieldAttr == null) { return; } //don't allow non public properties to be automatic fields
        //        if (fieldAttr is IgnoreFieldAttribute) { return; }
        //        if (fieldAttr is FieldAttribute)
        //        {
        //            Fields.AddField(property, (FieldAttribute)fieldAttr);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new ORMException("Unable to register property: " + property.Name, e);
        //    }
        //}
    }
}