using System.Collections.Generic;

namespace FMSC.ORM.ModelGenerator
{
    public interface IGenerator
    {
        void GenerateFiles(ISchemaInfoProvider provider, string @namespace, string directory, IEnumerable<string> nonPersistedColumns);
    }
}