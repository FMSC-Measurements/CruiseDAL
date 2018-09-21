using System;

namespace FMSC.ORM.EntityModel.Attributes
{
    public class FieldAccesabilityException : ORMException
    {
        public FieldAccesabilityException(string fieldName, bool missingGet, bool missingSet)
            : base(String.Format("{0} missingGet:{1} missingSet{2}", fieldName, missingGet, missingSet))
        { }
    }
}