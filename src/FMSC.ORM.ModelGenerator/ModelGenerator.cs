using System;
using System.Text;

namespace FMSC.ORM.ModelGenerator
{
    public class ModelGenerator
    {
        private int TAB_SIZE = 4;

        public void GenerateFiles(ISchemaInfoProvider provider, string @namespace, string directory)
        {
            foreach (var tableInfo in provider.Tables)
            {
                GenerateFile(tableInfo, @namespace, directory);
            }
        }

        public void GenerateFile(TableInfo tableInfo, string @namespace, string directory)
        {
            var fileContent = GenerateFileContent(tableInfo, @namespace);

            var fileName = tableInfo.TableName + ".cs";

            var filePath = System.IO.Path.Combine(directory, fileName);

            System.IO.File.WriteAllText(filePath, fileContent);
        }

        public string GenerateFileContent(TableInfo tableInfo, string @namespace)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using FMSC.ORM.EntityModel.Attributes;");

            sb.AppendLine($"namespace {@namespace}");
            sb.AppendLine("{");

            sb.AppendLine(GenerateClass(tableInfo, 1));

            sb.AppendLine("}");

            return sb.ToString();
        }

        public string GenerateClass(TableInfo tableInfo,
            int tabIndex = 0,
            string tableAttr = "EntitySource",
            string fieldAttr = "Field",
            string primaryKeyAttr = "PrimaryKeyField")
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(tableAttr))
            {
                sb.Append(Tab(tabIndex)).AppendLine($"[{tableAttr}(\"{tableInfo.TableName}\")]");
            }
            sb.Append(Tab(tabIndex)).AppendLine($"public partial class {tableInfo.TableName}");
            sb.Append(Tab(tabIndex)).AppendLine("{");
            tabIndex = tabIndex + 1;

            foreach (var fi in tableInfo.Fields)
            {
                var attr = (fi.IsPK) ?
                    $"[{primaryKeyAttr}(\"{fi.FieldName}\")]" 
                    : $"[{fieldAttr}(\"{fi.FieldName}\")]";

                sb.Append(Tab(tabIndex)).AppendLine(attr);

                var typeName = fi.RuntimeTimeType.Name;

                if(fi.RuntimeTimeType.IsValueType && fi.NotNull == false)
                { typeName = typeName + "?"; }

                sb.Append(Tab(tabIndex)).AppendLine($"public {typeName} {fi.FieldName} {{ get; set; }}");
                sb.AppendLine();
            }

            tabIndex = tabIndex - 1;

            sb.Append(Tab(tabIndex)).AppendLine("}");

            return sb.ToString();
        }

        private string Tab(int tabCount)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < tabCount; i++)
            {
                sb.Append(" ");
            }
            return sb.ToString();
        }
    }
}