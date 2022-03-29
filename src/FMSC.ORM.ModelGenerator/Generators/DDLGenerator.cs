using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.ModelGenerator.Generators
{
    public class DDLGenerator : IGenerator
    {
        public void GenerateFiles(ISchemaInfoProvider provider, string @namespace, string directory, IEnumerable<string> nonPersistedColumns)
        {
            var fileName = "DDL.txt";
            var path = Path.Combine(directory, fileName);

            var sb = new StringBuilder();
            

            foreach (var tbl in provider.Tables)
            {
                sb.AppendLine($"-- ************** {tbl.TableName} **************");

                sb.AppendLine(tbl.DDL);
            }

            File.WriteAllText(path, sb.ToString());
        }
    }
}
