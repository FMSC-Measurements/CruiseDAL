using Backpack.SqlBuilder;
using FMSC.ORM;
using System;

namespace CruiseDAL
{
    public static partial class Updater
    {
        public static void Update(DAL db)
        {
            //PatchSureToMeasure(db);

            //killswitch
            //if the version is not 2.* or not using date versioning
            if (!CheckCanUpdate(db))
            {
                throw new IncompatibleSchemaException("The version of this cruise file is not compatible with the version of the software you are using." +
                    "Go to github.com/FMSC-Measurements to get the latest version of our software.", null);
            }

            if (db.DatabaseVersion.StartsWith("2"))
            {
                //if (db.DatabaseVersion == "2013.05.28" || db.DatabaseVersion == "Unknown")
                //{
                //    UpdateToVersion2013_05_30(db);
                //}

                //if (db.DatabaseVersion == "2013.05.30")
                //{
                //    UpdateToVersion2013_06_12(db);
                //}

                //if (db.DatabaseVersion == "2013.06.12" || db.DatabaseVersion == "2013.06.13")
                //{
                //    UpdateToVersion2013_06_17(db);
                //}

                //if (db.DatabaseVersion == "2013.06.17" || db.DatabaseVersion == "2013.06.18")
                //{
                //    UpdateToVersion2013_06_19(db);
                //}

                //if (db.DatabaseVersion == "2013.06.19")
                //{
                //    UpdateVersion2013_06_19(db);
                //    UpdateToVersion2013_08_02(db);
                //}

                //if (db.DatabaseVersion == "2013.08.02")
                //{
                //    UpdateToVersion2013_08_29(db);
                //}

                //if (db.DatabaseVersion == "2013.08.29")
                //{
                //    UpdateToVersion2013_10_29(db);
                //}

                //if (db.DatabaseVersion == "2013.10.29")
                //{
                //    UpdateToVersion2013_11_01(db);
                //}

                //if (db.DatabaseVersion == "2013.11.01")
                //{
                //    UpdateToVersion2013_11_22(db);
                //}

                //if (db.DatabaseVersion == "2013.11.22")
                //{
                //    UpdateToVersion2014_01_21(db);
                //}
                //if (db.DatabaseVersion == "2014.01.21")
                //{
                //    UpdateToVersion2014_03_12(db);
                //}
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
                if (db.DatabaseVersion.StartsWith("2.2."))
                {
                    UpdateTo_2_5_0(db);
                }
            }
            else if (db.DatabaseVersion.StartsWith("3."))
            {
                //no updates yet
            }

            FixTreeAuditValueFKeyErrors(db);

            CleanupErrorLog(db);
        }

        public static void UpdateMajorVersion(DAL db)
        {
            var version = db.DatabaseVersion;

            if (version.StartsWith("2"))
            {
                UpdateTo_3_0(db);
            }
        }

        public static void UpdateToVersion2015_04_28(DAL db)
        {
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
                throw new SchemaUpdateException(db.DatabaseVersion, "2015.04.28", e);
            }
        }

        public static void UpdateToVersion2015_08_03(DAL db)
        {
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
                throw new SchemaUpdateException(db.DatabaseVersion, "2015.08.03", e);
            }
        }

        private static void UpdateToVersion2015_08_19(DAL db)
        {
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
                throw new SchemaUpdateException(db.DatabaseVersion, "2015.08.19", e);
            }
        }

        //patch for some a version that got out in the wild with bad triggers
        private static void UpdateToVersion2015_09_01(DAL db)
        {
            db.BeginTransaction();
            try
            {
                //because there are a lot of changes with triggers
                //lets just recreate all triggers
                foreach (string trigName in ListTriggers(db))
                {
                    db.Execute("DROP TRIGGER " + trigName + ";");
                }

                db.Execute(Schema.Schema.CREATE_TRIGGERS);

                SetDatabaseVersion(db, "2015.09.01");
                db.CommitTransaction();
            }
            catch (Exception e)
            {
                db.RollbackTransaction();
                throw new SchemaUpdateException(db.DatabaseVersion, "2015.09.01", e);
            }
        }

        private static void UpdateTo_2_1_1(DAL db)
        {
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
                throw new SchemaUpdateException(db.DatabaseVersion, "2.1.1", e);
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
        private static void UpdateTo_2_1_2(DAL db)
        {
            db.Execute("PRAGMA primary_keys = off;");

            db.BeginTransaction();
            try
            {
                var treeTriggerDDL = GetTriggerDDL(db, "Tree");

                db.Execute(
            "CREATE TABLE new_Tree (\r\n" +
                "Tree_CN INTEGER PRIMARY KEY AUTOINCREMENT,\r\n" +
                "Tree_GUID TEXT," +
                "TreeDefaultValue_CN INTEGER REFERENCES TreeDefaultValue,\r\n" +
                "Stratum_CN INTEGER REFERENCES Stratum NOT NULL,\r\n" +
                "SampleGroup_CN INTEGER REFERENCES SampleGroup," +
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
            "INSERT INTO new_Tree SELECT * FROM Tree;\r\n" +
            "DROP Table Tree;\r\n" +
            "ALTER Table new_Tree RENAME TO Tree;\r\n" +
            treeTriggerDDL);

                SetDatabaseVersion(db, "2.1.2");
                db.CommitTransaction();

                db.Execute("PRAGMA primary_keys = on;");
            }
            catch (Exception e)
            {
                db.RollbackTransaction();
                throw new SchemaUpdateException(db.DatabaseVersion, "2.1.2", e);
            }
        }

        private static void UpdateTo_2_2_0(DAL db)
        {
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
                throw new SchemaUpdateException(db.DatabaseVersion, "2.2.0", e);
            }
        }

        private static void UpdateTo_2_5_0(DAL db)
        {
            db.BeginTransaction();
            try
            {
                db.Execute("ALTER TABLE TreeCalculatedValues ADD COLUMN TipwoodVolume REAL Default 0.0;");
                db.Execute("ALTER TABLE LCD ADD COLUMN SumTipwood DOUBLE Default 0.0;");
                db.Execute("ALTER TABLE CuttingUnit  ADD COLUMN Rx TEXT;");
                db.Execute("ALTER TABLE Stratum ADD COLUMN SamplingFrequency INTEGER Default 0;");
                db.Execute("ALTER TABLE Stratum ADD COLUMN VolumeFactor REAL Default 0.333;");
                db.Execute("ALTER TABLE Plot ADD COLUMN ThreePRandomValue INTEGER Default 0;");

                SetDatabaseVersion(db, "2.5.0");
                db.CommitTransaction();
            }
            catch (Exception e)
            {
                db.RollbackTransaction();
                throw new SchemaUpdateException(db.DatabaseVersion, "2.5.0", e);
            }
        }

        //private const string CREATE_TABLE_TALLY_LEDGER_COMMAND =
        //    "CREATE TABLE TallyLedger " +
        //    "( " +
        //    "TallyLedgerID TEXT PRIMARY KEY, " + //guid
        //    "UnitCode TEXT NOT NULL, " +
        //    "StratumCode TEXT NOT NULL, " +
        //    "SampleGroupCode TEXT NOT NULL, " +
        //    "PlotNumber INTEGER," +
        //    "Species TEXT, " +
        //    "LiveDead TEXT, " +
        //    "TreeCount INTEGER NOT NULL, " +
        //    "KPI INTEGER DEFAULT 0, " +
        //    "ThreePRandomValue INTEGER DEFAULT 0, " +
        //    "Tree_GUID TEXT REFERENCES Tree (Tree_GUID) ON DELETE CASCADE, " +
        //    "TimeStamp TEXT DEFAULT (datetime('now', 'localtime')), " +
        //    "Signature TEXT, " +
        //    "Reason TEXT, " +
        //    "Remarks TEXT, " +
        //    "EntryType TEXT" +
        //    ");";

        private const string REBUILD_TREE_TABLE =
            //"CREATE TEMP TABLE sqlite_master_temp AS SELECT * FROM sqlite_master WHERE Name = 'Tree';\r\n" +
            "CREATE TABLE new_Tree ( " +
                "Tree_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "Tree_GUID TEXT UNIQUE , " + //added unique constraint
                "TreeDefaultValue_CN INTEGER REFERENCES TreeDefaultValue, " +
                "Stratum_CN INTEGER REFERENCES Stratum NOT NULL, " +
                "SampleGroup_CN INTEGER REFERENCES SampleGroup, " +
                "CuttingUnit_CN INTEGER REFERENCES CuttingUnit NOT NULL, " +
                "Plot_CN INTEGER REFERENCES Plot, " +
                "TreeNumber INTEGER NOT NULL, " +
                "Species TEXT, " +
                "CountOrMeasure TEXT, " +
                "TreeCount REAL Default 0.0, " +
                "KPI REAL Default 0.0, " +
                "STM TEXT Default 'N', " +
                "SeenDefectPrimary REAL Default 0.0, " +
                "SeenDefectSecondary REAL Default 0.0, " +
                "RecoverablePrimary REAL Default 0.0, " +
                "HiddenPrimary REAL Default 0.0, " +
                "Initials TEXT, " +
                "LiveDead TEXT, " +
                "Grade TEXT, " +
                "HeightToFirstLiveLimb REAL Default 0.0, " +
                "PoleLength REAL Default 0.0, " +
                "ClearFace TEXT, " +
                "CrownRatio REAL Default 0.0, " +
                "DBH REAL Default 0.0, " +
                "DRC REAL Default 0.0, " +
                "TotalHeight REAL Default 0.0, " +
                "MerchHeightPrimary REAL Default 0.0, " +
                "MerchHeightSecondary REAL Default 0.0, " +
                "FormClass REAL Default 0.0, " +
                "UpperStemDOB REAL Default 0.0, " +
                "UpperStemDiameter REAL Default 0.0, " +
                "UpperStemHeight REAL Default 0.0, " +
                "DBHDoubleBarkThickness REAL Default 0.0, " +
                "TopDIBPrimary REAL Default 0.0, " +
                "TopDIBSecondary REAL Default 0.0, " +
                "DefectCode TEXT, " +
                "DiameterAtDefect REAL Default 0.0, " +
                "VoidPercent REAL Default 0.0, " +
                "Slope REAL Default 0.0, " +
                "Aspect REAL Default 0.0, " +
                "Remarks TEXT, " +
                "XCoordinate DOUBLE Default 0.0, " +
                "YCoordinate DOUBLE Default 0.0, " +
                "ZCoordinate DOUBLE Default 0.0, " +
                "MetaData TEXT, " +
                "IsFallBuckScale INTEGER Default 0, " +
                "ExpansionFactor REAL Default 0.0, " +
                "TreeFactor REAL Default 0.0, " +
                "PointFactor REAL Default 0.0, " +
                "CreatedBy TEXT DEFAULT 'none', " +
                "CreatedDate DateTime DEFAULT (datetime('now', 'localtime')) , " + //date time changed
                "ModifiedBy TEXT, " +
                "ModifiedDate DateTime , " +
                "RowVersion INTEGER DEFAULT 0);\r\n" +
            "INSERT INTO new_Tree SELECT * FROM Tree;\r\n" +
            "DROP Table Tree;\r\n" +
            "ALTER Table new_Tree RENAME TO Tree;\r\n";

        //private const string CREATE_VIEW_TALLY_POPULATION =
        //    "CREATE VIEW TallyPopulation " +
        //    "( StratumCode, SampleGroupCode, Species, LiveDead, Description, HotKey) " +
        //    "AS " +
        //    "SELECT Stratum.Code, SampleGroup.Code, TDV.Species, TDV.LiveDead, Tally.Description, Tally.HotKey  " +
        //    "FROM CountTree " +
        //    "JOIN SampleGroup USING (SampleGroup_CN) " +
        //    "JOIN Stratum USING (Stratum_CN) " +
        //    "LEFT JOIN TreeDefaultValue AS TDV USING (TreeDefaultValue_CN) " +
        //    "JOIN Tally USING (Tally_CN) " +
        //    "GROUP BY SampleGroup_CN, ifnull(TreeDefaultValue_CN, '');";

        private const string INITIALIZE_TALLY_LEDGER_FROM_COUNTTREE =
            "INSERT INTO TallyLedger " +
            "(TallyLedgerID, UnitCode, StratumCode, SampleGroupCode, Species, LiveDead, TreeCount, KPI, EntryType) " +
            "SELECT " +
            "(lower(hex(randomblob(4))) || '-' || lower(hex(randomblob(2))) || '-4' || substr(lower(hex(randomblob(2))),2) || '-' || substr('89ab',abs(random()) % 4 + 1, 1) || substr(lower(hex(randomblob(2))),2) || '-' || lower(hex(randomblob(6)))), " +
            "CuttingUnit.Code AS UnitCode, " +
            "Stratum.Code AS StratumCode, " +
            "SampleGroup.Code AS SampleGroupCode, " +
            "TDV.Species AS Species, " +
            "TDV.LiveDead AS LiveDead, " +
            "Sum(TreeCount) AS TreeCount, " +
            "Sum(SumKPI) AS SumKPI, " +
            "'utility' AS EntryType " +
            "FROM CountTree " +
            "JOIN CuttingUnit USING (CuttingUnit_CN) " +
            "JOIN SampleGroup USING (SampleGroup_CN) " +
            "JOIN Stratum USING (Stratum_CN) " +
            "LEFT JOIN TreeDefaultValue AS TDV USING (TreeDefaultValue_CN) " +
            "GROUP BY CuttingUnit.Code, Stratum.Code, SampleGroup.Code, ifnull(TDV.Species, 0), ifnull(TDV.LiveDead, 0), Component_CN;";

        public static void UpdateTo_3_0(DAL db)
        {
            db.Execute("PRAGMA foreign_keys=OFF;");
            db.BeginTransaction();
            try
            {
                db.Execute(Schema.Schema.CREATE_VIEW_TALLY_POPULATION);
                db.Execute(REBUILD_TREE_TABLE);
                db.Execute(Schema.Schema.CREATE_TABLE_TALLY_LEDGER_COMMAND);
                db.Execute(INITIALIZE_TALLY_LEDGER_FROM_COUNTTREE);
                SetDatabaseVersion(db, "3.0.0");
                db.CommitTransaction();
                db.Execute("PRAGMA foreign_keys=ON;");
            }
            catch (Exception e)
            {
                db.RollbackTransaction();
                throw new SchemaUpdateException(db.DatabaseVersion, "3.0.0", e);
            }
        }
    }
}