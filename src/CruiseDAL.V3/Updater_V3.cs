using CruiseDAL.Schema;
using CruiseDAL.Schema.Views;
using FMSC.ORM;
using FMSC.ORM.Core;
using FMSC.ORM.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace CruiseDAL
{
    public class Updater_V3 : IUpdater
    {
        public static ILogger Logger { get; set; } = LoggerProvider.Get();

        public void Update(CruiseDatastore datastore)
        {
            var version = datastore.DatabaseVersion;
            if (version == "3.0.0"
                || version == "3.0.1"
                || version == "3.0.2"
                || version == "3.0.3")
            {
                UpdateTo_3_1_0(datastore);
            }
            if (version == "3.1.0" || version == "3.2.0" || version == "3.2.1")
            {
                UpdateTo_3_2_2(datastore);
            }
            if (version == "3.2.2")
            {
                UpdateTo_3_2_3(datastore);
            }
            if (version == "3.2.3")
            {
                UpdateTo_3_2_4(datastore);
            }
            if (version == "3.2.4")
            {
                UpdateTo_3_3_0(datastore);
            }
            if (version == "3.3.0")
            {
                UpdateTo_3_3_1(datastore);
            }
            if (version == "3.3.2")
            {
                UpdateTo_3_3_2(datastore);
            }
        }

        private void UpdateTo_3_3_2(CruiseDatastore ds)
        {
            var curVersion = ds.DatabaseVersion;
            var targetVersion = "3.3.1";

            try
            {
                // create an in-memory database
                // to migrate into
                using (var newDatastore = new CruiseDatastore_V3())
                {
                    var excludeTables = new[]
                    {
                        "LogField",
                        "TreeField",
                        "Log",
                        "Stem",
                };

                    Migrate(ds, newDatastore, excluding: excludeTables, excludeLookupTables: true);

                    // use back up rutine to replace old database with
                    // migrated contents
                    newDatastore.BackupDatabase(ds);
                }
            }
            catch(Exception e)
            {
                throw new SchemaUpdateException(curVersion, targetVersion, e);
            }
        }

        // update 3.3.1 notes:
        // Add CruiseID fields to Log and Stem tables
        // Change is fully backwards compatible with prior versions
        private void UpdateTo_3_3_1(CruiseDatastore ds)
        {
            var curVersion = ds.DatabaseVersion;
            var targetVersion = "3.3.1";

            // create an in-memory database
            // to migrate into
            using (var newDatastore = new CruiseDatastore_V3())
            {
                var excludeTables = new[]
                {
                        "LogField",
                        "TreeField",
                        "Log",
                        "Stem",
                };

                Migrate(ds, newDatastore, excluding: excludeTables, excludeLookupTables: true);

                var destConn = newDatastore.OpenConnection();
                var sourceConn = ds.OpenConnection();

                try
                {
                    var to = "main"; // alias used by the source database
                    var from = "fromdb";

#if SYSTEM_DATA_SQLITE
                    var srcDataSource = ((System.Data.SQLite.SQLiteConnection)sourceConn).FileName;
#else
            var srcDataSource = sourceConn.DataSource;
#endif
                    destConn.ExecuteNonQuery($"ATTACH DATABASE \"{srcDataSource}\" AS {from};");

                    using (var transaction = destConn.BeginTransaction())
                    {
                        try
                        {
                            destConn.ExecuteNonQuery(
@$"INSERT INTO {to}.Log (
    Log_CN,
    CruiseID,
    LogID,
    TreeID,
    LogNumber,
    Grade,
    SeenDefect,
    PercentRecoverable,
    Length,
    ExportGrade,
    SmallEndDiameter,
    LargeEndDiameter,
    GrossBoardFoot,
    NetBoardFoot,
    GrossCubicFoot,
    NetCubicFoot,
    BoardFootRemoved,
    CubicFootRemoved,
    DIBClass,
    BarkThickness,
    CreatedBy,
    Created_TS,
    ModifiedBy,
    Modified_TS
)
SELECT
    l.Log_CN,
    t.CruiseID,
    l.LogID,
    l.TreeID,
    l.LogNumber,
    l.Grade,
    l.SeenDefect,
    l.PercentRecoverable,
    l.Length,
    l.ExportGrade,
    l.SmallEndDiameter,
    l.LargeEndDiameter,
    l.GrossBoardFoot,
    l.NetBoardFoot,
    l.GrossCubicFoot,
    l.NetCubicFoot,
    l.BoardFootRemoved,
    l.CubicFootRemoved,
    l.DIBClass,
    l.BarkThickness,
    l.CreatedBy,
    l.Created_TS,
    l.ModifiedBy,
    l.Modified_TS
FROM {from}.Log AS l
JOIN Tree AS t USING (TreeID);");

                            destConn.ExecuteNonQuery(
@$"INSERT INTO {to}.Stem (
    Stem_CN,
    StemID,
    CruiseID,
    TreeID,
    Diameter,
    DiameterType,
    CreatedBy,
    Created_TS,
    ModifiedBy,
    Modified_TS
)
SELECT
    s.Stem_CN,
    s.StemID,
    t.CruiseID,
    s.TreeID,
    s.Diameter,
    s.DiameterType,
    s.CreatedBy,
    s.Created_TS,
    s.ModifiedBy,
    s.Modified_TS
FROM {from}.Stem AS s
JOIN Tree AS t USING (TreeID);");

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
                finally
                {
                    newDatastore.ReleaseConnection();
                    ds.ReleaseConnection();
                }

                // use back up rutine to replace old database with
                // migrated contents
                newDatastore.BackupDatabase(ds);
            }
        }

        // update 3.3.0 notes
        // redesign Stratum Template tables. Remove existing StratumDefault, LogFieldSetupDefault, TreeFielSetupDefault tables
        // Replace with StratumTemplate and StratumTemplateTreeFieldSetup tables
        // Updated schema is not backwards compatible with previous schema,
        // but no application code relies on previous tables. Previous tables were only written to when
        // file was created.
        private void UpdateTo_3_3_0(CruiseDatastore ds)
        {
            var curVersion = ds.DatabaseVersion;
            var targetVersion = "3.3.0";
            ds.BeginTransaction();

            try
            {
                var cruiseIDs = ds.QueryScalar<string>("SELECT CruiseID FROM Cruise;");
                if (cruiseIDs.Count() == 1)
                {
                    var cruiseID = cruiseIDs.Single();

                    ds.Execute(new StratumTemplateTableDefinition().CreateTable);

                    ds.Execute(
    $@"INSERT INTO StratumTemplate (
    StratumTemplateName,
    CruiseID,
    StratumCode,
    Method,
    BasalAreaFactor,
    FixedPlotSize,
    KZ3PPNT,
    SamplingFrequency,
    Hotkey,
    FBSCode,
    YieldComponent,
    FixCNTField
) SELECT
    (CASE WHEN sd.Method NOT NULL THEN sd.Method || ' ' ELSE '' END) || (CASE WHEN sd.StratumCode NOT NULL THEN sd.StratumCode || ' ' ELSE '' END) || ifnull(Description, '') AS StratumTemplateName,
    '{cruiseID}' AS CruiseID,
    StratumCode,
    Method,
    BasalAreaFactor,
    FixedPlotSize,
    KZ3PPNT,
    SamplingFrequency,
    Hotkey,
    FBSCode,
    YieldComponent,
    FixCNTField
FROM StratumDefault AS sd;");

                    var sttfs = new StratumTemplateTreeFieldSetupTableDefinition();
                    ds.Execute(sttfs.CreateTable);
                    ds.Execute(sttfs.CreateIndexes);

                    ds.Execute(
    $@"INSERT INTO StratumTemplateTreeFieldSetup (
    StratumTemplateName,
    CruiseID,
    Field,
    FieldOrder,
    IsHidden,
    IsLocked,
    DefaultValueInt,
    DefaultValueReal,
    DefaultValueBool,
    DefaultValueText
) SELECT
    (CASE WHEN sd.Method NOT NULL THEN sd.Method || ' ' ELSE '' END) || (CASE WHEN sd.StratumCode NOT NULL THEN sd.StratumCode || ' ' ELSE '' END) || ifnull(Description, '') AS StratumTemplateName,
    '{cruiseID}' AS CruiseID,
    Field,
    FieldOrder,
    IsHidden,
    IsLocked,
    DefaultValueInt,
    DefaultValueReal,
    DefaultValueBool,
    DefaultValueText
FROM TreeFieldSetupDefault AS tfsd
JOIN StratumDefault AS sd USING (StratumDefaultID);");

                    var stlfs = new StratumTemplateLogFieldSetupTableDefinition();
                    ds.Execute(stlfs.CreateTable);
                    ds.Execute(stlfs.CreateIndexes);

                    ds.Execute(
    $@"INSERT INTO StratumTemplateLogFieldSetup (
    StratumTemplateName,
    CruiseID,
    Field,
    FieldOrder
) SELECT
    (CASE WHEN sd.Method NOT NULL THEN sd.Method || ' ' ELSE '' END) || (CASE WHEN sd.StratumCode NOT NULL THEN sd.StratumCode || ' ' ELSE '' END) || ifnull(Description, '') AS StratumTemplateName,
    '{cruiseID}' AS CruiseID,
    Field,
    FieldOrder
FROM LogFieldSetupDefault AS lfsd
JOIN StratumDefault AS sd USING (StratumDefaultID);");

                    ds.Execute("DROP TABLE StratumDefault;");
                    ds.Execute("DROP TABLE TreeFieldSetupDefault;");
                    ds.Execute("DROP TABLE LogFieldSetupDefault;");

                    ds.Execute(new StratumDefaultViewDefinition().CreateView);
                    ds.Execute(new TreeFieldSetupDefaultViewDefinition().CreateView);
                    ds.Execute(new LogFieldSetupDefaultViewDefinition().CreateView);
                }

                SetDatabaseVersion(ds, targetVersion);

                ds.CommitTransaction();
            }
            catch (Exception e)
            {
                ds.RollbackTransaction();
                throw new SchemaUpdateException(curVersion, targetVersion, e);
            }
        }

        private void UpdateTo_3_2_4(CruiseDatastore ds)
        {
            // create an in-memory database
            // to migrate into
            using (var newDatastore = new CruiseDatastore_V3())
            {
                var excludeTables = new[]
                {
                    "LK_CruiseMethod",
                    "LK_District",
                    "LK_FIA",
                    "LK_Forest",
                    "LK_LoggingMethod",
                    "LK_Product",
                    "LK_Purpose",
                    "LK_Region",
                    "LK_UOM",
                    "LogField",
                    "TreeField",
                };
                // migrate contents of old db into new in-memory database
                Migrate(ds, newDatastore, excludeTables);

                // use back up rutine to replace old database with
                // migrated contents
                newDatastore.BackupDatabase(ds);
            }
        }

        // update notes: changed view TreeAuditError
        public static void UpdateTo_3_2_3(CruiseDatastore datastore)
        {
            datastore.BeginTransaction();
            try
            {
                var viewDef = new TreeAuditErrorViewDefinition();
                RecreateView(datastore, viewDef);

                datastore.Execute("DELETE FROM TreeField WHERE Field = 'MetaData';");
                datastore.Execute("INSERT INTO TreeField (Field, DefaultHeading, DbType, IsTreeMeasurmentField) VALUES ('MetaData', 'Meta Data', 'TEXT', 1)");

                SetDatabaseVersion(datastore, "3.2.3");
                datastore.CommitTransaction();
            }
            catch
            {
                datastore.RollbackTransaction();
            }
        }

        // update 3.2.2 notes: Added table LK_District and updated initialization for LK_Forests
        public static void UpdateTo_3_2_2(CruiseDatastore ds)
        {
            // create an in-memory database
            // to migrate into
            using (var newDatastore = new CruiseDatastore_V3())
            {
                var excludeTables = new[]
                {
                    "LK_CruiseMethod",
                    "LK_District",
                    "LK_FIA",
                    "LK_Forest",
                    "LK_LoggingMethod",
                    "LK_Product",
                    "LK_Purpose",
                    "LK_Region",
                    "LK_UOM",
                    "LogField",
                    "TreeField",
                };
                // migrate contents of old db into new in-memory database
                Migrate(ds, newDatastore, excludeTables);

                // use back up rutine to replace old database with
                // migrated contents
                newDatastore.BackupDatabase(ds);
            }
        }

        public static void UpdateTo_3_1_0(CruiseDatastore ds)
        {
            // create an in-memory database
            // to migrate into
            using (var newDatastore = new CruiseDatastore_V3())
            {
                var excludeTables = new[] { "SamplerState" };
                // migrate contents of old db into new in-memory database
                Migrate(ds, newDatastore, excludeTables);

                // use back up rutine to replace old database with
                // migrated contents
                newDatastore.BackupDatabase(ds);
            }
        }

        public static void RecreateView(CruiseDatastore datastore, IViewDefinition viewDef)
        {
            var viewName = viewDef.ViewName;
            datastore.Execute($"DROP VIEW {viewName};");
            datastore.Execute(viewDef.CreateView);
        }

        public static void Migrate(CruiseDatastore sourceDS, CruiseDatastore destinationDS, IEnumerable<string> excluding = null, bool excludeLookupTables = false)
        {
            var destConn = destinationDS.OpenConnection();
            var sourceConn = sourceDS.OpenConnection();

            try
            {
                Migrate(sourceConn, destConn, excluding);
            }
            finally
            {
                destinationDS.ReleaseConnection();
                sourceDS.ReleaseConnection();
            }
        }

        public static void Migrate(DbConnection sourceConn, DbConnection destConn, IEnumerable<string> excluding = null, bool excludeLookupTables = false)
        {
            var to = "main"; // alias used by the source database
            var from = "fromdb";

            // get the initial state of foreign keys, used to restore foreign key setting at end of merge process
            var foreignKeys = destConn.ExecuteScalar<string>("PRAGMA foreign_keys;", null, null);
            destConn.ExecuteNonQuery("PRAGMA foreign_keys = off;");

#if SYSTEM_DATA_SQLITE
            var srcDataSource = ((System.Data.SQLite.SQLiteConnection)sourceConn).FileName;
#else
            var srcDataSource = sourceConn.DataSource;
#endif
            destConn.ExecuteNonQuery($"ATTACH DATABASE \"{srcDataSource}\" AS {from};");
            try
            {
                using (var transaction = destConn.BeginTransaction())
                {
                    // get list of all tables that are in both databases
                    IEnumerable<string> tables = ListTablesIntersect(destConn, sourceConn);
                    try
                    {
                        foreach (var table in tables)
                        {
                            if (excludeLookupTables && table.StartsWith("LK_"))
                            { continue; }
                            if (excluding?.Contains(table) ?? false)
                            { continue; }

                            if (table == "Globals")
                            {
                                destConn.ExecuteNonQuery(
                $@"INSERT OR IGNORE INTO {to}.{table} (""Block"", ""Key"", ""Value"")
SELECT ""Block"", ""Key"", ""Value""
FROM {from}.{table}
WHERE ""Block"" != 'Database' AND ""Key"" != 'Version';", null, transaction);
                                continue;
                            }
                            else
                            {
                                // get the interscetion of fields in table from both databases
                                // encased in double quotes just incase any field names are sql keywords
                                string[] both = ListFieldsIntersect(sourceConn, destConn, table);
                                var fields = string.Join(",", both);

                                destConn.ExecuteNonQuery(
                $@"INSERT OR IGNORE INTO {to}.{table} ({fields})
SELECT {fields} FROM {from}.{table};"
                                    , null, transaction);
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        Logger.LogException(e);
                        throw;
                    }
                    finally
                    {
                        sourceConn.ExecuteNonQuery($"PRAGMA foreign_keys = {foreignKeys};");
                    }
                }
            }
            finally
            {
                destConn.ExecuteNonQuery($"DETACH DATABASE {from};");
            }
        }

        public static string[] ListFieldsIntersect(DbConnection sourceConn, DbConnection destConn, string table)
        {
            var sourceFields = sourceConn.QueryScalar2<string>($@"SELECT '""' || Name || '""' FROM pragma_table_info('{table}');");

            var destFields = destConn.QueryScalar2<string>($@"SELECT '""' || Name || '""' FROM pragma_table_info('{table}');");

            var both = sourceFields.Intersect(destFields).ToArray();
            return both;
        }

        public static string[] ListTablesIntersect(DbConnection conn1, DbConnection conn2)
        {
            var query = "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite^_%' ESCAPE '^';";

            var tables1 = conn1.QueryScalar2<string>(query);

            var tables2 = conn2.QueryScalar2<string>(query);

            return tables1.Intersect(tables2).ToArray();
        }

        public static void SetDatabaseVersion(CruiseDatastore db, string newVersion)
        {
            string command = String.Format("UPDATE Globals SET Value = '{0}' WHERE Block = 'Database' AND Key = 'Version';", newVersion);
            db.Execute(command);
            db.LogMessage($"Updated structure version to {newVersion}");
        }
    }
}