using System;
using System.Collections.Generic;
using System.Text;


namespace FMSC.ORM.Core.EntityAttributes
{
    public class FieldAccesabilityException : ORMException
    {
        public FieldAccesabilityException(string fieldName, bool missingGet, bool missingSet)
            : base(String.Format("{0} missingGet:{1} missingSet{2}", fieldName, missingGet, missingSet))
        { }
    }
}
