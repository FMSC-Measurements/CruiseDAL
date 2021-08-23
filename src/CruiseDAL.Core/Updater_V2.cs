using Backpack.SqlBuilder;
using FMSC.ORM;
using FMSC.ORM.Core;
using System;
using System.Data.Common;

namespace CruiseDAL
{
    public partial class Updater_V2 : IUpdater
    {
        public void Update(CruiseDatastore datastore)
        {

            //PatchSureToMeasure(db);

            Update_Impl(datastore);

            // the following method calls are not nessicary for updating the 
            // database. They just need to be ran to clean up potential errors. 
            CleanupErrorLog(datastore);
            FixTreeAuditValueFKeyErrors(datastore);
        }

        public static void Update_Impl(CruiseDatastore db)
        {
            var dbVersion = db.DatabaseVersion;
            if (string.IsNullOrWhiteSpace(dbVersion))
            { throw new UpdateException("unable to determin file version"); }

            if (!CheckCanUpdate(dbVersion))
            {
                throw new IncompatibleSchemaException($"The version of this cruise file ({dbVersion}) is not compatible with the version of the software you are using." +
                    "Go to github.com/FMSC-Measurements to get the latest version of our software.", null);
            }

            if (dbVersion.StartsWith("2013") || dbVersion == "2014.01.21")
            {
                throw new IncompatibleSchemaException($"The version of this cruise file ({dbVersion}) is no longer supported." +
                "Go to github.com/FMSC-Measurements to get archived versions of our software.", null);
            }

            if (db.DatabaseVersion == "2014.03.12")
            {
                UpdateToVersion2014_06_04(db);
            }
            if (db.DatabaseVersion == "2014.06.04")
            {
                UpdateToVersion2014_07_02(db);
            }
            if (db.DatabaseVersion == "2014.07.02")
            {
                UpdateToVersion2014_07_07(db);
            }
            if (db.DatabaseVersion == "2014.07.07")
            {
                UpdateToVersion2014_07_17(db);
            }
            if (db.DatabaseVersion == "2014.07.17")
            {
                UpdateToVersion2014_07_24(db);
            }
            if (db.DatabaseVersion == "2014.07.24")
            {
                UpdateToVersion2014_08_20(db);
            }
            if (db.DatabaseVersion == "2014.08.20")
            {
                UpdateToVersion2014_09_02(db);
            }
            if (db.DatabaseVersion == "2014.09.02")
            {
                UpdateToVersion2014_10_01(db);
            }
            if (db.DatabaseVersion == "2014.10.01" || db.DatabaseVersion == "2015.01.05")
            {
                UpdateToVersion2015_04_28(db);
            }

            if (db.DatabaseVersion == "2015.04.28")
            {
                UpdateToVersion2015_08_03(db);
            }

            if (db.DatabaseVersion == "2015.06.01")
            {
                SetDatabaseVersion(db, "2015.08.03");
            }

            if (db.DatabaseVersion == "2015.08.03")
            {
                UpdateToVersion2015_08_19(db);
            }
            if (db.DatabaseVersion == "2015.08.19")
            {
                UpdateToVersion2015_09_01(db);
            }
            if (db.DatabaseVersion == "2015.09.01"
                || db.DatabaseVersion == "2.0.0"
                || db.DatabaseVersion == "2.1.0")
            {
                UpdateTo_2_1_1(db);
            }
            if (db.DatabaseVersion.StartsWith("2.1.1"))
            {
                UpdateTo_2_1_2(db);
            }
            if (db.DatabaseVersion.StartsWith("2.1.2"))
            {
                UpdateTo_2_2_0(db);
            }
            // files updated to 2.5.0 may have corrupted data
            // attempt to patch them and set version to 2.5.1.1
            if (db.DatabaseVersion.StartsWith("2.5.0"))
            {
                FixVersion_2_5_0(db);
            }
            if (db.DatabaseVersion.StartsWith("2.2."))
            {
                UpdateTo_2_5_1(db);
            }
            if (db.DatabaseVersion.StartsWith("2.5."))
            {
                UpdateTo_2_6_1(db);
            }
            if (db.DatabaseVersion.StartsWith("2.6."))
            {
                UpdateTo_2_7_0(db);
            }
            if (db.DatabaseVersion == "2.7.0")
            {
                UpdateTo_2_7_1(db);
            }
            if (db.DatabaseVersion == "2.7.1" || db.DatabaseVersion == "2.7.3")
            {
                UpdateTo_2_7_3(db);
            }

            if (db.CheckFieldExists("Stratum", "HotKey") == false)
            {
                db.AddField("Stratum", new ColumnInfo("HotKey", "TEXT"));
            }

        }



        public static void UpdateToVersion2015_04_28(CruiseDatastore db)
        {
            var startVersion = db.DatabaseVersion;

            try
            {
                db.BeginTransaction();
                db.Execute(@"CREATE TABLE Util_Tombstone (
                    RecordID INTEGER ,
                    RecordGUID TEXT,
                    TableName TEXT NOT NULL COLLATE NOCASE,
                    Data TEXT,
                    DeletedDate DATETIME NON NULL);");

                //                db.Execute(@"
                //                CREATE VIEW CountTree_View AS
                //SELECT Stratum.Code as StratumCode,
                //Stratum.Method as Method,
                //SampleGroup.Code as SampleGroupCode,
                //SampleGroup.PrimaryProduct as PrimaryProduct,
                //CountTree.*
                //FROM CountTree JOIN SampleGroup USING (SampleGroup_CN) JOIN Stratum USING (Stratum_CN);");

                db.Execute(@"ALTER TABLE Sale ADD COLUMN RowVersion INTEGER DEFAULT 0;
                    ALTER TABLE CuttingUnit ADD COLUMN RowVersion INTEGER DEFAULT 0;
                    ALTER TABLE Stratum ADD COLUMN RowVersion INTEGER DEFAULT 0;
                    ALTER TABLE SampleGroup ADD COLUMN RowVersion INTEGER DEFAULT 0;
                    ALTER TABLE TreeDefaultValue ADD COLUMN RowVersion INTEGER DEFAULT 0;
                    ALTER TABLE Plot ADD COLUMN RowVersion INTEGER DEFAULT 0;
                    ALTER TABLE Tree ADD COLUMN RowVersion INTEGER DEFAULT 0;
                    ALTER TABLE Log ADD COLUMN RowVersion INTEGER DEFAULT 0;
                    ALTER TABLE Stem ADD COLUMN RowVersion INTEGER DEFAULT 0;
                    ALTER TABLE CountTree ADD COLUMN RowVersion INTEGER DEFAULT 0;");

                db.Execute(@"ALTER TABLE Stem ADD COLUMN CreatedBy TEXT;
                    ALTER TABLE Stem ADD COLUMN CreatedDate DATETIME;
                    ALTER TABLE Stem ADD COLUMN ModifiedBy TEXT;
                    ALTER TABLE Stem ADD COLUMN ModifiedDate DATETIME;
                    ALTER TABLE TreeEstimate ADD COLUMN CreatedBy TEXT;
                    ALTER TABLE TreeEstimate ADD COLUMN CreatedDate DATETIME;
                    ALTER TABLE TreeEstimate ADD COLUMN ModifiedBy TEXT;
                    ALTER TABLE TreeEstimate ADD COLUMN ModifiedDate DATETIME;
                    ALTER TABLE TreeDefaultValue ADD COLUMN CreatedBy TEXT;
                    ALTER TABLE TreeDefaultValue ADD COLUMN CreatedDate DATETIME;
                    ALTER TABLE TreeDefaultValue ADD COLUMN ModifiedBy TEXT;
                    ALTER TABLE TreeDefaultValue ADD COLUMN ModifiedDate DATETIME;");

                db.Execute("ALTER TABLE SampleGroup ADD COLUMN SmallFPS REAL DEFAULT 0.0;");

                db.Execute("ALTER TABLE Tree ADD COLUMN UpperStemDiameter REAL DEFAULT 0.0;");
                db.Execute("UPDATE Tree SET UpperStemDiameter = UpperstemDOB;");
                db.Execute("UPDATE TreeFieldSetup SET Field = 'UpperStemDiameter' WHERE Field = 'UpperStemDiameter';");
                db.Execute("UPDATE TreeFieldSetupDefault SET Field = 'UpperStemDiameter' WHERE Field = 'UpperStemDiameter';");

                db.Execute("ALTER TABLE Stratum ADD COLUMN YieldComponent TEXT DEFAULT 'CL';");
                db.Execute("UPDATE TreeDefaultValue SET Chargeable = null;");

                db.Execute("ALTER TABLE CuttingUnitStratum ADD COLUMN StratumArea REAL;");

                db.Execute(@"CREATE VIEW StratumAcres_View AS
SELECT CuttingUnit.Code as CuttingUnitCode,
Stratum.Code as StratumCode,
ifnull(Area, CuttingUnit.Area) as Area,
CuttingUnitStratum.*
FROM CuttingUnitStratum
JOIN CuttingUnit USING (CuttingUnit_CN)
JOIN Stratum USING (Stratum_CN);");

                db.Execute("PRAGMA user_version = 1");
                SetDatabaseVersion(db, "2015.04.28");
                db.CommitTransaction();
            }
            catch (Exception e)
            {
                db.RollbackTransaction();
                throw new SchemaUpdateException(startVersion, "2015.04.28", e);
            }
        }

        public static void UpdateToVersion2015_08_03(CruiseDatastore db)
        {
            var startVersion = db.DatabaseVersion;

            try
            {
                db.BeginTransaction();
                db.AddField("Plot", new ColumnInfo("Plot_GUID", "TEXT"));
                db.AddField("Tree", new ColumnInfo("Tree_GUID", "TEXT"));
                db.AddField("Log", new ColumnInfo("Log_GUID", "TEXT"));
                db.AddField("Stem", new ColumnInfo("Stem_GUID", "TEXT"));
                db.AddField("TreeEstimate", new ColumnInfo("TreeEstimate_GUID", "TEXT"));

                SetDatabaseVersion(db, "2015.08.03");

                db.CommitTransaction();
            }
            catch (Exception e)
            {
                db.RollbackTransaction();
                throw new SchemaUpdateException(startVersion, "2015.08.03", e);
            }
        }

        private static void UpdateToVersion2015_08_19(CruiseDatastore db)
        {
            var startVersion = db.DatabaseVersion;

            var tavCols = db.GetTableInfo("TreeAuditValue");
            bool hasErrorMessageCol = false;
            foreach (ColumnInfo col in tavCols)
            {
                if (col.Name == "ErrorMessage")
                {
                    hasErrorMessageCol = true; break;
                }
            }

            try
            {
                db.BeginTransaction();
                if (!hasErrorMessageCol)
                {
                    db.AddField("TreeAuditValue", new ColumnInfo("ErrorMessage", "TEXT"));
                }

                SetDatabaseVersion(db, "2015.08.19");
                db.CommitTransaction();
            }
            catch (Exception e)
            {
                db.RollbackTransaction();
                throw new SchemaUpdateException(startVersion, "2015.08.19", e);
            }
        }

        //patch for some a version that got out in the wild with bad triggers
        private static void UpdateToVersion2015_09_01(CruiseDatastore db)
        {
            var startVersion = db.DatabaseVersion;

            db.BeginTransaction();
            try
            {
                //because there are a lot of changes with triggers
                //lets just recreate all triggers
                foreach (string trigName in ListTriggers(db))
                {
                    db.Execute("DROP TRIGGER " + trigName + ";");
                }

                //db.Execute(Schema.Schema.CREATE_TRIGGERS);

                SetDatabaseVersion(db, "2015.09.01");
                db.CommitTransaction();
            }
            catch (Exception e)
            {
                db.RollbackTransaction();
                throw new SchemaUpdateException(startVersion, "2015.09.01", e);
            }
        }

        private static void UpdateTo_2_1_1(CruiseDatastore db)
        {
            var startVersion = db.DatabaseVersion;

            db.BeginTransaction();
            try
            {
                db.Execute(@"CREATE TABLE IF NOT EXISTS FixCNTTallyClass (
				FixCNTTallyClass_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				Stratum_CN INTEGER REFERENCES Stratum NOT NULL,
				FieldName INTEGER Default 0);");

                db.Execute(@"CREATE TABLE IF NOT EXISTS FixCNTTallyPopulation (
				FixCNTTallyPopulation_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				FixCNTTallyClass_CN INTEGER REFERENCES FixCNTTallyClass NOT NULL,
				SampleGroup_CN INTEGER REFERENCES SampleGroup NOT NULL,
				TreeDefaultValue_CN INTEGER REFERENCES TreeDefaultValue NOT NULL,
				IntervalSize INTEGER Default 0,
				Min INTEGER Default 0,
				Max INTEGER Default 0);");

                SetDatabaseVersion(db, "2.1.1");
                db.CommitTransaction();
            }
            catch (Exception e)
            {
                db.RollbackTransaction();
                throw new SchemaUpdateException(startVersion, "2.1.1", e);
            }
        }

        //remove unique constraint from tree.treeNumber
        //when users were trying to renumber trees in a
        //transaction it would fail. Even though at the end
        //of the transaction the constraint would not be in
        //violation. Because there is no way to fix this bug
        //in software with previous versions, aswell, the unique
        // constraint was only working properly when Plot_CN was
        // not null I'm deciding to remove it alltogether. RIP
        private static void UpdateTo_2_1_2(CruiseDatastore db)
        {
            var startVersion = db.DatabaseVersion;

            try
            {
                var fk = db.ExecuteScalar("Pragma foreign_keys;");

                var treeTriggerDDL = GetTriggerDDL(db, "Tree");

                db.Execute(
                "PRAGMA foreign_keys = off;\r\n" +
                "BEGIN;\r\n" +
                "CREATE TABLE new_Tree (\r\n" +
                "Tree_CN INTEGER PRIMARY KEY AUTOINCREMENT,\r\n" +
                "Tree_GUID TEXT," +
                "TreeDefaultValue_CN INTEGER REFERENCES TreeDefaultValue,\r\n" +
                "Stratum_CN INTEGER REFERENCES Stratum NOT NULL,\r\n" +
                "SampleGroup_CN INTEGER REFERENCES SampleGroup,\r\n" +
                "CuttingUnit_CN INTEGER REFERENCES CuttingUnit NOT NULL,\r\n" +
                "Plot_CN INTEGER REFERENCES Plot,\r\n" +
                "TreeNumber INTEGER NOT NULL,\r\n" +
                "Species TEXT,\r\n" +
                "CountOrMeasure TEXT,\r\n" +
                "TreeCount REAL Default 0.0,\r\n" +
                "KPI REAL Default 0.0,\r\n" +
                "STM TEXT Default 'N',\r\n" +
                "SeenDefectPrimary REAL Default 0.0,\r\n" +
                "SeenDefectSecondary REAL Default 0.0,\r\n" +
                "RecoverablePrimary REAL Default 0.0,\r\n" +
                "HiddenPrimary REAL Default 0.0,\r\n" +
                "Initials TEXT,\r\n" +
                "LiveDead TEXT,\r\n" +
                "Grade TEXT,\r\n" +
                "HeightToFirstLiveLimb REAL Default 0.0,\r\n" +
                "PoleLength REAL Default 0.0,\r\n" +
                "ClearFace TEXT,\r\n" +
                "CrownRatio REAL Default 0.0,\r\n" +
                "DBH REAL Default 0.0,\r\n" +
                "DRC REAL Default 0.0,\r\n" +
                "TotalHeight REAL Default 0.0,\r\n" +
                "MerchHeightPrimary REAL Default 0.0,\r\n" +
                "MerchHeightSecondary REAL Default 0.0,\r\n" +
                "FormClass REAL Default 0.0,\r\n" +
                "UpperStemDOB REAL Default 0.0,\r\n" +
                "UpperStemDiameter REAL Default 0.0,\r\n" +
                "UpperStemHeight REAL Default 0.0,\r\n" +
                "DBHDoubleBarkThickness REAL Default 0.0,\r\n" +
                "TopDIBPrimary REAL Default 0.0,\r\n" +
                "TopDIBSecondary REAL Default 0.0,\r\n" +
                "DefectCode TEXT,\r\n" +
                "DiameterAtDefect REAL Default 0.0,\r\n" +
                "VoidPercent REAL Default 0.0,\r\n" +
                "Slope REAL Default 0.0,\r\n" +
                "Aspect REAL Default 0.0,\r\n" +
                "Remarks TEXT,\r\n" +
                "XCoordinate DOUBLE Default 0.0,\r\n" +
                "YCoordinate DOUBLE Default 0.0,\r\n" +
                "ZCoordinate DOUBLE Default 0.0,\r\n" +
                "MetaData TEXT,\r\n" +
                "IsFallBuckScale INTEGER Default 0,\r\n" +
                "ExpansionFactor REAL Default 0.0,\r\n" +
                "TreeFactor REAL Default 0.0,\r\n" +
                "PointFactor REAL Default 0.0,\r\n" +
                "CreatedBy TEXT DEFAULT 'none',\r\n" +
                "CreatedDate DateTime DEFAULT(datetime('now')),\r\n" +
                "ModifiedBy TEXT,\r\n" +
                "ModifiedDate DateTime,\r\n" +
                "RowVersion INTEGER DEFAULT 0);" +
            "INSERT INTO new_Tree ( " +
                "Tree_CN,\r\n" +
                "Tree_GUID," +
                "TreeDefaultValue_CN,\r\n" +
                "Stratum_CN,\r\n" +
                "SampleGroup_CN,\r\n" +
                "CuttingUnit_CN,\r\n" +
                "Plot_CN,\r\n" +
                "TreeNumber,\r\n" +
                "Species,\r\n" +
                "CountOrMeasure,\r\n" +
                "TreeCount,\r\n" +
                "KPI,\r\n" +
                "STM,\r\n" +
                "SeenDefectPrimary,\r\n" +
                "SeenDefectSecondary,\r\n" +
                "RecoverablePrimary,\r\n" +
                "HiddenPrimary,\r\n" +
                "Initials,\r\n" +
                "LiveDead,\r\n" +
                "Grade,\r\n" +
                "HeightToFirstLiveLimb,\r\n" +
                "PoleLength,\r\n" +
                "ClearFace,\r\n" +
                "CrownRatio,\r\n" +
                "DBH,\r\n" +
                "DRC,\r\n" +
                "TotalHeight,\r\n" +
                "MerchHeightPrimary,\r\n" +
                "MerchHeightSecondary,\r\n" +
                "FormClass,\r\n" +
                "UpperStemDOB,\r\n" +
                "UpperStemDiameter,\r\n" +
                "UpperStemHeight,\r\n" +
                "DBHDoubleBarkThickness,\r\n" +
                "TopDIBPrimary,\r\n" +
                "TopDIBSecondary,\r\n" +
                "DefectCode,\r\n" +
                "DiameterAtDefect,\r\n" +
                "VoidPercent,\r\n" +
                "Slope,\r\n" +
                "Aspect,\r\n" +
                "Remarks,\r\n" +
                "XCoordinate,\r\n" +
                "YCoordinate,\r\n" +
                "ZCoordinate,\r\n" +
                "MetaData,\r\n" +
                "IsFallBuckScale,\r\n" +
                "ExpansionFactor,\r\n" +
                "TreeFactor,\r\n" +
                "PointFactor,\r\n" +
                "CreatedBy,\r\n" +
                "CreatedDate,\r\n" +
                "ModifiedBy,\r\n" +
                "ModifiedDate,\r\n" +
                "RowVersion " +
                ") " +
            " SELECT " +
                "Tree_CN,\r\n" +
                "Tree_GUID," +
                "TreeDefaultValue_CN,\r\n" +
                "Stratum_CN,\r\n" +
                "SampleGroup_CN,\r\n" +
                "CuttingUnit_CN,\r\n" +
                "Plot_CN,\r\n" +
                "TreeNumber,\r\n" +
                "Species,\r\n" +
                "CountOrMeasure,\r\n" +
                "TreeCount,\r\n" +
                "KPI,\r\n" +
                "STM,\r\n" +
                "SeenDefectPrimary,\r\n" +
                "SeenDefectSecondary,\r\n" +
                "RecoverablePrimary,\r\n" +
                "HiddenPrimary,\r\n" +
                "Initials,\r\n" +
                "LiveDead,\r\n" +
                "Grade,\r\n" +
                "HeightToFirstLiveLimb,\r\n" +
                "PoleLength,\r\n" +
                "ClearFace,\r\n" +
                "CrownRatio,\r\n" +
                "DBH,\r\n" +
                "DRC,\r\n" +
                "TotalHeight,\r\n" +
                "MerchHeightPrimary,\r\n" +
                "MerchHeightSecondary,\r\n" +
                "FormClass,\r\n" +
                "UpperStemDOB,\r\n" +
                "UpperStemDiameter,\r\n" +
                "UpperStemHeight,\r\n" +
                "DBHDoubleBarkThickness,\r\n" +
                "TopDIBPrimary,\r\n" +
                "TopDIBSecondary,\r\n" +
                "DefectCode,\r\n" +
                "DiameterAtDefect,\r\n" +
                "VoidPercent,\r\n" +
                "Slope,\r\n" +
                "Aspect,\r\n" +
                "Remarks,\r\n" +
                "XCoordinate,\r\n" +
                "YCoordinate,\r\n" +
                "ZCoordinate,\r\n" +
                "MetaData,\r\n" +
                "IsFallBuckScale,\r\n" +
                "ExpansionFactor,\r\n" +
                "TreeFactor,\r\n" +
                "PointFactor,\r\n" +
                "CreatedBy,\r\n" +
                "CreatedDate,\r\n" +
                "ModifiedBy,\r\n" +
                "ModifiedDate,\r\n" +
                "RowVersion " +
            "FROM Tree;\r\n" +
            "DROP Table Tree;\r\n" +
            "ALTER Table new_Tree RENAME TO Tree;\r\n" +
            "COMMIT;\r\n" +
            "PRAGMA primary_keys = on; " +
            treeTriggerDDL);

                SetDatabaseVersion(db, "2.1.2");
            }
            catch (Exception e)
            {
                throw new SchemaUpdateException(startVersion, "2.1.2", e);
            }
        }

        private static void UpdateTo_2_2_0(CruiseDatastore db)
        {
            var startVersion = db.DatabaseVersion;

            try
            {
                db.BeginTransaction();
                db.Execute(
@"CREATE TABLE LogGradeAuditRule (
Species TEXT,
DefectMax REAL Default 0.0,
ValidGrades TEXT);");
                SetDatabaseVersion(db, "2.2.0");
                db.CommitTransaction();
            }
            catch (Exception e)
            {
                db.RollbackTransaction();
                throw new SchemaUpdateException(startVersion, "2.2.0", e);
            }
        }

        private static void FixVersion_2_5_0(CruiseDatastore db)
        {
            var startVersion = db.DatabaseVersion;
            var targetVersion = "2.5.1.1";
            db.BeginTransaction();
            try
            {
                db.Execute("UPDATE Tree SET Tree_GUID = NULL WHERE typeof(Tree_GUID) = 'text' AND Tree_GUID NOT LIKE '________-____-____-____-____________';");
                db.Execute("UPDATE Log SET Log_GUID = NULL WHERE typeof(Log_GUID) = 'text' AND Log_GUID NOT LIKE '________-____-____-____-____________';");
                db.Execute("UPDATE Stem SET Stem_GUID = NULL WHERE typeof(Stem_GUID) = 'text' AND Stem_GUID NOT LIKE '________-____-____-____-____________';");
                db.Execute("UPDATE Plot SET Plot_GUID = NULL WHERE typeof(Plot_GUID) = 'text' AND Plot_GUID NOT LIKE '________-____-____-____-____________';");
                db.Execute("UPDATE Component SET GUID = NULL WHERE typeof(GUID) = 'text' AND GUID NOT LIKE '________-____-____-____-____________';");

                SetDatabaseVersion(db, targetVersion);
                db.CommitTransaction();
            }
            catch (Exception e)
            {
                db.RollbackTransaction();
                throw new SchemaUpdateException(startVersion, targetVersion, e);
            }
        }

        private static void UpdateTo_2_5_1(CruiseDatastore db)
        {
            var startVersion = db.DatabaseVersion;
            var targetVersion = "2.5.1";

            db.BeginTransaction();
            try
            {
                db.Execute("ALTER TABLE TreeCalculatedValues ADD COLUMN TipwoodVolume REAL Default 0.0;");
                db.Execute("ALTER TABLE LCD ADD COLUMN SumTipwood DOUBLE Default 0.0;");
                db.Execute("ALTER TABLE CuttingUnit  ADD COLUMN Rx TEXT;");
                db.Execute("ALTER TABLE Stratum ADD COLUMN SamplingFrequency INTEGER Default 0;");
                db.Execute("ALTER TABLE Stratum ADD COLUMN VolumeFactor REAL Default 0.333;");
                db.Execute("ALTER TABLE Plot ADD COLUMN ThreePRandomValue INTEGER Default 0;");

                SetDatabaseVersion(db, targetVersion);
                db.CommitTransaction();
            }
            catch (Exception e)
            {
                db.RollbackTransaction();
                throw new SchemaUpdateException(startVersion, targetVersion, e);
            }
        }

        private static void UpdateTo_2_6_1(CruiseDatastore db)
        {
            var version = db.DatabaseVersion;
            var targetVersion = "2.6.1";

            db.BeginTransaction();
            try
            {
                db.Execute(
@"CREATE TABLE SamplerState (
        SamplerState_CN INTEGER PRIMARY KEY AUTOINCREMENT, 
        SampleGroup_CN INTEGER NOT NULL,
        SampleSelectorType TEXT COLLATE NOCASE, 
        BlockState TEXT, 
        SystematicIndex INTEGER DEFAULT 0, 
        Counter INTEGER DEFAULT 0, 
        InsuranceIndex DEFAULT -1,
        InsuranceCounter DEFAULT -1,
        ModifiedDate DateTime,

        UNIQUE (SampleGroup_CN),

        FOREIGN KEY (SampleGroup_CN) REFERENCES SampleGroup (SampleGroup_CN) ON DELETE CASCADE ON UPDATE CASCADE
);");

                db.Execute(
@"CREATE TRIGGER SamplerState_OnUpdate 
    AFTER UPDATE OF 
        BlockState, 
        Counter, 
        InsuranceCounter 
    ON SamplerState 
    FOR EACH ROW 
    BEGIN 
        UPDATE SamplerState SET ModifiedDate = datetime('now', 'localtime') WHERE SamplerState_CN = old.SamplerState_CN;
    END;
");
                SetDatabaseVersion(db, targetVersion);
                db.CommitTransaction();
            }
            catch (Exception e)
            {
                db.RollbackTransaction();
                throw new SchemaUpdateException(version, targetVersion, e);
            }
        }

        private static void UpdateTo_2_7_0(CruiseDatastore db)
        {
            var version = db.DatabaseVersion;
            var targetVersion = "2.7.0";

            try
            {
                var treeTriggerDDL = GetTriggerDDL(db, "Tree");
                var plotTriggerDDL = GetTriggerDDL(db, "Plot");
                var stemTriggerDDL = GetTriggerDDL(db, "Stem");
                var logTriggerDDL = GetTriggerDDL(db, "Log");
                var treeEstimateDDL = GetTriggerDDL(db, "TreeEstimate");

                if (db.GetTableSQL("Tree").Contains("Tree_GUID TEXT UNIQUE"))
                {
                    db.OpenConnection();
                    db.Execute(
@"PRAGMA foreign_keys = off;
BEGIN;
CREATE TABLE new_tree (
    Tree_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    Tree_GUID TEXT,
    TreeDefaultValue_CN INTEGER REFERENCES TreeDefaultValue,
    Stratum_CN INTEGER REFERENCES Stratum NOT NULL,
    SampleGroup_CN INTEGER REFERENCES SampleGroup,
    CuttingUnit_CN INTEGER REFERENCES CuttingUnit NOT NULL,
    Plot_CN INTEGER REFERENCES Plot,
    TreeNumber INTEGER NOT NULL,
    Species TEXT,
    CountOrMeasure TEXT,
    TreeCount REAL Default 0.0,
    KPI REAL Default 0.0,
    STM TEXT Default 'N',
    SeenDefectPrimary REAL Default 0.0,
    SeenDefectSecondary REAL Default 0.0,
    RecoverablePrimary REAL Default 0.0,
    HiddenPrimary REAL Default 0.0,
    Initials TEXT,
    LiveDead TEXT,
    Grade TEXT,
    HeightToFirstLiveLimb REAL Default 0.0,
    PoleLength REAL Default 0.0,
    ClearFace TEXT,
    CrownRatio REAL Default 0.0,
    DBH REAL Default 0.0,
    DRC REAL Default 0.0,
    TotalHeight REAL Default 0.0,
    MerchHeightPrimary REAL Default 0.0,
    MerchHeightSecondary REAL Default 0.0,
    FormClass REAL Default 0.0,
    UpperStemDOB REAL Default 0.0,
    UpperStemDiameter REAL Default 0.0,
    UpperStemHeight REAL Default 0.0,
    DBHDoubleBarkThickness REAL Default 0.0,
    TopDIBPrimary REAL Default 0.0,
    TopDIBSecondary REAL Default 0.0,
    DefectCode TEXT,
    DiameterAtDefect REAL Default 0.0,
    VoidPercent REAL Default 0.0,
    Slope REAL Default 0.0,
    Aspect REAL Default 0.0,
    Remarks TEXT,
    XCoordinate DOUBLE Default 0.0,
    YCoordinate DOUBLE Default 0.0,
    ZCoordinate DOUBLE Default 0.0,
    MetaData TEXT,
    IsFallBuckScale INTEGER Default 0,
    ExpansionFactor REAL Default 0.0,
    TreeFactor REAL Default 0.0,
    PointFactor REAL Default 0.0,
    CreatedBy TEXT DEFAULT 'none',
    CreatedDate DateTime DEFAULT(datetime(current_timestamp, 'localtime')),
    ModifiedBy TEXT,
    ModifiedDate DateTime,
    somtthing text,
    RowVersion INTEGER DEFAULT 0);
INSERT INTO new_Tree (
    Tree_CN,
    Tree_GUID,
    TreeDefaultValue_CN,
    Stratum_CN,
    SampleGroup_CN,
    CuttingUnit_CN,
    Plot_CN,
    TreeNumber,
    Species,
    CountOrMeasure,
    TreeCount,
    KPI,
    STM,
    SeenDefectPrimary,
    SeenDefectSecondary,
    RecoverablePrimary,
    HiddenPrimary,
    Initials,
    LiveDead,
    Grade,
    HeightToFirstLiveLimb,
    PoleLength,
    ClearFace,
    CrownRatio,
    DBH,
    DRC,
    TotalHeight,
    MerchHeightPrimary,
    MerchHeightSecondary,
    FormClass,
    UpperStemDOB,
    UpperStemDiameter,
    UpperStemHeight,
    DBHDoubleBarkThickness,
    TopDIBPrimary,
    TopDIBSecondary,
    DefectCode,
    DiameterAtDefect,
    VoidPercent,
    Slope,
    Aspect,
    Remarks,
    XCoordinate,
    YCoordinate,
    ZCoordinate,
    MetaData,
    IsFallBuckScale,
    ExpansionFactor,
    TreeFactor,
    PointFactor,
    CreatedBy,
    CreatedDate,
    ModifiedBy,
    ModifiedDate,
    RowVersion)
 SELECT 
    Tree_CN,
    Tree_GUID,
    TreeDefaultValue_CN,
    Stratum_CN,
    SampleGroup_CN,
    CuttingUnit_CN,
    Plot_CN,
    TreeNumber,
    Species,
    CountOrMeasure,
    TreeCount,
    KPI,
    STM,
    SeenDefectPrimary,
    SeenDefectSecondary,
    RecoverablePrimary,
    HiddenPrimary,
    Initials,
    LiveDead,
    Grade,
    HeightToFirstLiveLimb,
    PoleLength,
    ClearFace,
    CrownRatio,
    DBH,
    DRC,
    TotalHeight,
    MerchHeightPrimary,
    MerchHeightSecondary,
    FormClass,
    UpperStemDOB,
    UpperStemDiameter,
    UpperStemHeight,
    DBHDoubleBarkThickness,
    TopDIBPrimary,
    TopDIBSecondary,
    DefectCode,
    DiameterAtDefect,
    VoidPercent,
    Slope,
    Aspect,
    Remarks,
    XCoordinate,
    YCoordinate,
    ZCoordinate,
    MetaData,
    IsFallBuckScale,
    ExpansionFactor,
    TreeFactor,
    PointFactor,
    CreatedBy,
    CreatedDate,
    ModifiedBy,
    ModifiedDate,
    RowVersion
FROM Tree;
DROP Table Tree;
ALTER TABLE new_Tree RENAME TO Tree;

CREATE TABLE new_plot (
    Plot_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    Plot_GUID TEXT,
    Stratum_CN INTEGER REFERENCES Stratum NOT NULL,
    CuttingUnit_CN INTEGER REFERENCES CuttingUnit NOT NULL,
    PlotNumber INTEGER NOT NULL,
    IsEmpty TEXT,
    Slope REAL Default 0.0,
    KPI REAL Default 0.0,
    Aspect REAL Default 0.0,
    Remarks TEXT,
    XCoordinate REAL Default 0.0,
    YCoordinate REAL Default 0.0,
    ZCoordinate REAL Default 0.0,
    MetaData TEXT,
    Blob BLOB,
    ThreePRandomValue INTEGER Default 0,
    CreatedBy TEXT DEFAULT 'none',
    CreatedDate DateTime DEFAULT (datetime(current_timestamp, 'localtime')) ,
    ModifiedBy TEXT ,
    ModifiedDate DateTime ,
    RowVersion INTEGER DEFAULT 0,
    UNIQUE (Stratum_CN, CuttingUnit_CN, PlotNumber));
INSERT INTO new_plot (
    Plot_CN,
    Plot_GUID,
    Stratum_CN,
    CuttingUnit_CN,
    PlotNumber,
    IsEmpty,
    Slope,
    KPI,
    Aspect,
    Remarks,
    XCoordinate,
    YCoordinate,
    ZCoordinate,
    MetaData,
    Blob,
    ThreePRandomValue,
    CreatedBy,
    CreatedDate,
    ModifiedBy,
    ModifiedDate,
    RowVersion)
SELECT 
    Plot_CN,
    Plot_GUID,
    Stratum_CN,
    CuttingUnit_CN,
    PlotNumber,
    IsEmpty,
    Slope,
    KPI,
    Aspect,
    Remarks,
    XCoordinate,
    YCoordinate,
    ZCoordinate,
    MetaData,
    Blob,
    ThreePRandomValue,
    CreatedBy,
    CreatedDate,
    ModifiedBy,
    ModifiedDate,
    RowVersion
FROM Plot;
DROP TABLE Plot;
ALTER TABLE new_plot RENAME TO Plot;

CREATE TABLE new_log (
	Log_CN INTEGER PRIMARY KEY AUTOINCREMENT,
	Log_GUID TEXT,
	Tree_CN INTEGER REFERENCES Tree NOT NULL,
	LogNumber TEXT NOT NULL,
	Grade TEXT,
	SeenDefect REAL Default 0.0,
	PercentRecoverable REAL Default 0.0,
	Length INTEGER Default 0,
	ExportGrade TEXT,
	SmallEndDiameter REAL Default 0.0,
	LargeEndDiameter REAL Default 0.0,
	GrossBoardFoot REAL Default 0.0,
	NetBoardFoot REAL Default 0.0,
	GrossCubicFoot REAL Default 0.0,
	NetCubicFoot REAL Default 0.0,
	BoardFootRemoved REAL Default 0.0,
	CubicFootRemoved REAL Default 0.0,
	DIBClass REAL Default 0.0,
	BarkThickness REAL Default 0.0,
	CreatedBy TEXT DEFAULT 'none',
	CreatedDate DateTime DEFAULT (datetime(current_timestamp, 'localtime')) ,
	ModifiedBy TEXT,
	ModifiedDate DateTime,
	RowVersion INTEGER DEFAULT 0,
	UNIQUE (Tree_CN, LogNumber));
INSERT INTO new_log (
    Log_CN,
	Log_GUID,
	Tree_CN,
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
	CreatedDate,
	ModifiedBy,
	ModifiedDate,
	RowVersion)
SELECT 
    Log_CN,
	Log_GUID,
	Tree_CN,
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
	CreatedDate,
	ModifiedBy,
	ModifiedDate,
	RowVersion
FROM Log;
DROP TABLE Log;
ALTER TABLE new_log RENAME TO Log;

CREATE TABLE new_stem (
    Stem_CN INTEGER PRIMARY KEY AUTOINCREMENT,
	Stem_GUID TEXT UNIQUE,
	Tree_CN INTEGER REFERENCES Tree,
	Diameter REAL Default 0.0,
	DiameterType TEXT,
	CreatedBy TEXT DEFAULT 'none',
	CreatedDate DateTime DEFAULT (datetime(current_timestamp, 'localtime')) ,
	ModifiedBy TEXT ,
	ModifiedDate DateTime ,
	RowVersion INTEGER DEFAULT 0,
	UNIQUE (Tree_CN));
INSERT INTO new_stem (
    Stem_CN,
    Stem_GUID,
    Tree_CN,
    Diameter,
    DiameterType,
    CreatedBy,
    CreatedDate,
    ModifiedBy,
    ModifiedDate,
    RowVersion)
SELECT 
    Stem_CN,
    Stem_GUID,
    Tree_CN,
    Diameter,
    DiameterType,
    CreatedBy,
    CreatedDate,
    ModifiedBy,
    ModifiedDate,
    RowVersion
FROM Stem;
DROP TABLE Stem;
ALTER TABLE new_stem RENAME TO Stem;

CREATE TABLE new_treeEstimate (
    TreeEstimate_CN INTEGER PRIMARY KEY AUTOINCREMENT,
	CountTree_CN INTEGER REFERENCES CountTree,
	TreeEstimate_GUID TEXT UNIQUE,
	KPI REAL NOT NULL,
	CreatedBy TEXT DEFAULT 'none',
	CreatedDate DateTime DEFAULT (datetime(current_timestamp, 'localtime')) ,
	ModifiedBy TEXT ,
	ModifiedDate DateTime);
INSERT INTO new_treeEstimate (
    TreeEstimate_CN,
	CountTree_CN,
	TreeEstimate_GUID,
	KPI,
	CreatedBy,
	CreatedDate,
	ModifiedBy,
	ModifiedDate)
SELECT 
    TreeEstimate_CN,
	CountTree_CN,
	TreeEstimate_GUID,
	KPI,
	CreatedBy,
	CreatedDate,
	ModifiedBy,
	ModifiedDate
FROM TreeEstimate;
DROP TABLE TreeEstimate;
ALTER TABLE new_treeEstimate RENAME TO TreeEstimate;
" +
 treeTriggerDDL + ";\r\n" +
plotTriggerDDL + ";\r\n" +
logTriggerDDL + ";\r\n" +
stemTriggerDDL + ";\r\n" +
treeEstimateDDL + ";\r\n" +
@"PRAGMA foreign_keys = on;
COMMIT;");



                    //db.Execute();
                    db.ReleaseConnection();
                }

                SetDatabaseVersion(db, targetVersion);
            }
            catch (Exception e)
            {
                throw new SchemaUpdateException(version, targetVersion, e);
            }
        }

        private static void UpdateTo_2_7_1(CruiseDatastore db)
        {
            var version = db.DatabaseVersion;
            var targetVersion = "2.7.1";

            try
            {
                db.Execute(
@"DROP TABLE TreeEstimate;
CREATE TABLE TreeEstimate (
			TreeEstimate_CN INTEGER PRIMARY KEY AUTOINCREMENT,
			CountTree_CN INTEGER,
			TreeEstimate_GUID TEXT,
			KPI REAL NOT NULL,
			CreatedBy TEXT DEFAULT 'none',
			CreatedDate DateTime DEFAULT (datetime(current_timestamp, 'localtime')) ,
			ModifiedBy TEXT ,
			ModifiedDate DateTime );");

                db.ReleaseConnection();


                SetDatabaseVersion(db, targetVersion);
            }
            catch (Exception e)
            {
                throw new SchemaUpdateException(version, targetVersion, e);
            }
        }

        private static void UpdateTo_2_7_3(CruiseDatastore db)
        {
            var startVersion = db.DatabaseVersion;
            var targetVersion = "2.7.3";

            try
            {
                db.BeginTransaction();
                db.Execute("DROP VIEW IF EXISTS CountTree_View;");
                db.Execute("DROP VIEW IF EXISTS StratumAcres_View;");
                SetDatabaseVersion(db, targetVersion);
                db.CommitTransaction();
            }
            catch (Exception e)
            {
                db.RollbackTransaction();
                throw new SchemaUpdateException(startVersion, targetVersion, e);
            }
        }

        public static bool CheckCanUpdate(string version)
        {
            if (version.StartsWith("2"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void FixTreeAuditValueFKeyErrors(Datastore db)
        {
            if (db.HasForeignKeyErrors("TreeDefaultValueTreeAuditValue"))
            {
                db.BeginTransaction();
                try
                {
                    db.Execute("DELETE FROM TreeDefaultValueTreeAuditValue WHERE TreeDefaultValue_CN NOT IN (Select TreeDefaultValue_CN FROM TreeDefaultValue);");
                    db.Execute("DELETE FROM TreeDefaultValueTreeAuditValue WHERE TreeAuditValue_CN NOT IN (SELECT TreeAuditValue_CN FROM TreeAuditValue);");
                    db.CommitTransaction();
                }
                catch
                {
                    db.RollbackTransaction();
                }
            }
        }

        private static String[] ListTriggers(Datastore db)
        {
            var result = db.ExecuteScalar("SELECT group_concat(name,',') FROM sqlite_master WHERE type LIKE 'trigger';") as string;
            if (string.IsNullOrEmpty(result)) { return new string[0]; }
            else
            {
                return result.Split(',');
            }
        }

        private static String[] ListTriggers(DbConnection connection, DbTransaction transaction)
        {
            var result = connection.ExecuteScalar("SELECT group_concat(name,',') FROM sqlite_master WHERE type LIKE 'trigger';", null, transaction) as string;
            if (string.IsNullOrEmpty(result)) { return new string[0]; }
            else
            {
                return result.Split(',');
            }
        }

        public static string GetTriggerDDL(Datastore db, string tableName)
        {
            var getTriggers = String.Format("SELECT group_concat(sql,';\r\n') FROM sqlite_master WHERE tbl_name LIKE '{0}' and type LIKE 'trigger';", tableName);
            return db.ExecuteScalar(getTriggers) as string;
        }
    }
}