using Backpack.SqlBuilder;
using Backpack.SqlBuilder.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMSC.ORM.ModelGenerator
{
    public class CSModelGenerator : IGenerator
    {
        public static ISqlDialect Dialect { get; } = new SqliteDialect();
        private int TAB_SIZE = 4;

        public void GenerateFiles(ISchemaInfoProvider provider, string @namespace, string directory, IEnumerable<string> nonPersistedColumns)
        {
            foreach (var tableInfo in provider.Tables)
            {
                GenerateFile(tableInfo, @namespace, directory, nonPersistedColumns);
            }
        }

        public static void GenerateFile(TableInfo tableInfo, string @namespace, string directory, IEnumerable<string> nonPersistedColumns)
        {
            var fileContent = GenerateFileContent(tableInfo, @namespace, nonPersistedColumns);

            var fileName = tableInfo.TableName + ".cs";

            var filePath = System.IO.Path.Combine(directory, fileName);

            System.IO.File.WriteAllText(filePath, fileContent);
        }

        public static string GenerateFileContent(TableInfo tableInfo, string @namespace, IEnumerable<string> nonPersistedColumns)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using FMSC.ORM.EntityModel.Attributes;");

            sb.AppendLine($"namespace {@namespace}");
            sb.AppendLine("{");

            sb.AppendLine(GenerateClass(tableInfo, tabIndex: 1, nonPersistedColumns: nonPersistedColumns));

            sb.AppendLine("}");

            return sb.ToString();
        }

        public static string GenerateClass(TableInfo tableInfo,
            int tabIndex = 0,
            string tableAttr = "Table",
            string fieldAttr = "Field",
            string primaryKeyAttr = "PrimaryKeyField",
            IEnumerable<string> nonPersistedColumns = null)
        {
            nonPersistedColumns ??= Enumerable.Empty<string>();
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
                var fieldName = fi.FieldName;
                var isNonPersisted = nonPersistedColumns.Contains(fieldName, StringComparer.InvariantCultureIgnoreCase);
                var persistanceFlags = isNonPersisted ? ", PersistanceFlags = PersistanceFlags.Never" : "";
                var attr = (fi.IsPK) ?
                    $"[{primaryKeyAttr}(\"{fi.FieldName}\"{persistanceFlags})]" 
                    : $"[{fieldAttr}(\"{fi.FieldName}\"{persistanceFlags})]";

                sb.Append(Tab(tabIndex)).AppendLine(attr);

                var runtypeType = DbTypeToRuntimeType(fi.DbType);
                var typeName = runtypeType.Name;

                if(runtypeType.IsValueType && fi.NotNull == false)
                { typeName = typeName + "?"; }

                sb.Append(Tab(tabIndex)).AppendLine($"public {typeName} {fi.FieldName} {{ get; set; }}");
                sb.AppendLine();
            }

            tabIndex = tabIndex - 1;

            sb.Append(Tab(tabIndex)).AppendLine("}");

            return sb.ToString();
        }

        private static string Tab(int tabCount)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < tabCount; i++)
            {
                sb.Append(" ");
            }
            return sb.ToString();
        }

        public static Type DbTypeToRuntimeType(string dbType)
        {
            return Dialect.Try(
                                y => y.MapSQLtypeToSystemType(dbType),
                                (z, e) => { Console.WriteLine($"{dbType}| {e.Message}"); return typeof(string); });
        }
    }

    
}