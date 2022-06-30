using CruiseDAL;
using CruiseDAL.UpConvert;
using System;
using System.IO;
using System.Linq;

namespace CruiseCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(string.Join(",", args));

            try
            {
                if (args.Length > 0)
                {
                    var convertArg = args[0];
                    if (File.Exists(convertArg) == false)
                    { throw new FileNotFoundException("invalid file path", convertArg); }

                    var filePath = Path.GetFullPath(convertArg);
                    var directory = Path.GetDirectoryName(filePath);
                    var fileName = Path.GetFileNameWithoutExtension(filePath);
                    var extention = Path.GetExtension(filePath).ToLower();

                    if (extention == ".cruise" || extention == ".cut")
                    {
                        var v3Path = Path.Combine(directory, fileName + ".crz3");
                        Console.WriteLine($"Converting {filePath} --> {v3Path}");

                        using (var v2Db = new DAL(filePath))
                        using (var v3Db = new CruiseDatastore_V3())
                        {
                            new Migrator().MigrateFromV2ToV3(v2Db, v3Db, deviceID: "CruiseCLI");
                            v3Db.BackupDatabase(v3Path);
                        }
                    }
                    else if (extention == ".crz3")
                    {
                        var v2Path = Path.Combine(directory, fileName + ".cruise");
                        Console.WriteLine($"Converting {filePath} --> {v2Path}");

                        using (var v2Db = new DAL())
                        using (var v3Db = new CruiseDatastore_V3(filePath))
                        {
                            var cruiseIDs = v3Db.QueryScalar<string>("SELECT CruiseID FROM Cruise;").ToArray();
                            
                            if(cruiseIDs.Length > 1)
                            { throw new InvalidOperationException("More than one cruise found"); }
                            var cruiseID = cruiseIDs.Single();

                            var downMigrator = new DownMigrator();

                            downMigrator.MigrateFromV3ToV2(cruiseID, v3Db, v2Db, createdBy: "CruiseCLI");
                            v2Db.BackupDatabase(v2Path);
                        }
                    }

                }

                Console.WriteLine("Done");
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

            if(Environment.UserInteractive)
            {
                Console.WriteLine("Hit Enter To Close Window");
                Console.ReadLine();
            }
        }
    }
}
