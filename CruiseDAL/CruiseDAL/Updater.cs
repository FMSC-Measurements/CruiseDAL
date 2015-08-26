using System;
using CruiseDAL.DataObjects;

namespace CruiseDAL
{
    internal static class Updater
    {
        public static void Update(DAL db)
        {
            //PatchSureToMeasure(db);

            if (string.IsNullOrEmpty(db.DatabaseVersion))
            {
                UpdateToVersion2013_05_28(db);
            }

            if (db.DatabaseVersion == "2013.05.28")
            {
                UpdateToVersion2013_05_30(db);
            }

            if (db.DatabaseVersion == "2013.05.30")
            {
                UpdateToVersion2013_06_12(db);
            }

            if (db.DatabaseVersion == "2013.06.12" || db.DatabaseVersion == "2013.06.13")
            {
                UpdateToVersion2013_06_17(db);
            }

            if (db.DatabaseVersion == "2013.06.17" || db.DatabaseVersion == "2013.06.18")
            {
                UpdateToVersion2013_06_19(db);
            }

            if (db.DatabaseVersion == "2013.06.19")
            {
                UpdateVersion2013_06_19(db);
                UpdateToVersion2013_08_02(db);
            }

            if (db.DatabaseVersion == "2013.08.02")
            {
                UpdateToVersion2013_08_29(db);
            }

            if (db.DatabaseVersion == "2013.08.29")
            {
                UpdateToVersion2013_10_29(db);
            }

            if (db.DatabaseVersion == "2013.10.29")
            {
                UpdateToVersion2013_11_01(db);
            }

            if (db.DatabaseVersion == "2013.11.01")
            {
                UpdateToVersion2013_11_22(db);
            }

            if (db.DatabaseVersion == "2013.11.22")
            {
                UpdateToVersion2014_01_21(db);
            }
            if (db.DatabaseVersion == "2014.01.21")
            {
                UpdateToVersion2014_03_12(db);
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

            if (HasBadTreeAuditValueTable(db))
            {
                try
                {
                    db.BeginTransaction();
                    RebuildTable(db, "TreeAuditValue",
                        @"CREATE TABLE TreeAuditValue (
				TreeAuditValue_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				Field TEXT NOT NULL,
				Min REAL Default 0.0,
				Max REAL Default 0.0,
				ValueSet TEXT,
				Required BOOLEAN Default 0,
				ErrorMessage TEXT);", "Field, Min, Max, ValueSet, Required, ErrorMessage");
                    db.EndTransaction();
                }
                catch (Exception e)
                {
                    db.CancelTransaction();
                    throw new DatabaseExecutionException("failed fixing TreeAuditValue table", e);
                }
            }

            if (db.HasForeignKeyErrors(Schema.TREEDEFAULTVALUETREEAUDITVALUE._NAME))
            {
                try
                {
                    db.BeginTransaction();
                    db.Execute("DELETE FROM TreeDefaultValueTreeAuditValue WHERE TreeDefaultValue_CN NOT IN (Select TreeDefaultValue_CN FROM TreeDefaultValue);");
                    db.Execute("DELETE FROM TreeDefaultValueTreeAuditValue WHERE TreeAuditValue_CN NOT IN (SELECT TreeAuditValue_CN FROM TreeAuditValue);");
                    db.EndTransaction();
                }
                catch 
                {
                    db.CancelTransaction();
                }
            }

            foreach (ErrorLogDO el in db.Read<ErrorLogDO>("ErrorLog","WHERE CN_Number != 0"))
            {
                InsureErrorLogEntry(db, el);
            }

        }

        private static void RebuildTable(DAL db, String tableName, String newTableDef, String columnList)
        {
            //get all triggers accocated with table so we can recreate them later
            string getTriggers = String.Format("SELECT group_concat(sql,';\r\n') FROM sqlite_master WHERE tbl_name LIKE '{0}' and type LIKE 'trigger';", tableName);
            string triggers = db.ExecuteScalar(getTriggers) as string;

            //rename existing table
            string command = String.Format("ALTER TABLE {0} RENAME TO {0}temp;", tableName);
            db.Execute(command);

            //create rebuilt table
            command = String.Format("{0};", newTableDef);
            db.Execute(command);

            //copy data from existing table to rebuilt table
            command = String.Format("INSERT INTO {0} ({1}) SELECT {1} FROM {0}temp;", tableName, columnList);
            db.Execute(command);

            command = String.Format("DROP TABLE {0}temp;",tableName);
            db.Execute(command);

            //recreate treggers
            if (triggers != null)
            {
                db.Execute(triggers);
            }
        }

        private static void SetDatabaseVersion(DAL db, string newVersion)
        {
            string command = String.Format("UPDATE Globals SET Value = '{0}' WHERE Block = 'Database' AND Key = 'Version';", newVersion);
            db.Execute(command);
            db.LogMessage(String.Format("Updated structure version to {0}", newVersion), "I");
            db.DatabaseVersion = newVersion;
        }

        private static void InsureErrorLogEntry(DAL db, ErrorLogDO el)
        {

            if (db.GetRowCount(el.TableName, "WHERE rowID = ?", el.CN_Number) == 0)
            {
                el.Delete();
            }
        }

        private static void PatchSureToMeasure(DAL db)
        {
            string command = @"UPDATE TreeFieldSetup SET Field = 'STM' WHERE Field = 'SureToMeasure';
                               UPDATE TreeFieldSetupDefault SET Field = 'STM' WHERE Field = 'SureToMeasure';";
            db.Execute(command);
        }

        public static bool HasBadTreeAuditValueTable(DAL db)
        {
            object obj = db.ExecuteScalar("pragma foreign_key_list(TreeAuditValue);");
            return  obj != null && (long)obj == 0;
        }



        private static void UpdateToVersion2013_05_28(DAL db)
        {
            try
            {
                db.BeginTransaction();
                string command = "DROP TABLE IF EXISTS TreeAudit;";
                db.Execute(command);
                command = @"CREATE TABLE IF NOT EXISTS TreeDefaultValueTreeAuditValue (
	                        TreeAuditValue_CN INTEGER REFERENCES TreeAuditValue NOT NULL,
	                        TreeDefaultValue_CN INTEGER REFERENCES TreeDefaultValue NOT NULL);";
                db.Execute(command);
                command = @"DROP TABLE IF EXISTS TreeAuditValue;";
                db.Execute(command);
                command = @"CREATE TABLE IF NOT EXISTS TreeAuditValue (
	                        TreeAuditValue_CN INTEGER PRIMARY KEY AUTOINCREMENT REFERENCES TreeAudit,
	                        Field TEXT NOT NULL,
	                        Min REAL Default 0.0,
	                        Max REAL Default 0.0,
	                        ValueSet TEXT,
	                        Required BOOL,
	                        ErrorMessage TEXT);";
                db.Execute(command);

                SetDatabaseVersion(db, "2013.05.28");
                db.EndTransaction();
            }
            catch (Exception e)
            {
                db.CancelTransaction();
                throw new DatabaseExecutionException("failed updateing database to version 2013.05.28", e);
            }
        }

        private static void UpdateToVersion2013_05_30(DAL db)
        {
            try
            {
                db.BeginTransaction();
                string command = @"
                                    CREATE TABLE TempPlot (
				                        Plot_CN INTEGER PRIMARY KEY AUTOINCREMENT,
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
				                        CreatedBy TEXT NOT NULL,
				                        CreatedDate DATETIME,
				                        ModifiedBy TEXT,
				                        ModifiedDate DATETIME,
				                        UNIQUE (Stratum_CN, CuttingUnit_CN, PlotNumber));
                                    INSERT INTO TempPlot (PlotNumber, IsEmpty, Slope, KPI, Aspect, Remarks, XCoordinate, YCoordinate, ZCoordinate, MetaData, Blob, Stratum_CN, CuttingUnit_CN, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
                                    SELECT PlotNumber, IsEmpty, Slope, KPI, Aspect, Remarks, XCoordinate, YCoordinate, ZCoordinate, MetaData, Blob, Stratum_CN, CuttingUnit_CN, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate FROM Plot;
                                    DROP TABLE Plot;
                                    ALTER TABLE TempPlot RENAME TO Plot;";

                db.Execute(command);

                command = @"
                CREATE TRIGGER OnNewPlot AFTER INSERT ON Plot BEGIN 
			    UPDATE Plot SET CreatedDate = datetime(current_timestamp, 'localtime') WHERE rowID = new.rowID; END;

	            CREATE TRIGGER OnUpdatePlot UPDATE ON Plot BEGIN
			    UPDATE Plot SET ModifiedDate = datetime(current_timestamp, 'localtime') WHERE rowID = new.rowID; END;";
                db.Execute(command);

                db.AddField("CuttingUnit", "TallyHistory TEXT");

                SetDatabaseVersion(db, "2013.05.30");
                db.EndTransaction();
            }
            catch (Exception e)
            {
                db.CancelTransaction();
                throw new DatabaseExecutionException("failed updateing database to version 2013.05.30", e);
            }

        }

        private static void UpdateToVersion2013_08_02(DAL db)
        {
            try
            {
                db.BeginTransaction();

                db.AddField("Sale", "DefaultUOM TEXT");

                string command = "DROP TABLE LogFieldSetupDefault;";
                db.Execute(command);
                command = @"CREATE TABLE LogFieldSetupDefault (
				            LogFieldSetupDefault_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				            Field TEXT NOT NULL,
				            FieldName TEXT,
				            FieldOrder INTEGER Default 0,
				            ColumnType TEXT,
				            Heading TEXT,
				            Width REAL Default 0.0,
				            Format TEXT,
				            Behavior TEXT,
				            UNIQUE (Field));";
                db.Execute(command);

                SetDatabaseVersion(db, "2013.08.02");
                db.EndTransaction();
            }
            catch (Exception e)
            {
                db.CancelTransaction();
                throw new DatabaseExecutionException("failed updateing database to version 2013.08.02", e);
            }


        }

        private static void UpdateToVersion2013_08_29(DAL db)
        {

            try
            {
                db.BeginTransaction();
                db.AddField("Stratum", "KZ3PPNT INTEGER Default 0");

                SetDatabaseVersion(db, "2013.08.29");
                db.EndTransaction();
            }
            catch (Exception e)
            {
                db.CancelTransaction();
                throw new DatabaseExecutionException("failed updateing database to version 2013.08.29", e);
            }
        }

        private static void UpdateToVersion2013_10_29(DAL db)
        {
            try
            {
                db.BeginTransaction();

                db.AddField("Tree", "HiddenPrimary REAL Default 0.0");
                SetDatabaseVersion(db, "2013.10.29");
                db.EndTransaction();
            }
            catch (Exception e)
            {
                db.CancelTransaction();
                throw new DatabaseExecutionException("failed updateing database to version 2013.10.29", e);
            }


        }

        private static void UpdateToVersion2013_11_01(DAL db)
        {
            try
            {
                db.BeginTransaction();
                string command = @"    CREATE TABLE IF NOT EXISTS LogMatrix (
				ReportNumber TEXT,
				GradeDescription TEXT,
				LogSortDescription TEXT,
				Species TEXT,
				LogGrade1 TEXT,
				LogGrade2 TEXT,
				LogGrade3 TEXT,
				LogGrade4 TEXT,
				LogGrade5 TEXT,
				LogGrade6 TEXT,
				SEDlimit TEXT,
				SEDminimum DOUBLE Default 0.0,
				SEDmaximum DOUBLE Default 0.0);";

                db.Execute(command);
                SetDatabaseVersion(db, "2013.11.01");
                db.EndTransaction();
            }
            catch (Exception e)
            {
                db.CancelTransaction();
                throw new DatabaseExecutionException("failed updateing database to version 2013.11.01", e);
            }
        }


        private static void UpdateToVersion2013_06_12(DAL db)
        {
            try
            {
                db.BeginTransaction();

                db.AddField("ErrorLog", "Program TEXT");

                //modify globals table, making Block & Key Unique
                string command = @"CREATE TABLE TempGlobals (
				        Block TEXT,
				        Key TEXT,
				        Value TEXT,
				        UNIQUE (Block, Key));";

                db.Execute(command);

                command = "INSERT OR IGNORE INTO TempGlobals (Block, Key, Value) SELECT Block, Key, Value FROM Globals;";
                db.Execute(command);

                command = "DROP TABLE Globals;";
                db.Execute(command);

                command = "ALTER TABLE TempGlobals RENAME TO Globals;";
                db.Execute(command);

                command = "UPDATE CountTree SET TreeDefaultValue_CN = NULL WHERE TreeDefaultValue_CN = 0;";
                db.Execute(command);

                SetDatabaseVersion(db, "2013.06.12");

                db.EndTransaction();
            }
            catch (Exception e)
            {
                db.CancelTransaction();
                throw new DatabaseExecutionException("failed updateing database to version 2013.06.12", e);
            }

        }

        private static void UpdateToVersion2013_06_17(DAL db)
        {
            try
            {
                db.BeginTransaction();
                string command = @"
                                    PRAGMA foreign_keys = OFF;
                                    
                                    CREATE TABLE TempTree (
				                        Tree_CN INTEGER PRIMARY KEY AUTOINCREMENT,
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
				                        CreatedBy TEXT NOT NULL,
				                        CreatedDate DATETIME,
				                        ModifiedBy TEXT,
				                        ModifiedDate DATETIME,
				                        ExpansionFactor REAL Default 0.0,
				                        TreeFactor REAL Default 0.0,
				                        PointFactor REAL Default 0.0,
				                        UNIQUE (TreeDefaultValue_CN, Stratum_CN, SampleGroup_CN, CuttingUnit_CN, Plot_CN, TreeNumber));

                                        INSERT INTO TempTree (TreeNumber, Species, CountOrMeasure, TreeCount, KPI, STM, SeenDefectPrimary, SeenDefectSecondary, RecoverablePrimary, Initials, LiveDead, Grade, HeightToFirstLiveLimb, PoleLength, ClearFace, CrownRatio, DBH, DRC, TotalHeight, MerchHeightPrimary, MerchHeightSecondary, FormClass, UpperStemDOB, UpperStemHeight, DBHDoubleBarkThickness, TopDIBPrimary, TopDIBSecondary, DefectCode, DiameterAtDefect, VoidPercent, Remarks, XCoordinate, YCoordinate, ZCoordinate, MetaData, IsFallBuckScale, ExpansionFactor, TreeFactor, PointFactor, TreeDefaultValue_CN, Stratum_CN, SampleGroup_CN, CuttingUnit_CN, Plot_CN, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
                                        SELECT TreeNumber, Species, CountOrMeasure, TreeCount, KPI, STM, SeenDefectPrimary, SeenDefectSecondary, RecoverablePrimary, Initials, LiveDead, Grade, HeightToFirstLiveLimb, PoleLength, ClearFace, CrownRatio, DBH, DRC, TotalHeight, MerchHeightPrimary, MerchHeightSecondary, FormClass, UpperStemDOB, UpperStemHeight, DBHDoubleBarkThickness, TopDIBPrimary, TopDIBSecondary, DefectCode, DiameterAtDefect, VoidPercent, Remarks, XCoordinate, YCoordinate, ZCoordinate, MetaData, IsFallBuckScale, ExpansionFactor, TreeFactor, PointFactor, nullif(TreeDefaultValue_CN,0), Stratum_CN, nullif(SampleGroup_CN, 0), CuttingUnit_CN, Plot_CN, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate FROM Tree;
                                        DROP TABLE Tree;
                                        ALTER TABLE TempTree RENAME TO Tree;";
                db.Execute(command);

                //recreate triggers 
                command = @"
                CREATE TRIGGER IF NOT EXISTS OnNewTree AFTER INSERT ON Tree BEGIN 
			    UPDATE Tree SET CreatedDate = datetime(current_timestamp, 'localtime') WHERE rowID = new.rowID; END;

	            CREATE TRIGGER IF NOT EXISTS OnUpdateTree UPDATE ON Tree BEGIN
			    UPDATE Tree SET ModifiedDate = datetime(current_timestamp, 'localtime') WHERE rowID = new.rowID; END;";

                db.Execute(command);

                //command = "ALTER TABLE ErrorLog ADD COLUMN Suppress BOOLEAN;";
                //db.Execute(command);

                SetDatabaseVersion(db, "2013.06.17");
                db.EndTransaction();
            }
            catch (Exception e)
            {
                db.CancelTransaction();
                throw new DatabaseExecutionException("failed updateing database to version 2013.06.13", e);
            }

        }

        public static void UpdateToVersion2013_06_19(DAL db)
        {
            try
            {
                db.BeginTransaction();
                string command = "DROP TABLE ErrorLog;";
                db.Execute(command);

                command = @"
                    CREATE TABLE ErrorLog (
				    TableName TEXT NOT NULL,
				    CN_Number INTEGER NOT NULL,
				    ColumnName TEXT NOT NULL,
				    Level TEXT NOT NULL,
				    Message TEXT,
				    Program TEXT,
				    Suppress BOOLEAN Default 0,
				    UNIQUE (TableName, CN_Number, ColumnName, Level));";
                db.Execute(command);

                ////////////////////////////////////////////////////////////////////////Clean up 
                command = "DROP TABLE IF EXISTS TempGlobals;";
                db.Execute(command);
                ////////////////////////////////////////////////////////////////////////

                SetDatabaseVersion(db, "2013.06.19");
                db.EndTransaction();

            }
            catch (Exception e)
            {
                db.CancelTransaction();
                throw new DatabaseExecutionException("failed updateing database to version 2013.06.19", e);
            }

        }

        //fixes bug in database version 2013_06_19, doesn't alter schema
        private static void UpdateVersion2013_06_19(DAL db)
        {
            try
            {
                db.BeginTransaction();
                string command = "DELETE FROM ErrorLog WHERE Message LIKE 'Total Height, Merch Height Primary,%';";
                db.Execute(command);
                command = "DELETE FROM ErrorLog WHERE rowid IN (SELECT ErrorLog.rowid FROM ErrorLog JOIN Tree WHERE Tree.Tree_CN = ErrorLog.CN_Number AND ErrorLog.TableName = 'Tree');";


                db.Execute(command);
                command = "UPDATE TreeFieldSetup set ColumnType = 'Combo' WHERE Field = 'CountOrMeasure' OR Field = 'LiveDead';";
                db.Execute(command);
                db.EndTransaction();
            }
            catch (Exception e)
            {
                db.CancelTransaction();
                throw new DatabaseExecutionException("failed updateing database version 2013.06.19", e);
            }
        }

        public static void UpdateToVersion2013_11_22(DAL db)
        {
            try
            {
                db.BeginTransaction();



                string command = @"CREATE TABLE TempCountTree (
				CountTree_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				SampleGroup_CN INTEGER REFERENCES SampleGroup NOT NULL,
				CuttingUnit_CN INTEGER REFERENCES CuttingUnit NOT NULL,
				Tally_CN INTEGER REFERENCES Tally,
				TreeDefaultValue_CN INTEGER REFERENCES TreeDefaultValue,
				Component_CN INTEGER REFERENCES Component,
				TreeCount INTEGER Default 0,
				SumKPI INTEGER Default 0,
				SampleSelectorType TEXT,
				SampleSelectorState TEXT,
				CreatedBy TEXT NOT NULL,
				CreatedDate DATETIME,
				ModifiedBy TEXT,
				ModifiedDate DATETIME,
				UNIQUE (SampleGroup_CN, CuttingUnit_CN, TreeDefaultValue_CN, Component_CN));";

                db.Execute(command);

                command = @"INSERT INTO TempCountTree 
                    (CountTree_CN,
                    SampleGroup_CN,
                    CuttingUnit_CN,
                    Tally_CN,
                    TreeDefaultValue_CN,
                    TreeCount,
                    SumKPI,
                    SampleSelectorType,
                    SampleSelectorState,
                    CreatedBy,
                    CreatedDate,
                    ModifiedBy,
                    ModifiedDate)
                SELECT
                    CountTree_CN,
                    SampleGroup_CN,
                    CuttingUnit_CN,
                    Tally_CN,
                    TreeDefaultValue_CN,
                    TreeCount,
                    SumKPI,
                    SampleSelectorType,
                    SampleSelectorState,
                    CreatedBy,
                    CreatedDate,
                    ModifiedBy,
                    ModifiedDate FROM CountTree;";
                db.Execute(command);

                command = "DROP TABLE CountTree;";
                db.Execute(command);

                command = "ALTER TABLE TempCountTree RENAME TO CountTree;";
                db.Execute(command);

                command = @"
                CREATE TRIGGER OnNewCountTree AFTER INSERT ON CountTree BEGIN 
			    UPDATE CountTree SET CreatedDate = datetime(current_timestamp, 'localtime') WHERE rowID = new.rowID; END;

	            CREATE TRIGGER OnUpdateCountTree UPDATE ON CountTree BEGIN
			    UPDATE CountTree SET ModifiedDate = datetime(current_timestamp, 'localtime') WHERE rowID = new.rowID; END;";
                db.Execute(command);

                command = @"CREATE TABLE Component (
				Component_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				GUID TEXT,
				LastMerge DATETIME,
				FileName TEXT);";

                db.Execute(command);

                SetDatabaseVersion(db, "2013.11.22");
                db.EndTransaction();
            }
            catch (Exception e)
            {
                db.CancelTransaction();
                throw new DatabaseExecutionException("failed updateing database to version 2013.11.22", e);
            }

        }

        private static void UpdateToVersion2014_01_21(DAL db)
        {
            try
            {
                db.BeginTransaction();
                db.AddField("SampleGroup", "BigBAF INTEGER Default 0");

                SetDatabaseVersion(db, "2014.01.21");
                db.EndTransaction();
            }
            catch (Exception e)
            {
                db.CancelTransaction();
                throw new DatabaseExecutionException("failed updateing database to version 2014.01.21", e);
            }

        }

        private static void UpdateToVersion2014_03_12(DAL db)
        {
            try
            {
                db.BeginTransaction();
                db.AddField("SampleGroup", "SampleSelectorType TEXT");
                db.AddField("SampleGroup", "SampleSelectorState TEXT");

                RebuildTable(db, "CountTree",
                @"CREATE TABLE CountTree (
				CountTree_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				SampleGroup_CN INTEGER REFERENCES SampleGroup NOT NULL,
				CuttingUnit_CN INTEGER REFERENCES CuttingUnit NOT NULL,
				Tally_CN INTEGER REFERENCES Tally,
				TreeDefaultValue_CN INTEGER REFERENCES TreeDefaultValue,
				Component_CN INTEGER REFERENCES Component,
				TreeCount INTEGER Default 0,
				SumKPI INTEGER Default 0,
				CreatedBy TEXT NOT NULL,
				CreatedDate DATETIME,
				ModifiedBy TEXT,
				ModifiedDate DATETIME,
				UNIQUE (SampleGroup_CN, CuttingUnit_CN, TreeDefaultValue_CN, Component_CN));",
                "CountTree_CN, SampleGroup_CN, CuttingUnit_CN, Tally_CN, TreeDefaultValue_CN, Component_CN, TreeCount, SumKPI, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate");

                SetDatabaseVersion(db, "2014.03.12");

                db.EndTransaction();
            }
            catch (Exception e)
            {
                db.CancelTransaction();
                throw new DatabaseExecutionException("failed updating database to version 2014.03.12", e);
            }
        }

        private static void UpdateToVersion2014_06_04(DAL db)
        {
            try
            {
                db.BeginTransaction();
                db.AddField("Sale", "LogGradingEnabled BOOLEAN Default 0");

                SetDatabaseVersion(db, "2014.06.04");
                db.EndTransaction();
            }
            catch (Exception e)
            {
                db.CancelTransaction();
                throw new DatabaseExecutionException("failed updating database to version 2014.06.04", e);
            }
        }
        private static void UpdateToVersion2014_07_02(DAL db)
        {
            try
            {
                db.BeginTransaction();
                db.AddField("LogStock", "BoardUtil REAL Default 0.0");
                db.AddField("LogStock", "CubicUtil REAL Default 0.0");
                SetDatabaseVersion(db, "2014.07.02");
                db.EndTransaction();
            }
            catch (Exception e) 
            {
                db.CancelTransaction();
                throw new DatabaseExecutionException("failded updating database to version 2014.07.02", e);
            }
        }

        private static void UpdateToVersion2014_07_07(DAL db)
        {
            try
            {
                db.BeginTransaction();
                db.AddField("SampleGroup", "MinKPI INTEGER Default 0");
                db.AddField("SampleGroup", "MaxKPI INTEGER Default 0");
                SetDatabaseVersion(db, "2014.07.07");
                db.EndTransaction();
            }
            catch (Exception e)
            {
                db.CancelTransaction();
                throw new DatabaseExecutionException("failded updating database to version 2014.07.07", e);
            }
        }

        private static void UpdateToVersion2014_07_17(DAL db)
        {
            try
            {
                db.BeginTransaction();
                //rebuild Volume Equation table (remove MinLogLengthSecondary, MaxLogLengthSecondary; Add int MerchModFlag)
//                RebuildTable(db, "VolumeEquation", 
//                    @"
//CREATE TABLE VolumeEquation (
//				Species TEXT NOT NULL,
//				PrimaryProduct TEXT NOT NULL,
//				VolumeEquationNumber TEXT NOT NULL,
//				StumpHeight REAL Default 0.0,
//				TopDIBPrimary REAL Default 0.0,
//				TopDIBSecondary REAL Default 0.0,
//				CalcTotal INTEGER Default 0,
//				CalcBoard INTEGER Default 0,
//				CalcCubic INTEGER Default 0,
//				CalcCord INTEGER Default 0,
//				CalcTopwood INTEGER Default 0,
//				CalcBiomass INTEGER Default 0,
//				Trim REAL Default 0.0,
//				SegmentationLogic INTEGER Default 0,
//				MinLogLengthPrimary REAL Default 0.0,
//				MaxLogLengthPrimary REAL Default 0.0,
//				MinMerchLength REAL Default 0.0,
//				Model TEXT,
//				CommonSpeciesName TEXT,
//				MerchModFlag INTEGER Default 0,
//				UNIQUE (Species, PrimaryProduct, VolumeEquationNumber));
//",
// @"
//                Species,
//                PrimaryProduct,
//                VolumeEquationNumber,
//                StumpHeight,
//                TopDIBPrimary,
//                TopDIBSecondary,
//                CalcTotal,
//                CalcBoard,
//                CalcCubic,
//                CalcCord,
//                CalcTopWood,
//                CalcBiomass,
//                Trim,
//                SegmentationLogic,
//                MinLogLengthPrimary,
//                MaxLogLengthPrimary,
//                MinMerchLength,
//                Model,
//                CommonSpeciesName");
                db.AddField("VolumeEquation", "MerchModFlag INTEGER Default 0");

                //rebuild TreeAuditValue Table ( remove error message field)
//                RebuildTable(db, "TreeAuditValue",
//                    @"
//CREATE TABLE TreeAuditValue (
//				TreeAuditValue_CN INTEGER PRIMARY KEY AUTOINCREMENT,
//				Field TEXT NOT NULL,
//				Min REAL Default 0.0,
//				Max REAL Default 0.0,
//				ValueSet TEXT,
//				Required BOOLEAN Default 0);
//",
// @"
//Field, 
//Min, 
//Max, 
//ValueSet,
//Required");
                //Add ReconPlots
                db.AddField("SampleGroupStats", "ReconPlots INTEGER Default 0");
                //Add ReconTrees
                db.AddField("SampleGroupStats", "ReconTrees INTEGER Default 0");
                SetDatabaseVersion(db, "2014.07.17");
                db.EndTransaction();

            }
            catch (Exception e)
            {
                db.CancelTransaction();
                throw new DatabaseExecutionException("failed updating database to version 2014.07.17", e);
            }

        }

        private static void UpdateToVersion2014_07_24(DAL db)
        {
            try
            {
                db.BeginTransaction();
                db.Execute(@"
DROP TRIGGER IF EXISTS OnDeleteTree;
CREATE TRIGGER OnDeleteTree AFTER DELETE ON Tree BEGIN 
			INSERT INTO MessageLog (Message, Date, Time) VALUES (('Tree (' || OLD.Tree_CN || ') Deleted CU_cn:' || OLD.CuttingUnit_CN || ' St_cn:' || OLD.Stratum_CN || ' Plt_CN:' || ifnull(OLD.Plot_CN,'-') || ' T#:' || OLD.TreeNumber), date('now'), time('now')); END;
DROP TRIGGER IF EXISTS OnDeletePlot;
CREATE TRIGGER OnDeletePlot AFTER DELETE ON Plot BEGIN 
			INSERT INTO MessageLog (Message, Date, Time) VALUES (('Plot (' || OLD.Plot_CN || ') Deleted CU_cn:' || OLD.CuttingUnit_CN  || ' St_cn:' || OLD.Stratum_CN || ' Plt#:' || OLD.PlotNumber), date('now'), time('now')); END;
");
                SetDatabaseVersion(db, "2014.07.24");
                db.EndTransaction();
            }
            catch(Exception e)
            {
                db.CancelTransaction();
                throw new DatabaseExecutionException("failed updating database to version 2014.07.24", e);
            }
        }

        private static void UpdateToVersion2014_08_20(DAL db)
        {
            try
            {
                db.BeginTransaction();
                db.AddField(Schema.VOLUMEEQUATION._NAME, "EvenOddSegment INTEGER Default 0");
                SetDatabaseVersion(db, "2014.08.20");
                db.EndTransaction();
            }
            catch (Exception e)
            {
                db.CancelTransaction();
                throw new DatabaseExecutionException("failed updating database to version 2014.08.20", e);
            }
        }

        private static void UpdateToVersion2014_09_02(DAL db)
        {
            try
            {
                db.BeginTransaction();
                db.AddField(Schema.SAMPLEGROUP._NAME, "TallyMethod TEXT");
                SetDatabaseVersion(db, "2014.09.02");
                db.EndTransaction();
            }
            catch (Exception e)
            {
                db.CancelTransaction();
                throw new DatabaseExecutionException("failed updating database to version 2014.09.02", e);
            }
        }

        private static void UpdateToVersion2014_10_01(DAL db)
        {
            try
            {
                db.BeginTransaction();
                db.Execute("DROP TABLE Regression;");
                db.Execute(@"CREATE TABLE Regression (
				Regression_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				rVolume TEXT,
				rVolType TEXT,
				rSpeices TEXT,
				rProduct TEXT,
				rLiveDead TEXT,
				CoefficientA REAL Default 0.0,
				CoefficientB REAL Default 0.0,
				CoefficientC REAL Default 0.0,
				TotalTrees INTEGER Default 0,
				MeanSE REAL Default 0.0,
				Rsquared REAL Default 0.0,
				RegressModel TEXT,
				rMinDbh REAL Default 0.0,
				rMaxDbh REAL Default 0.0);");
                SetDatabaseVersion(db, "2014.10.01");
                db.EndTransaction();
            }
            catch(Exception e)
            {
                db.CancelTransaction();
                throw new DatabaseExecutionException("failed updating database to version 2014.10.01", e);
            }

        }

    }
}
