using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.ModelGenerator.Generators
{
    public class CSVGenerator : IGenerator
    {
        public void GenerateFiles(ISchemaInfoProvider provider, string @namespace, string directory, IEnumerable<string> nonPersistedColumns)
        {

            var tables = provider.Tables;
            foreach (var table in tables)
            {
                if (table.TableName.EndsWith("_Tombstone")) { continue; }
                if (table.Type == TableType.View) { continue; }

                

                var ddDir = Path.Combine(directory, "DataDictionary");
                if (!Directory.Exists(ddDir)) { Directory.CreateDirectory(ddDir); }

                var fileName = table.TableName + ".csv";
                var path = Path.Combine(ddDir, fileName);

                using var writer = new StreamWriter(path);
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);


                csv.WriteHeader<MyFieldInfo>();
                csv.NextRecord();

                

                csv.WriteRecords(table.Fields.Select(x => 
                new MyFieldInfo
                {
                    Table = table.TableName,
                    FieldName = x.FieldName,
                    IsPK = x.IsPK ? "X" : "",
                    IsFK = x.IsFK ? "X" : "",
                    References = x.References,
                    NotNull = x.NotNull ? "X" : "",
                    DbType = x.DbType,
                }));
            }
        }

        public class MyFieldInfo
        {
            public string Table { get; set; }

            public string FieldName { get; set; }

            public string DbType { get; set; }

            public string IsPK { get; set; }

            public string IsFK { get; set; }

            public string References { get; set; }

            public string NotNull { get; set; }

            public string Description { get; set; }

            public string Min { get; set; }

            public string Max { get; set; }

            public string RegionsUsing { get; set; }

            public string AdditionalRemarks { get; set; }
        }
    }
}
