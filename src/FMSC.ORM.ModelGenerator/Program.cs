using CommandLine;
using FMSC.ORM.Core;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace FMSC.ORM.ModelGenerator
{
    public class Program
    {
        class Options
        {
            [Option(HelpText = "use datastore to get schemo info")]
            public bool ds { get; set; }

            [Option(HelpText = "Path to assembly that will be used to generate models from")]
            public string TargetAssembly { get; set; }

            [Option(HelpText = "Name of datastore class")]
            public string TypeName { get; set; }

            //[Option(HelpText = "parameters to pass to ", Default = (string[])null)]
            public string[] Params { get; set; } = (string[])null;

            [Option(HelpText = "Root namespace of the generated files ", Default = "MyNamespace")]
            public string Namespace { get; set; }

            [Option(HelpText = "output directory")]
            public string OutputDirectory { get; set; }

            [Option(HelpText = "column seperated list of columns to ignore")]
            public string IgnoreColumns { get; set; }
        }

        static void Main(string[] args)
        {
            //Console.WriteLine($"Args: {String.Join(", ", args)}");

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    if (o.ds)
                    {
                        if (o.TargetAssembly != null)
                        {
                            var datastore = CreateDatastore(o.TargetAssembly, o.TypeName, o.Params);

                            var ignoreColmns = (string.IsNullOrWhiteSpace(o.IgnoreColumns) == false) ?
                                o.IgnoreColumns.Split(',')
                                : (string[])null;

                            var schemaProvider = new SqliteDatastoreSchemaInfoProvider(datastore, ignoreColmns);

                            var modelGenerator = new ModelGenerator();
                            modelGenerator.GenerateFiles(schemaProvider, o.Namespace, o.OutputDirectory);
                        }
                    }
                });
        }

        public static Datastore CreateDatastore(string targetAssembly, string typeName, string[] parameters)
        {
            parameters = parameters ?? new string[0];
            var assembly = Assembly.LoadFrom(targetAssembly);
            var datastoreType = assembly.GetType(typeName);

            //Console.WriteLine($"resolved datastore type {typeName} as {datastoreType?.FullName ?? "null"}");

            var constructors = datastoreType.GetConstructors();

            var constructor = constructors
            .Where(c => c.GetParameters().Count() == parameters.Count())
            .Single();

            var datastore = (Datastore)constructor.Invoke(parameters);
            return datastore;
        }
    }
}
