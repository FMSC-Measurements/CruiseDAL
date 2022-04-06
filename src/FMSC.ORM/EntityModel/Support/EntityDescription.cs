using Backpack.SqlBuilder;
using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Linq;

namespace FMSC.ORM.EntityModel.Support
{
    public class EntityDescription
    {
        public Type EntityType { get; }

        public String SourceName => Source?.SourceName;

        public TableOrSubQuery Source { get; set; }
        public FieldInfoCollection Fields { get; set; } = new FieldInfoCollection();

        public EntityDescription(Type type)
        {
            EntityType = type;
            Fields = new FieldInfoCollection(type);
            Source = InitializeSource(type);
        }

        protected static TableOrSubQuery InitializeSource(Type type)
        {
            try
            {
                //read Entity attribute
                var eAttr = type.GetCustomAttributes(typeof(TableAttribute), true)
                    .OfType<TableAttribute>()
                    .FirstOrDefault();

                if (eAttr is null)
                {
                    return new TableOrSubQuery(type.Name);
                }
                else
                {
                    var tableName = eAttr.Name ?? type.Name;
                    var source = new TableOrSubQuery(tableName);

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

                    return source;
                }
            }
            catch (Exception e)
            {
                throw new ORMException("Unable to initialize EntityDescription for " + type.Name, e);
            }
        }
    }
}