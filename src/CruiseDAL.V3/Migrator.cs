using CruiseDAL.Migrators;
using FMSC.ORM;
using FMSC.ORM.Core;
using FMSC.ORM.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;

namespace CruiseDAL
{
    public static class Migrator
    {
        public static readonly IEnumerable<IMigrator> MIGRATORS = new IMigrator[]
        {
            new SaleMigrator(),
            new CruiseMigrator(),
            new CuttingUnitMigrator(),
            new StratumMigrator(),
            new CuttingUnit_StratumMigrator(),
            new SampleGroupMigrator(),
            new PlotMigrator(),
            new Plot_StratumMigrator(),
            new SpeciesMigrator(),
            new SubPopulationMigrator(),
            new TreeDefaultValueMigrator(),
            new FixCNTTallyPopulationMigrator(),
            new TreeMigrator(),
            new TreeMeasurmentsMigrator(),
            new LogMigrator(),
            new TallyLedgerMigrator(),
            new LogFieldSetupMigrator(),
            new TreeFieldSetupMigrator(),
            new TallyDescriptionMigrator(),
            new TallyHotkeyMigrator(),
            new LogGradeAuditRuleMigrator(),
            new TreeAuditRuleMigrator(),
            new TreeAuditRuleSelectorMigrator(),
            new ReportsMigrator(),

            new GlobalsMigrator(),
            new MessageLogMigrator(),
        };

        public static ILogger Logger { get; set; } = LoggerProvider.Get();

        public static string GetConvertedPath(string v2Path)
        {
            // get path, directory and filename of original file
            var fileDirectory = System.IO.Path.GetDirectoryName(v2Path);

            // create path for new temp file
            var newFileName = GetConvertedFileName(v2Path);
            var newFilePath = System.IO.Path.Combine(fileDirectory, newFileName);

            return newFilePath;
        }

        public static string GetConvertedFileName(string v2Path)
        {
            var fileName = System.IO.Path.GetFileNameWithoutExtension(v2Path);
            var newFileName = fileName + ".crz3";
            return newFileName;
        }

        public static string MigrateFromV2ToV3(string v2Path, bool overwrite = false)
        {
            var newFilePath = GetConvertedPath(v2Path);
            var newFileName = System.IO.Path.GetFileName(newFilePath);

            // check temp file doesn't already exist, otherwise throw exception
            var fileAlreadyExists = File.Exists(newFilePath);
            if (fileAlreadyExists && !overwrite) { throw new UpdateException(newFileName + " already exists"); }

            MigrateFromV2ToV3(v2Path, newFilePath);

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
                MigrateFromV2ToV3(connection, oldDbAlias, v3db.ExceptionProcessor);
            }
            finally
            {
                v3db.DetachDB(oldDbAlias);
                v3db.ReleaseConnection();
            }
        }

        public static void MigrateFromV2ToV3(DbConnection connection, string from = "v2", IExceptionProcessor exceptionProcessor = null)
        {
            var to = "main";

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();
            //using (var transaction = connection.BeginTransaction())
            //{
            //    try
            //    {
            //        var migrators = MIGRATORS;
            //        foreach (var migrator in migrators)
            //        {
            //            var command = migrator.MigrateToV3(to, from, cruiseID, saleID);
            //            connection.ExecuteNonQuery(command, transaction: transaction, exceptionProcessor: exceptionProcessor);
            //        }

            //        transaction.Commit();
            //    }
            //    catch
            //    {
            //        transaction.Rollback();
            //        throw;
            //    }
            //}

            DbTransaction transaction = null;

            var migrators = MIGRATORS;
            foreach (var migrator in migrators)
            {
                var command = migrator.MigrateToV3(to, from, cruiseID, saleID);
                connection.ExecuteNonQuery(command, transaction: transaction, exceptionProcessor: exceptionProcessor);
            }
        }
    }
}