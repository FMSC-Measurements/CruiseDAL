using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.ModelGenerator
{
    public class PlantUMLGenerator : IGenerator
    {
        public void GenerateFiles(ISchemaInfoProvider provider, string @namespace, string directory, IEnumerable<string> nonPersistedColumns)
        {
            var fileName = "PlantERD.txt";
            var path = Path.Combine(directory, fileName);

            var sb = new StringBuilder();
            sb.AppendLine("@startuml");

            foreach(var tbl in provider.Tables)
            {
                sb.AppendLine(GenerateEntity(tbl));
            }

            foreach(var tbl in provider.Tables)
            {
                sb.AppendLine();
                foreach(var fk in tbl.ForeignKeys)
                {
                    sb.AppendLine($"{tbl.TableName} }}o--o| {fk.Table}");
                }
            }

            sb.AppendLine("@enduml");

            File.WriteAllText(path, sb.ToString());
        }

        public static string GenerateEntity(TableInfo tableInfo)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"entity \"{tableInfo.TableName}\" {{");

            var pk = tableInfo.PrimaryKeyField;
            if (pk != null)
            {
                sb.AppendLine($"* {pk.FieldName}");
                sb.AppendLine("--");
            }

            foreach (var f in tableInfo.Fields)
            {
                if (f.IsPK) { continue; }
                sb.Append($"    {f.FieldName} : {f.DbType}");
                if(f.IsFK)
                { sb.Append(" <<FK>>"); }
                sb.AppendLine();
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
