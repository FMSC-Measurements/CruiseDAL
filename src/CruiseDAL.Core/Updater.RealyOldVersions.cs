using Backpack.SqlBuilder;
using FMSC.ORM;
using System;

namespace CruiseDAL
{
    public static partial class Updater
    {
        private static void UpdateToVersion2014_06_04(DAL db)
        {
            var startVersion = db.DatabaseVersion;

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
                throw new SchemaUpdateException(startVersion, "2014.06.04", e);
            }
        }

        private static void UpdateToVersion2014_07_02(DAL db)
        {
            var startVersion = db.DatabaseVersion;

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
                throw new SchemaUpdateException(startVersion, "2014.07.02", e);
            }
        }

        private static void UpdateToVersion2014_07_07(DAL db)
        {
            var startVersion = db.DatabaseVersion;

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
                throw new SchemaUpdateException(startVersion, "2014.07.07", e);
            }
        }

        private static void UpdateToVersion2014_07_17(DAL db)
        {
            var startVersion = db.DatabaseVersion;

            try
            {
                db.BeginTransaction();

                db.AddField("VolumeEquation", new ColumnInfo("MerchModFlag", "INTEGER") { Default = "0" });

                db.AddField("SampleGroupStats", new ColumnInfo("ReconPlots", "INTEGER") { Default = "0" });
                db.AddField("SampleGroupStats", new ColumnInfo("ReconTrees", "INTEGER") { Default = "0" });

                SetDatabaseVersion(db, "2014.07.17");
                db.CommitTransaction();
            }
            catch (Exception e)
            {
                db.RollbackTransaction();
                throw new SchemaUpdateException(startVersion, "2014.07.17", e);
            }
        }

        private static void UpdateToVersion2014_07_24(DAL db)
        {
            var startVersion = db.DatabaseVersion;

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
                throw new SchemaUpdateException(startVersion, "2014.07.24", e);
            }
        }

        private static void UpdateToVersion2014_08_20(DAL db)
        {
            var startVersion = db.DatabaseVersion;

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
                throw new SchemaUpdateException(startVersion, "2014.08.20", e);
            }
        }

        private static void UpdateToVersion2014_09_02(DAL db)
        {
            var startVersion = db.DatabaseVersion;

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

                throw new SchemaUpdateException(startVersion, "2014.09.02", e);
            }
        }

        private static void UpdateToVersion2014_10_01(DAL db)
        {
            var startVersion = db.DatabaseVersion;

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
                throw new SchemaUpdateException(startVersion, "2014.10.01", e);
            }
        }
    }
}