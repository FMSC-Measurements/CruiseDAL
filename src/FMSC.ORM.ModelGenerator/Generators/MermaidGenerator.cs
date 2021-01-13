using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.ModelGenerator.Generators
{
    public class MermaidGenerator : IGenerator
    {

        public void GenerateFiles(ISchemaInfoProvider provider, string @namespace, string directory, IEnumerable<string> nonPersistedColumns)
        {
            var tabIndex = 0;
            var fileName = "Mermaid.txt";
            var path = Path.Combine(directory, fileName);

            var sb = new StringBuilder();
            sb.AppendLine("classDiagram");
            tabIndex++;

            foreach (var tbl in provider.Tables)
            {
                sb.AppendLine(GenerateEntity(tbl, tabIndex));
            }

            foreach (var tbl in provider.Tables)
            {
                sb.AppendLine();
                foreach (var fk in tbl.ForeignKeys)
                {
                    sb.TabAppendLine($"{fk.Table} <|-- {tbl.TableName}", tabIndex);
                }
            }

            File.WriteAllText(path, sb.ToString());
        }

        public static string GenerateEntity(TableInfo tableInfo, int tabIndex)
        {
            var sb = new StringBuilder();
            sb.TabAppendLine($"class {tableInfo.TableName} {{", tabIndex);

            var pk = tableInfo.PrimaryKeyField;
            if (pk != null)
            {
                sb.TabAppendLine($"* {pk.FieldName}", tabIndex);
                sb.TabAppendLine("-----", tabIndex);
            }

            tabIndex++;
            foreach (var f in tableInfo.Fields)
            {
                if (f.IsPK) { continue; }
                sb.TabAppend($"- {f.FieldName} : {f.DbType}", tabIndex);
                if (f.IsFK)
                { sb.Append(" <<FK>>"); }
                sb.AppendLine();
            }
            tabIndex--;

            sb.TabAppendLine("}", tabIndex);

            return sb.ToString();
        }

    }
}
