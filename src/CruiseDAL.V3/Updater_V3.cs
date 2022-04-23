using CruiseDAL.Schema;
using CruiseDAL.Schema.Views;
using FMSC.ORM;
using FMSC.ORM.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CruiseDAL
{
    public partial class Updater_V3 : IUpdater
    {
        public static ILogger Logger { get; set; } = LoggerProvider.Get();

        public void Update(CruiseDatastore db)
        {
            db.OpenConnection(); // make sure connection stays open during update process

            var version = db.DatabaseVersion;

            if (version == "3.0.0"
                || version == "3.0.1"
                || version == "3.0.2"
                || version == "3.0.3")
            {
                UpdateTo_3_1_0(db);
            }
            if (version == "3.1.0" || version == "3.2.0" || version == "3.2.1")
            {
                UpdateTo_3_2_2(db);
            }
            if (version == "3.2.2")
            {
                UpdateTo_3_2_3(db);
            }
            if (version == "3.2.3")
            {
                UpdateTo_3_2_4(db);
            }
            if (version == "3.2.4")
            {
                UpdateTo_3_3_0(db);
            }

            try
            {
                try
                {
                    var v = new Version(version);
                    if (v.Major == 3 && v.Minor < 4)
                    {
                        db.Execute("DROP TRIGGER TreeLocation_OnUpdate;");
                        db.Execute(TreeLocationTableDefinition.CREATE_TRIGGER_TreeLocation_ONUPDATE);
                    }
                }
                // handel exception thrown when parsing version code
                catch (ArgumentException ex)
                {
                    throw new SchemaUpdateException(version, null, ex);
                }

                if (db.DatabaseVersion == "3.3.0")
                {
                    UpdateTo_3_3_1(db);
                }
                if (db.DatabaseVersion == "3.3.1")
                {
                    UpdateTo_3_3_2(db);
                }
                if (db.DatabaseVersion == "3.3.2")
                {
                    UpdateTo_3_3_3(db);
                }
                if (db.DatabaseVersion == "3.3.3")
                {
                    UpdateTo_3_3_4(db);
                }
                if (db.DatabaseVersion == "3.3.4")
                {
                    UpdateTo_3_4_0(db);
                }
                if (db.DatabaseVersion == "3.4.0")
                {
                    UpdateTo_3_4_1(db);
                }
                if (db.DatabaseVersion == "3.4.1")
                {
                    UpdateTo_3_4_2(db);
                }
                if (db.DatabaseVersion == "3.4.2")
                {
                    UpdateTo_3_4_3(db);
                }
                if (db.DatabaseVersion == "3.4.3")
                {
                    UpdateTo_3_4_4(db);
                }
                if (db.DatabaseVersion == "3.4.4")
                {
                    UpdateTo_3_5_0(db);
                }
                if (db.DatabaseVersion == "3.5.0")
                {
                    UpdateTo_3_5_1(db);
                }

                if (db.DatabaseVersion == "3.5.1")
                {
                    UpdateTo_3_5_2(db);
                }
            }
            finally
            {
                db.ReleaseConnection();
            }
        }

        private void UpdateTo_3_5_2(CruiseDatastore db)
        {
            var curVersion = db.DatabaseVersion;
            var targetVersion = "3.5.2";

            var fKeys = db.ExecuteScalar<string>("PRAGMA foreign_keys;");
            db.Execute("PRAGMA foreign_keys=OFF;");

            db.BeginTransaction();
            try
            {
                // Rebuild VolumeEquation table adding ForeignKey constraint on CruiseID
                db.Execute("DELETE FROM VolumeEquation WHERE CruiseID NOT IN (SELECT CruiseID FROM Cruise)");
                RebuildTable(db, new VolumeEquationTableDefinition());



                SetDatabaseVersion(db, targetVersion);
                db.CommitTransaction();

                db.Execute($"PRAGMA foreign_keys={fKeys};");
            }
            catch (Exception e)
            {
                db.RollbackTransaction();
                throw new SchemaUpdateException(curVersion, targetVersion, e);
            }
        }

        private void UpdateTo_3_5_1(CruiseDatastore db)
        {
            var curVersion = db.DatabaseVersion;
            var targetVersion = "3.5.1";

            db.BeginTransaction();
            try
            {
                db.Execute("DROP VIEW Tree_TreeDefaultValue;");
                db.Execute("DROP VIEW TreeAuditError");

                db.Execute(Tree_TreeDefaultValue.CREATE_VIEW_3_5_1);
                db.Execute(TreeAuditErrorViewDefinition.CREATE_VIEW_3_5_1);

                SetDatabaseVersion(db, targetVersion);
                db.CommitTransaction();
            }
            catch (Exception e)
            {
                db.RollbackTransaction();
                throw new SchemaUpdateException(curVersion, targetVersion, e);
            }
        }

        private void UpdateTo_3_5_0(CruiseDatastore db)
        {
            var curVersion = db.DatabaseVersion;
            var targetVersion = "3.5.0";

            db.BeginTransaction();
            try
            {
                // create Species_Product table
                CreateTable(db, new Species_ProductTableDefinition());

                // populate Species_Product table from Species table
                db.Execute("INSERT INTO Species_Product (" +
                    "CruiseID, " +
                    "SpeciesCode, " +
                    "PrimaryProduct, " +
                    "ContractSpecies " +
                    ") " +
                    "SELECT CruiseID, SpeciesCode, null AS PrimaryProduct, ContractSpecies " +
                    "FROM Species WHERE ContractSpecies IS NOT NULL;");

                // add Trigger to keep Species and Species_Product in sync
                db.Execute(SpeciesTableDefinition.CREATE_TRIGGER_Species_OnUpdate_ContractSpecies);

                // rebuild TallyLedger_Tree_Totals view
                db.Execute("DROP VIEW TallyLedger_Tree_Totals;");
                db.Execute(TallyLedgerViewDefinition.CREATE_VIEW_TallyLedger_Tree_Totals);

                SetDatabaseVersion(db, targetVersion);
                db.CommitTransaction();
            }
            catch (Exception e)
            {
                db.RollbackTransaction();
                throw new SchemaUpdateException(curVersion, targetVersion, e);
            }
        }

        // remove foreign keys from TallyLedger table used to keep SpeciesCode, LiveDead, StratumCode and SampleGroupCode
        // in sync between Tree and TallyLedger tables and replace them with triggers
        // this works better in situations where either SpeciesCode or LiveDead were initialy null
        // also removing indexes used to support those foreign keys from tree table
        private void UpdateTo_3_4_4(CruiseDatastore db)
        {
            var curVersion = db.DatabaseVersion;
            var targetVersion = "3.4.4";

            var fKeys = db.ExecuteScalar<string>("PRAGMA foreign_keys;");
            db.Execute("PRAGMA foreign_keys=OFF;");

            db.BeginTransaction();
            try
            {
                db.Execute("DROP VIEW TallyLedger_Totals;");
                db.Execute("DROP VIEW TallyLedger_Tree_Totals;");
                db.Execute("DROP VIEW TallyLedger_Plot_Totals;");
                RebuildTable(db, new TallyLedgerTableDefinition());
                db.Execute(new TallyLedgerViewDefinition().CreateView);

                db.Execute(TreeTableDefinition.CREATE_TRIGGER_TREE_Cascade_Species_Updates);
                db.Execute(TreeTableDefinition.CREATE_TRIGGER_TREE_Cascade_LiveDead_Updates);
                db.Execute(TreeTableDefinition.CREATE_TRIGGER_TREE_Cascade_SampleGroupCode_Updates);
                db.Execute(TreeTableDefinition.CREATE_TRIGGER_TREE_Cascade_StratumCode_Updates);
                db.Execute("DROP INDEX UIX_Tree_TreeID_SpeciesCode;");
                db.Execute("DROP INDEX UIX_Tree_TreeID_LiveDead;");

                SetDatabaseVersion(db, targetVersion);
                db.CommitTransaction();

                db.Execute($"PRAGMA foreign_keys={fKeys};");
            }
            catch (Exception e)
            {
                db.RollbackTransaction();
                throw new SchemaUpdateException(curVersion, targetVersion, e);
            }
        }

        // add validation on tree for DBH and DRC
        private void UpdateTo_3_4_3(CruiseDatastore db)
        {
            var curVersion = db.DatabaseVersion;
            var targetVersion = "3.4.3";

            db.BeginTransaction();
            try
            {
                db.Execute("DROP VIEW TreeError;");
                db.Execute(TreeErrorViewDefinition.v3_4_3);
                SetDatabaseVersion(db, targetVersion);
                db.CommitTransaction();
            }
            catch (Exception e)
            {
                db.RollbackTransaction();
                throw new SchemaUpdateException(curVersion, targetVersion, e);
            }
        }

        // UpdateTo_3_4_1 forgot to add CountOrMeasure, TreeCount, and AverageHeight fields to
        // the Plot_Stratum table. This update checks to see if they need to be added and adds them
        // if missing
        private void UpdateTo_3_4_2(CruiseDatastore db)
        {
            var curVersion = db.DatabaseVersion;
            var targetVersion = "3.4.2";

            if (db.CheckFieldExists("Plot_Stratum_Tombstone", "CountOrMeasure") is false)
            {
                db.BeginTransaction();
                try
                {
                    db.Execute("ALTER TABLE Plot_Stratum_Tombstone ADD COLUMN CountOrMeasure TEXT COLLATE NOCASE;");
                    db.Execute("ALTER TABLE Plot_Stratum_Tombstone ADD COLUMN TreeCount INTEGER Default 0;");
                    db.Execute("ALTER TABLE Plot_Stratum_Tombstone ADD COLUMN AverageHeight REAL Default 0.0;");

                    SetDatabaseVersion(db, targetVersion);
                    db.CommitTransaction();
                }
                catch (Exception e)
                {
                    db.RollbackTransaction();
                    throw new SchemaUpdateException(curVersion, targetVersion, e);
                }
            }
            else
            {
                SetDatabaseVersion(db, targetVersion);
            }
        }

        // Add TreeCount, Average Height, and CountOrMeasure fields to plot stratum
        private void UpdateTo_3_4_1(CruiseDatastore db)
        {
            var curVersion = db.DatabaseVersion;
            var targetVersion = "3.4.1";

            var fKeys = db.ExecuteScalar<string>("PRAGMA foreign_keys;");
            db.Execute("PRAGMA foreign_keys=OFF;");

            db.BeginTransaction();
            try
            {
                //Rebuild Plot_Stratum table
                db.Execute("DROP VIEW main.PlotError");
                RebuildTable(db, new Plot_StratumTableDefinition_3_4_1());
                db.Execute(new PlotErrorViewDefinition().CreateView);

                //create a bunch of clearTombstone triggers
                db.Execute(CuttingUnit_StratumTableDefinition.CREATE_TRIGGER_CuttingUnit_Stratum_OnInsert_ClearTombstone);
                db.Execute(LogFieldSetupTableDefinition.CREATE_TRIGGER_LogFieldSetup_OnInsert_ClearTombstone);
                db.Execute(SubPopulationTableDefinition.CREATE_TRIGGER_SubPopulation_OnInsert_ClearTombstone);
                db.Execute(TreeAuditRuleSelectorTableDefinition.CREATE_TRIGGER_TreeAuditRuleSelector_OnInsert_ClearTombstone);
                db.Execute(TreeFieldSetupTableDefinition.CREATE_TRIGGER_TreeFieldSetup_OnInsert_ClearTombstone);
                db.Execute(ReportsTableDefinition.CREATE_TRIGGER_Reports_OnInsert_ClearTombstone);
                db.Execute(VolumeEquationTableDefinition.CREATE_TRIGGE_VolumeEquation_OnInsert_ClearTombstone);

                // recreate index on Reports_Tombstone
                db.Execute("DROP INDEX Reports_Tombstone_ReportID;");
                db.Execute("CREATE INDEX Reports_Tombstone_ReportID_CruiseID ON Reports_Tombstone (ReportID, CruiseID);");

                SetDatabaseVersion(db, targetVersion);
                db.CommitTransaction();

                db.Execute($"PRAGMA foreign_keys={fKeys};");
            }
            catch (Exception e)
            {
                db.RollbackTransaction();
                throw new SchemaUpdateException(curVersion, targetVersion, e);
            }
        }

        private void UpdateTo_3_4_0(CruiseDatastore db)
        {
            var curVersion = db.DatabaseVersion;
            var targetVersion = "3.4.0";

            var fKeys = db.ExecuteScalar<string>("PRAGMA foreign_keys;");
            db.Execute("PRAGMA foreign_keys=OFF;");

            db.BeginTransaction();
            try
            {
                RebuildTable(db, new CruiseTableDefinition_3_4_0(), customFieldMaps:
                    new KeyValuePair<string, string>[] { new KeyValuePair<string, string>(
                        "SaleNumber",
                        "(SELECT SaleNumber FROM Sale WHERE cruise.SaleID = sale.SaleID)")
                    });

                if (fKeys == "ON")
                {
                    var keyCheck = db.QueryGeneric("PRAGMA foreign_key_check;");
                    if (keyCheck.Any())
                    {
                        throw new SchemaException("Foreign Key Check failed");
                    }
                }

                db.Execute("CREATE INDEX NIX_TreeDefaultValue_PrimaryProduct ON TreeDefaultValue ('PrimaryProduct');");

                var tree_tdvViewDef = new Tree_TreeDefaultValue();
                db.Execute(tree_tdvViewDef.CreateView);

                var tm_defViewDef = new TreeMeasurment_DefaultResolved();
                db.Execute(tm_defViewDef.CreateView);

                SetDatabaseVersion(db, targetVersion);
                db.CommitTransaction();

                db.Execute($"PRAGMA foreign_keys={fKeys};");
            }
            catch (Exception e)
            {
                db.RollbackTransaction();
                throw new SchemaUpdateException(curVersion, targetVersion, e);
            }
        }

        // update 3.3.4 notes:
        // added change tracking fields to StratumTempalte, StratumTemplateTreeFieldSetup, and StratumTemplateLogFileSetup
        // added tombstone tables for StratumTempalte, StratumTemplateTreeFieldSetup, and StratumTemplateLogFileSetup
        private void UpdateTo_3_3_4(CruiseDatastore db)
        {
            var curVersion = db.DatabaseVersion;
            var targetVersion = "3.3.4";

            db.BeginTransaction();
            try
            {
                var stratumTemplateTableDef = new StratumTemplateTableDefinition();
                var stratumTemplateTreeFieldSetupTableDef = new StratumTemplateTreeFieldSetupTableDefinition();
                var stratumTemplateLogFieldSetupTableDef = new StratumTemplateLogFieldSetupTableDefinition();

                db.Execute("DROP VIEW StratumDefault;");
                db.Execute("DROP VIEW TreeFieldSetupDefault;");
                db.Execute("DROP VIEW LogFieldSetupDefault;");

                db.Execute(stratumTemplateTableDef.CreateTombstoneTable);
                db.Execute(stratumTemplateTreeFieldSetupTableDef.CreateTombstoneTable);
                db.Execute(stratumTemplateLogFieldSetupTableDef.CreateTombstoneTable);

                RebuildTable(db, stratumTemplateTableDef);
                RebuildTable(db, stratumTemplateTreeFieldSetupTableDef);
                RebuildTable(db, stratumTemplateLogFieldSetupTableDef);

                db.Execute(new StratumDefaultViewDefinition().CreateView);
                db.Execute(new TreeFieldSetupDefaultViewDefinition().CreateView);
                db.Execute(new LogFieldSetupDefaultViewDefinition().CreateView);

                SetDatabaseVersion(db, targetVersion);
                db.CommitTransaction();
            }
            catch (Exception e)
            {
                db.RollbackTransaction();
                throw new SchemaUpdateException(curVersion, targetVersion, e);
            }
        }

        // update 3.3.3 notes:
        // added column TemplateFile to Cruise table
        // added lookup table LK_TallyEntryType
        // remove check constraint on EntryType and add FKey on EntryType
        private void UpdateTo_3_3_3(CruiseDatastore db)
        {
            var curVersion = db.DatabaseVersion;
            var targetVersion = "3.3.3";

            db.BeginTransaction();
            try
            {
                db.Execute("ALTER TABLE main.Cruise ADD COLUMN TemplateFile TEXT;");

                // create table LK_TallyEntryType
                var tallyEntryTypeTableDef = new LK_TallyEntryType();
                CreateTable(db, tallyEntryTypeTableDef);

                // remove check constraint on EntryType and add FKey on EntryType
                var tallyLedgerTableDef = new TallyLedgerTableDefinition();
                UpdateTableDDL(db, tallyLedgerTableDef);

                SetDatabaseVersion(db, targetVersion);
                db.CommitTransaction();
            }
            catch (Exception e)
            {
                db.RollbackTransaction();
                throw new SchemaUpdateException(curVersion, targetVersion, e);
            }
        }

        // update 3.3.2 notes:
        // added Biomass and ValueEquation tables
        // changed TallyPopulation view so that 3p methods are always treaded as tally by subpop
        private void UpdateTo_3_3_2(CruiseDatastore db)
        {
            var curVersion = db.DatabaseVersion;
            var targetVersion = "3.3.2";

            db.BeginTransaction();
            try
            {
                var biomassTableDef = new BiomassEquationTableDefinition();
                var valueEqTableDef = new ValueEquationTableDefinition();

                CreateTable(db, biomassTableDef);
                CreateTable(db, valueEqTableDef);

                // allways treat 3p methods as tally by subpop
                var tallyPopViewDef = new TallyPopulationViewDefinition_3_3_2();
                RecreateView(db, tallyPopViewDef);

                SetDatabaseVersion(db, targetVersion);
                db.CommitTransaction();
            }
            catch (Exception e)
            {
                db.RollbackTransaction();
                throw new SchemaUpdateException(curVersion, targetVersion, e);
            }
        }

        // update 3.3.1 notes:
        // Add CruiseID fields to Log and Stem tables
        // Change is fully backwards compatible with prior versions
        private void UpdateTo_3_3_1(CruiseDatastore db)
        {
            var curVersion = db.DatabaseVersion;
            var targetVersion = "3.3.1";

            var fKeys = db.ExecuteScalar<string>("PRAGMA foreign_keys;");
            db.Execute("PRAGMA foreign_keys=OFF;");

            db.BeginTransaction();
            try
            {
                // need to drop any views associated with tables we are rebuilding
                db.Execute("DROP VIEW LogGradeError;");

                var logTableDef = new LogTableDefinition();
                var stemTableDef = new StemTableDefinition();

                db.Execute("DROP TABLE Log_Tombstone;");
                db.Execute("DROP TABLE Stem_Tombstone;");

                db.Execute(logTableDef.CreateTombstoneTable);
                db.Execute(stemTableDef.CreateTombstoneTable);

                RebuildTable(db, logTableDef, customFieldMaps: new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>("CruiseID", "(SELECT CruiseID FROM Tree WHERE Tree.TreeID = Log.TreeID)"),
                });

                RebuildTable(db, stemTableDef, customFieldMaps: new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>("CruiseID", "(SELECT CruiseID FROM Tree WHERE Tree.TreeID = Stem.TreeID)"),
                });

                var lgeViewDef = new LogGradeErrorViewDefinition();
                db.Execute(lgeViewDef.CreateView);

                SetDatabaseVersion(db, targetVersion);
                db.CommitTransaction();

                db.Execute($"PRAGMA foreign_keys={fKeys};");
            }
            catch (Exception e)
            {
                db.RollbackTransaction();
                throw new SchemaUpdateException(curVersion, targetVersion, e);
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
    }
}