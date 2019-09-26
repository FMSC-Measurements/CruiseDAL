using Backpack.SqlBuilder;
using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FMSC.ORM.EntityModel.Support
{
    public class EntityDescription
    {
        public Type EntityType { get; private set; }

        public String SourceName
        {
            get { return (Source != null) ? Source.SourceName : null; }
        }

        public TableOrSubQuery Source { get; set; }

        public FieldInfoCollection Fields { get; set; }

        public Dictionary<string, PropertyAccessor> Properties { get; set; }

        //public Dictionary<String, ReferenceAttribute> ReferenceFields { get; set; }

        public EntityCommandBuilder CommandBuilder { get; set; }

        protected EntityDescription()
        {
            Fields = new FieldInfoCollection();
            //Properties = new Dictionary<string, PropertyAccessor>();
        }

        public EntityDescription(Type type) : this()
        {
            EntityType = type;
            Initialize();

            this.CommandBuilder = new EntityCommandBuilder(this);
        }

        protected void Initialize()
        {
            try
            {
                //read Entity attribute
                var eAttr = EntityType.GetCustomAttributes(typeof(TableAttribute), true)
                    .OfType<TableAttribute>()
                    .FirstOrDefault();

                if (eAttr != null)
                {
                    var source = Source = new TableOrSubQuery(eAttr.Name);

                    // to provide backwards compatibility 
                    // check if it is a EntitySourceAttr
#pragma warning disable CS0618 // Type or member is obsolete
                    if (eAttr is EntitySourceAttribute)

                    {
                        var esa = (EntitySourceAttribute)eAttr;
                        source.Alias = esa.Alias;
                        source.Joins = esa.JoinCommands;
                    }
#pragma warning restore CS0618 // Type or member is obsolete
                }
                else
                {
                    Source = new TableOrSubQuery(EntityType.Name);
                }

                Fields = CreateFieldInfoCollection(EntityType);
            }
            catch (Exception e)
            {
                throw new ORMException("Unable to initialize EntityDescription for " + EntityType.Name, e);
            }
        }

        public static FieldInfoCollection CreateFieldInfoCollection(Type entityType)
        {
            var fields = new FieldInfoCollection();

            //find public properties
            foreach (PropertyInfo p in entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if(p.CanWrite == false) { continue; }

                var propType = p.PropertyType;
                propType = Nullable.GetUnderlyingType(propType) ?? propType;
                var typeCode = Type.GetTypeCode(propType);
                if(typeCode == TypeCode.Object && propType != typeof(Guid)) { continue; }

                var attr = Attribute.GetCustomAttribute(p, typeof(BaseFieldAttribute))
                    as BaseFieldAttribute;

                if (attr == null)
                {
                    var fieldInfo = new FieldInfo(new PropertyAccessor(p))
                    {
                        Name = p.Name,
                    };
                    fields.AddField(fieldInfo);
                }
                else if (attr is FieldAttribute)
                {
                    var fieldInfo = new FieldInfo((FieldAttribute)attr, new PropertyAccessor(p));
                    fields.AddField(fieldInfo);
                }
            }

            return fields;
        }
    }
}