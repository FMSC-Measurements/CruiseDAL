using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.ModelGenerator
{
    public interface IGenerator
    {
        void GenerateFiles(ISchemaInfoProvider provider, string @namespace, string directory);
    }
}
