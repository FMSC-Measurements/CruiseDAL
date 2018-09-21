using System;

namespace FMSC.ORM.EntityModel.Attributes
{
    public class ReferenceAttribute
    {
        public Type EntityType { get; set; }
        public String ReferenceSourceName { get; set; }
        public String ForeignKeyFieldName { get; set; }
    }
}