using FMSC.ORM;
using FMSC.ORM.Core;
using System;
using System.Data.Common;
using System.IO;

namespace CruiseDAL
{
    public class Migrator
    {
        public static string GetConvertedPath(string v2Path)
        {
            // get path, directory and filename of original file
            var fileDirectory = System.IO.Path.GetDirectoryName(v2Path);
            var fileName = System.IO.Path.GetFileNameWithoutExtension(v2Path);

            // create path for new temp file
            var newFileName = fileName + ".crz3";
            var newFilePath = System.IO.Path.Combine(fileDirectory, newFileName);

            return newFilePath;
        }

        public static string MigrateFromV2ToV3(string v2Path, bool overwrite = false)
        {
            var newFilePath = GetConvertedPath(v2Path);
            var newFileName = System.IO.Path.GetFileName(newFilePath);

            // check temp file doesn't already exist, otherwise throw exception
            var fileAlreadyExists = File.Exists(newFilePath);
            if (fileAlreadyExists && !overwrite) { throw new UpdateException(newFileName + " already exists"); }

            

            return newFilePath;
        }

        public static void MigrateFromV2ToV3(string v2Path, string newFilePath)
        {
            using (var v2Cruise = new CruiseDatastore(v2Path, false, null, new Updater_V2()))
            using (var newCruise = new CruiseDatastore_V3(newFilePath, true))
            {
                MigrateFromV2ToV3(v2Cruise, newCruise);
            }
        }

        public static void MigrateFromV2ToV3(CruiseDatastore v2db, CruiseDatastore_V3 v3db)
        {
            var oldDbAlias = "v2";
            v3db.AttachDB(v2db, oldDbAlias);

            try
            {
                var connection = v3db.OpenConnection();
                MigrateFromV2ToV3(connection, oldDbAlias);
            }
            finally
            {
                v3db.ReleaseConnection();
            }
        }

        public static void MigrateFromV2ToV3(DbConnection connection, string from)
        {
            var to = "main";
            using (var transaction = connection.BeginTransaction())
            {
                foreach (var command in Schema.Migrations.GetMigrateCommands(to, from))
                {
                    try
                    {
                        connection.ExecuteNonQuery(command, (object[])null, transaction);
                    }
                    catch (Exception e)
                    {
                        throw;
                    }
                }

                transaction.Commit();
            }
        }
    }
}