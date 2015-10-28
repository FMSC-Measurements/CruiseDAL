using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Core.EntityModel
{
    public interface IFieldValidator
    {
        string Field { get; }

        string TableName { get; }

        string ErrorMessage { get; }

        ErrorLevel Level { get; }

        bool Validate(object sender, object value);
    }
}
