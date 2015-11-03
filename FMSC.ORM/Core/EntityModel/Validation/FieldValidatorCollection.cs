using System;
using System.Collections.Generic;

namespace FMSC.ORM.Core.EntityModel
{
    public class FieldValidatorCollection
    {
        private Dictionary<string, IFieldValidator> lookup { get; set; }

        public FieldValidatorCollection()
        {
            lookup = new Dictionary<string, IFieldValidator>();
        }

        public void Add(IFieldValidator fv)
        {
            var fieldName = fv.Field;
            if (!HasConstraint(fieldName)) { lookup.Add(fieldName, fv); }
            if (object.ReferenceEquals(lookup[fieldName], fv)) { return; }

            lookup[fieldName] = fv;
        }


        public IFieldValidator this[string fieldName]
        {
            get
            {
                if (HasConstraint(fieldName) == false) { return null; }
                return lookup[fieldName];
            }
            set
            {
                lookup.Add(fieldName, value);
            }
        }

        public bool HasConstraint(String fieldName)
        {
            return lookup.ContainsKey(fieldName);
        }


    }
}
