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
            Updater_V2.Update_Impl(datastore);
        }

        public static void Update_Impl(CruiseDatastore db)
        {
            //PatchSureToMeasure(db);

            if (!CheckCanUpdate(db))
            {
                throw new IncompatibleSchemaException("The version of this cruise file is not compatible with the version of the software you are using." +
                    "Go to github.com/FMSC-Measurements to get the latest version of our software.", null);
            }

            if (db.DatabaseVersion.StartsWith("2013") || db.DatabaseVersion == "2014.01.21")
            {
                throw new IncompatibleSchemaException("The version of this cruise file is no longer supported." +
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
            if(db.DatabaseVersion.StartsWith("2.5.0"))
            {
                FixVersion_2_5_0(db);
            }
            if (db.DatabaseVersion.StartsWith("2.2."))
            {
                UpdateTo_2_5_1(db);
            }

            CleanupErrorLog(db);

            FixTreeAuditValueFKeyErrors(db);
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

        public static bool CheckNeedsMajorUpdate(CruiseDatastore dal)
        {
            var version = dal.DatabaseVersion;

            if (version.StartsWith("2"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CheckCanUpdate(CruiseDatastore dal)
        {
            var version = dal.DatabaseVersion;

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