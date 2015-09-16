using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.SQLExpressionTypes
{
    public abstract class SQLExpression
    {
        public override bool Equals(object obj)
        {
            SQLExpression val = obj as SQLExpression;
            if (val == null)
            {
                return false;
            }

            return base.Equals(obj);
        }

    }
}
