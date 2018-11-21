using Backpack.SqlBuilder;
using FMSC.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL
{
    public static partial class Updater
    {
    //    private static void UpdateToVersion2013_05_30(DAL db)
    //{
    //    try
    //    {
    //        db.BeginTransaction();

    //        RebuildTable(db, "Plot",
    //            @"CREATE TABLE Plot (
    //                    Plot_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    //                    Stratum_CN INTEGER REFERENCES Stratum NOT NULL,
    //                    CuttingUnit_CN INTEGER REFERENCES CuttingUnit NOT NULL,
    //                    PlotNumber INTEGER NOT NULL,
    //                    IsEmpty TEXT,
    //                    Slope REAL Default 0.0,
    //                    KPI REAL Default 0.0,
    //                    Aspect REAL Default 0.0,
    //                    Remarks TEXT,
    //                    XCoordinate REAL Default 0.0,
    //                    YCoordinate REAL Default 0.0,
    //                    ZCoordinate REAL Default 0.0,
    //                    MetaData TEXT,
    //                    Blob BLOB,
    //                    CreatedBy TEXT NOT NULL,
    //                    CreatedDate DATETIME,
    //                    ModifiedBy TEXT,
    //                    ModifiedDate DATETIME,
    //                    UNIQUE (Stratum_CN, CuttingUnit_CN, PlotNumber));",
    //            "PlotNumber, IsEmpty, Slope, KPI, Aspect, Remarks, XCoordinate, YCoordinate," +
    //            "ZCoordinate, MetaData, Blob, Stratum_CN, CuttingUnit_CN, CreatedBy, " +
    //            "CreatedDate, ModifiedBy, ModifiedDate");

    //        db.AddField("CuttingUnit", new ColumnInfo("TallyHistory", "TEXT"));

    //        SetDatabaseVersion(db, "2013.05.30");
    //        db.CommitTransaction();
    //    }
    //    catch (Exception e)
    //    {
    //        db.RollbackTransaction();
    //        throw new SchemaUpdateException(db.DatabaseVersion, "2013.05.30", e);
    //    }
    //}

    //private static void UpdateToVersion2013_08_02(DAL db)
    //{
    //    try
    //    {
    //        db.BeginTransaction();

    //        db.AddField("Sale", new ColumnInfo("DefaultUOM", "TEXT"));

    //        string command = "DROP TABLE LogFieldSetupDefault;";
    //        db.Execute(command);
    //        command = @"CREATE TABLE LogFieldSetupDefault (
				//            LogFieldSetupDefault_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				//            Field TEXT NOT NULL,
				//            FieldName TEXT,
				//            FieldOrder INTEGER Default 0,
				//            ColumnType TEXT,
				//            Heading TEXT,
				//            Width REAL Default 0.0,
				//            Format TEXT,
				//            Behavior TEXT,
				//            UNIQUE (Field));";
    //        db.Execute(command);

    //        SetDatabaseVersion(db, "2013.08.02");
    //        db.CommitTransaction();
    //    }
    //    catch (Exception e)
    //    {
    //        db.RollbackTransaction();
    //        throw new SchemaUpdateException(db.DatabaseVersion, "2013.08.02", e);
    //    }
    //}

    //private static void UpdateToVersion2013_08_29(DAL db)
    //{
    //    try
    //    {
    //        db.BeginTransaction();
    //        db.AddField("Stratum", new ColumnInfo("KZ3PPNT", "INTEGER") { Default = "0" });

    //        SetDatabaseVersion(db, "2013.08.29");
    //        db.CommitTransaction();
    //    }
    //    catch (Exception e)
    //    {
    //        db.RollbackTransaction();
    //        throw new SchemaUpdateException(db.DatabaseVersion, "2013.08.29", e);
    //    }
    //}

    //private static void UpdateToVersion2013_10_29(DAL db)
    //{
    //    try
    //    {
    //        db.BeginTransaction();

    //        db.AddField("Tree", new ColumnInfo("HiddenPrimary", "REAL") { Default = "0.0" });
    //        SetDatabaseVersion(db, "2013.10.29");
    //        db.CommitTransaction();
    //    }
    //    catch (Exception e)
    //    {
    //        db.RollbackTransaction();
    //        throw new SchemaUpdateException(db.DatabaseVersion, "2013.10.29", e);
    //    }
    //}

    //private static void UpdateToVersion2013_11_01(DAL db)
    //{
    //    try
    //    {
    //        db.BeginTransaction();
    //        string command = @"    CREATE TABLE IF NOT EXISTS LogMatrix (
				//ReportNumber TEXT,
				//GradeDescription TEXT,
				//LogSortDescription TEXT,
				//Species TEXT,
				//LogGrade1 TEXT,
				//LogGrade2 TEXT,
				//LogGrade3 TEXT,
				//LogGrade4 TEXT,
				//LogGrade5 TEXT,
				//LogGrade6 TEXT,
				//SEDlimit TEXT,
				//SEDminimum DOUBLE Default 0.0,
				//SEDmaximum DOUBLE Default 0.0);";

    //        db.Execute(command);
    //        SetDatabaseVersion(db, "2013.11.01");
    //        db.CommitTransaction();
    //    }
    //    catch (Exception e)
    //    {
    //        db.RollbackTransaction();
    //        throw new SchemaUpdateException(db.DatabaseVersion, "2013.11.01", e);
    //    }
    //}

    //private static void UpdateToVersion2013_06_12(DAL db)
    //{
    //    try
    //    {
    //        db.BeginTransaction();

    //        db.AddField("ErrorLog", new ColumnInfo("Program", "TEXT"));

    //        //modify globals table, making Block & Key Unique
    //        string command = @"CREATE TABLE TempGlobals (
				//        Block TEXT,
				//        Key TEXT,
				//        Value TEXT,
				//        UNIQUE (Block, Key));";

    //        db.Execute(command);

    //        command = "INSERT OR IGNORE INTO TempGlobals (Block, Key, Value) SELECT Block, Key, Value FROM Globals;";
    //        db.Execute(command);

    //        command = "DROP TABLE Globals;";
    //        db.Execute(command);

    //        command = "ALTER TABLE TempGlobals RENAME TO Globals;";
    //        db.Execute(command);

    //        command = "UPDATE CountTree SET TreeDefaultValue_CN = NULL WHERE TreeDefaultValue_CN = 0;";
    //        db.Execute(command);

    //        SetDatabaseVersion(db, "2013.06.12");

    //        db.CommitTransaction();
    //    }
    //    catch (Exception e)
    //    {
    //        db.RollbackTransaction();
    //        throw new SchemaUpdateException(db.DatabaseVersion, "2013.06.12", e);
    //    }
    //}

    //private static void UpdateToVersion2013_06_17(DAL db)
    //{
    //    try
    //    {
    //        db.BeginTransaction();
    //        RebuildTable(db, "Tree",
    //            @"CREATE TABLE Tree (
    //                    Tree_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    //                    TreeDefaultValue_CN INTEGER REFERENCES TreeDefaultValue,
    //                    Stratum_CN INTEGER REFERENCES Stratum NOT NULL,
    //                    SampleGroup_CN INTEGER REFERENCES SampleGroup,
    //                    CuttingUnit_CN INTEGER REFERENCES CuttingUnit NOT NULL,
    //                    Plot_CN INTEGER REFERENCES Plot,
    //                    TreeNumber INTEGER NOT NULL,
    //                    Species TEXT,
    //                    CountOrMeasure TEXT,
    //                    TreeCount REAL Default 0.0,
    //                    KPI REAL Default 0.0,
    //                    STM TEXT Default 'N',
    //                    SeenDefectPrimary REAL Default 0.0,
    //                    SeenDefectSecondary REAL Default 0.0,
    //                    RecoverablePrimary REAL Default 0.0,
    //                    Initials TEXT,
    //                    LiveDead TEXT,
    //                    Grade TEXT,
    //                    HeightToFirstLiveLimb REAL Default 0.0,
    //                    PoleLength REAL Default 0.0,
    //                    ClearFace TEXT,
    //                    CrownRatio REAL Default 0.0,
    //                    DBH REAL Default 0.0,
    //                    DRC REAL Default 0.0,
    //                    TotalHeight REAL Default 0.0,
    //                    MerchHeightPrimary REAL Default 0.0,
    //                    MerchHeightSecondary REAL Default 0.0,
    //                    FormClass REAL Default 0.0,
    //                    UpperStemDOB REAL Default 0.0,
    //                    UpperStemHeight REAL Default 0.0,
    //                    DBHDoubleBarkThickness REAL Default 0.0,
    //                    TopDIBPrimary REAL Default 0.0,
    //                    TopDIBSecondary REAL Default 0.0,
    //                    DefectCode TEXT,
    //                    DiameterAtDefect REAL Default 0.0,
    //                    VoidPercent REAL Default 0.0,
    //                    Slope REAL Default 0.0,
    //                    Aspect REAL Default 0.0,
    //                    Remarks TEXT,
    //                    XCoordinate DOUBLE Default 0.0,
    //                    YCoordinate DOUBLE Default 0.0,
    //                    ZCoordinate DOUBLE Default 0.0,
    //                    MetaData TEXT,
    //                    IsFallBuckScale INTEGER Default 0,
    //                    CreatedBy TEXT NOT NULL,
    //                    CreatedDate DATETIME,
    //                    ModifiedBy TEXT,
    //                    ModifiedDate DATETIME,
    //                    ExpansionFactor REAL Default 0.0,
    //                    TreeFactor REAL Default 0.0,
    //                    PointFactor REAL Default 0.0,
    //                    UNIQUE (TreeDefaultValue_CN, Stratum_CN, SampleGroup_CN, CuttingUnit_CN, Plot_CN, TreeNumber))",
    //                "TreeNumber, Species, CountOrMeasure, TreeCount, KPI, STM, SeenDefectPrimary, " +
    //                "SeenDefectSecondary, RecoverablePrimary, Initials, LiveDead, Grade, " +
    //                "HeightToFirstLiveLimb, PoleLength, ClearFace, CrownRatio, DBH, DRC, TotalHeight, " +
    //                "MerchHeightPrimary, MerchHeightSecondary, FormClass, UpperStemDOB, UpperStemHeight, " +
    //                "DBHDoubleBarkThickness, TopDIBPrimary, TopDIBSecondary, DefectCode, DiameterAtDefect, " +
    //                "VoidPercent, Remarks, XCoordinate, YCoordinate, ZCoordinate, MetaData, " +
    //                "IsFallBuckScale, ExpansionFactor, TreeFactor, PointFactor, TreeDefaultValue_CN, " +
    //                "Stratum_CN, SampleGroup_CN, CuttingUnit_CN, Plot_CN, CreatedBy, CreatedDate, " +
    //                "ModifiedBy, ModifiedDate");

    //        //command = "ALTER TABLE ErrorLog ADD COLUMN Suppress BOOLEAN;";
    //        //db.Execute(command);

    //        SetDatabaseVersion(db, "2013.06.17");
    //        db.CommitTransaction();
    //    }
    //    catch (Exception e)
    //    {
    //        db.RollbackTransaction();
    //        throw new SchemaUpdateException(db.DatabaseVersion, "2013.06.17", e);
    //    }
    //}

    //public static void UpdateToVersion2013_06_19(DAL db)
    //{
    //    try
    //    {
    //        db.BeginTransaction();
    //        string command = "DROP TABLE ErrorLog;";
    //        db.Execute(command);

    //        command = @"
    //                CREATE TABLE ErrorLog (
				//    TableName TEXT NOT NULL,
				//    CN_Number INTEGER NOT NULL,
				//    ColumnName TEXT NOT NULL,
				//    Level TEXT NOT NULL,
				//    Message TEXT,
				//    Program TEXT,
				//    Suppress BOOLEAN Default 0,
				//    UNIQUE (TableName, CN_Number, ColumnName, Level));";
    //        db.Execute(command);

    //        ////////////////////////////////////////////////////////////////////////Clean up
    //        command = "DROP TABLE IF EXISTS TempGlobals;";
    //        db.Execute(command);
    //        ////////////////////////////////////////////////////////////////////////

    //        SetDatabaseVersion(db, "2013.06.19");
    //        db.CommitTransaction();
    //    }
    //    catch (Exception e)
    //    {
    //        db.RollbackTransaction();
    //        throw new SchemaUpdateException(db.DatabaseVersion, "2013.06.19", e);
    //    }
    //}

    ////fixes bug in database version 2013_06_19, doesn't alter schema
    //private static void UpdateVersion2013_06_19(DAL db)
    //{
    //    try
    //    {
    //        db.BeginTransaction();
    //        string command = "DELETE FROM ErrorLog WHERE Message LIKE 'Total Height, Merch Height Primary,%';";
    //        db.Execute(command);
    //        command = "DELETE FROM ErrorLog WHERE rowid IN (SELECT ErrorLog.rowid FROM ErrorLog JOIN Tree WHERE Tree.Tree_CN = ErrorLog.CN_Number AND ErrorLog.TableName = 'Tree');";

    //        db.Execute(command);
    //        command = "UPDATE TreeFieldSetup set ColumnType = 'Combo' WHERE Field = 'CountOrMeasure' OR Field = 'LiveDead';";
    //        db.Execute(command);
    //        db.CommitTransaction();
    //    }
    //    catch (Exception e)
    //    {
    //        db.RollbackTransaction();
    //        throw new SchemaUpdateException(db.DatabaseVersion, "2013.06.19", e);
    //    }
    //}

    //public static void UpdateToVersion2013_11_22(DAL db)
    //{
    //    try
    //    {
    //        db.BeginTransaction();

    //        var command = @"CREATE TABLE Component (
				//Component_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				//GUID TEXT,
				//LastMerge DATETIME,
				//FileName TEXT);";

    //        db.Execute(command);

    //        command = @"CREATE TABLE TempCountTree (
				//CountTree_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				//SampleGroup_CN INTEGER REFERENCES SampleGroup NOT NULL,
				//CuttingUnit_CN INTEGER REFERENCES CuttingUnit NOT NULL,
				//Tally_CN INTEGER REFERENCES Tally,
				//TreeDefaultValue_CN INTEGER REFERENCES TreeDefaultValue,
				//Component_CN INTEGER REFERENCES Component,
				//TreeCount INTEGER Default 0,
				//SumKPI INTEGER Default 0,
				//SampleSelectorType TEXT,
				//SampleSelectorState TEXT,
				//CreatedBy TEXT NOT NULL,
				//CreatedDate DATETIME,
				//ModifiedBy TEXT,
				//ModifiedDate DATETIME,
				//UNIQUE (SampleGroup_CN, CuttingUnit_CN, TreeDefaultValue_CN, Component_CN));";

    //        db.Execute(command);

    //        command = @"INSERT INTO TempCountTree
    //                (CountTree_CN,
    //                SampleGroup_CN,
    //                CuttingUnit_CN,
    //                Tally_CN,
    //                TreeDefaultValue_CN,
    //                TreeCount,
    //                SumKPI,
    //                SampleSelectorType,
    //                SampleSelectorState,
    //                CreatedBy,
    //                CreatedDate,
    //                ModifiedBy,
    //                ModifiedDate)
    //            SELECT
    //                CountTree_CN,
    //                SampleGroup_CN,
    //                CuttingUnit_CN,
    //                Tally_CN,
    //                TreeDefaultValue_CN,
    //                TreeCount,
    //                SumKPI,
    //                SampleSelectorType,
    //                SampleSelectorState,
    //                CreatedBy,
    //                CreatedDate,
    //                ModifiedBy,
    //                ModifiedDate FROM CountTree;";
    //        db.Execute(command);

    //        command = "DROP TABLE CountTree;";
    //        db.Execute(command);

    //        command = "ALTER TABLE TempCountTree RENAME TO CountTree;";
    //        db.Execute(command);

    //        command = @"
    //            CREATE TRIGGER OnNewCountTree AFTER INSERT ON CountTree BEGIN
			 //   UPDATE CountTree SET CreatedDate = datetime(current_timestamp, 'localtime') WHERE rowID = new.rowID; END;

	   //         CREATE TRIGGER OnUpdateCountTree UPDATE ON CountTree BEGIN
			 //   UPDATE CountTree SET ModifiedDate = datetime(current_timestamp, 'localtime') WHERE rowID = new.rowID; END;";
    //        db.Execute(command);

    //        SetDatabaseVersion(db, "2013.11.22");
    //        db.CommitTransaction();
    //    }
    //    catch (Exception e)
    //    {
    //        db.RollbackTransaction();
    //        throw new SchemaUpdateException(db.DatabaseVersion, "2013.11.22", e);
    //    }
    //}

    //private static void UpdateToVersion2014_01_21(DAL db)
    //{
    //    try
    //    {
    //        db.BeginTransaction();
    //        db.AddField("SampleGroup", new ColumnInfo("BigBAF", "INTEGER") { Default = "0" });

    //        SetDatabaseVersion(db, "2014.01.21");
    //        db.CommitTransaction();
    //    }
    //    catch (Exception e)
    //    {
    //        db.RollbackTransaction();
    //        throw new SchemaUpdateException(db.DatabaseVersion, "2014.01.21", e);
    //    }
    //}

    //private static void UpdateToVersion2014_03_12(DAL db)
    //{
    //    try
    //    {
    //        db.BeginTransaction();
    //        db.AddField("SampleGroup", new ColumnInfo("SampleSelectorType", "TEXT"));
    //        db.AddField("SampleGroup", new ColumnInfo("SampleSelectorState", "TEXT"));

    //        RebuildTable(db, "CountTree",
    //        @"CREATE TABLE CountTree (
				//CountTree_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				//SampleGroup_CN INTEGER REFERENCES SampleGroup NOT NULL,
				//CuttingUnit_CN INTEGER REFERENCES CuttingUnit NOT NULL,
				//Tally_CN INTEGER REFERENCES Tally,
				//TreeDefaultValue_CN INTEGER REFERENCES TreeDefaultValue,
				//Component_CN INTEGER REFERENCES Component,
				//TreeCount INTEGER Default 0,
				//SumKPI INTEGER Default 0,
				//CreatedBy TEXT NOT NULL,
				//CreatedDate DATETIME,
				//ModifiedBy TEXT,
				//ModifiedDate DATETIME,
				//UNIQUE (SampleGroup_CN, CuttingUnit_CN, TreeDefaultValue_CN, Component_CN));",
    //        "CountTree_CN, SampleGroup_CN, CuttingUnit_CN, Tally_CN, TreeDefaultValue_CN, Component_CN, TreeCount, SumKPI, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate");

    //        SetDatabaseVersion(db, "2014.03.12");

    //        db.CommitTransaction();
    //    }
    //    catch (Exception e)
    //    {
    //        db.RollbackTransaction();
    //        throw new SchemaUpdateException(db.DatabaseVersion, "2014.03.12", e);
    //    }
    //}

    private static void UpdateToVersion2014_06_04(DAL db)
    {
        try
        {
            db.BeginTransaction();
            db.AddField("Sale", new ColumnInfo("LogGradingEnabled", "BOOLEAN") { Default = "0" });

            SetDatabaseVersion(db, "2014.06.04");
            db.CommitTransaction();
        }
        catch (Exception e)
        {
            db.RollbackTransaction();
            throw new SchemaUpdateException(db.DatabaseVersion, "2014.06.04", e);
        }
    }

    private static void UpdateToVersion2014_07_02(DAL db)
    {
        try
        {
            db.BeginTransaction();
            db.AddField("LogStock", new ColumnInfo("BoardUtil", "REAL") { Default = "0.0" });
            db.AddField("LogStock", new ColumnInfo("CubicUtil", "REAL") { Default = "0.0" });
            SetDatabaseVersion(db, "2014.07.02");
            db.CommitTransaction();
        }
        catch (Exception e)
        {
            db.RollbackTransaction();
            throw new SchemaUpdateException(db.DatabaseVersion, "2014.07.02", e);
        }
    }

    private static void UpdateToVersion2014_07_07(DAL db)
    {
        try
        {
            db.BeginTransaction();
            db.AddField("SampleGroup", new ColumnInfo("MinKPI", "INTEGER") { Default = "0" });
            db.AddField("SampleGroup", new ColumnInfo("MaxKPI", "INTEGER") { Default = "0" });
            SetDatabaseVersion(db, "2014.07.07");
            db.CommitTransaction();
        }
        catch (Exception e)
        {
            db.RollbackTransaction();
            throw new SchemaUpdateException(db.DatabaseVersion, "2014.07.07", e);
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
            db.AddField("VolumeEquation", new ColumnInfo("MerchModFlag", "INTEGER") { Default = "0" });

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

            db.AddField("SampleGroupStats", new ColumnInfo("ReconPlots", "INTEGER") { Default = "0" });
            db.AddField("SampleGroupStats", new ColumnInfo("ReconTrees", "INTEGER") { Default = "0" });

            SetDatabaseVersion(db, "2014.07.17");
            db.CommitTransaction();
        }
        catch (Exception e)
        {
            db.RollbackTransaction();
            throw new SchemaUpdateException(db.DatabaseVersion, "2014.07.17", e);
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
            db.CommitTransaction();
        }
        catch (Exception e)
        {
            db.RollbackTransaction();
            throw new SchemaUpdateException(db.DatabaseVersion, "2014.07.24", e);
        }
    }

    private static void UpdateToVersion2014_08_20(DAL db)
    {
        try
        {
            db.BeginTransaction();
            db.AddField(Schema.VOLUMEEQUATION._NAME, new ColumnInfo("EvenOddSegment", "INTEGER") { Default = "0" });
            SetDatabaseVersion(db, "2014.08.20");
            db.CommitTransaction();
        }
        catch (Exception e)
        {
            db.RollbackTransaction();
            throw new SchemaUpdateException(db.DatabaseVersion, "2014.08.20", e);
        }
    }

    private static void UpdateToVersion2014_09_02(DAL db)
    {
        try
        {
            db.BeginTransaction();
            db.AddField(Schema.SAMPLEGROUP._NAME, new ColumnInfo("TallyMethod", "TEXT"));
            SetDatabaseVersion(db, "2014.09.02");
            db.CommitTransaction();
        }
        catch (Exception e)
        {
            db.RollbackTransaction();

            throw new SchemaUpdateException(db.DatabaseVersion, "2014.09.02", e);
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
            db.CommitTransaction();
        }
        catch (Exception e)
        {
            db.RollbackTransaction();
            throw new SchemaUpdateException(db.DatabaseVersion, "2014.10.01", e);
        }
    }
}
}
