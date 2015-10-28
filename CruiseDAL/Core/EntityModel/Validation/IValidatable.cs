using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Core.EntityModel
{
    public interface IValidatable
    {
        void AddError(string fieldName, string message);
        void RemoveError(string fieldName, string message);
    }
}
