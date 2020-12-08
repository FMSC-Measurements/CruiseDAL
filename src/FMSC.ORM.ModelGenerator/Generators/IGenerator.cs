using System.Collections.Generic;

namespace FMSC.ORM.ModelGenerator.Generators
{
    public interface IGenerator
    {
        void GenerateFiles(ISchemaInfoProvider provider, string @namespace, string directory, IEnumerable<string> nonPersistedColumns);
    }
}