using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.V3
{
    public partial class DDL
    {
        public const string CREATE_TABLE_SAMPLEGROUP_V3 =
            "CREATE TABLE SampleGroup_V3 (" +
                "SampleGroup_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "SampleGroupCode TEXT NOT NULL COLLATE NOCASE, " +
                "StratumCode TEXT NOT NULL COLLATE NOCASE, " +
                "CutLeave TEXT NOT NULL COLLATE NOCASE, " +
                "UOM TEXT NOT NULL COLLATE NOCASE, " +
                "PrimaryProduct TEXT NOT NULL COLLATE NOCASE, " +
                "SecondaryProduct TEXT COLLATE NOCASE, " +
                "BiomassProduct TEXT, " +
                "DefaultLiveDead TEXT COLLATE NOCASE, " +
                "SamplingFrequency INTEGER Default 0, " +
                "InsuranceFrequency INTEGER Default 0, " +
                "KZ INTEGER Default 0, " +
                "BigBAF INTEGER Default 0, " +
                "TallyMethod TEXT COLLATE NOCASE, " +
                "Description TEXT, " +
                "SampleSelectorType TEXT COLLATE NOCASE, " +
                "MinKPI INTEGER Default 0, " +
                "MaxKPI INTEGER Default 0, " +
                "SmallFPS REAL DEFAULT 0.0," +
                "CreatedBy TEXT, " +
                "CreatedDate DATETIME DEFAULT (datetime('now', 'localtime')), " +
                "ModifiedBy TEXT, " +
                "ModifiedDate DATETIME, " +
                "RowVersion INTEGER DEFAULT 0, " +
                "UNIQUE (StratumCode, SampleGroupCode), " +
                "FOREIGN KEY (StratumCode) REFERENCES Stratum (Code) ON DELETE CASCADE ON UPDATE CASCADE" +
            ");";

        public const string CREATE_TRIGGER_SAMPLEGROUP_V3_ONUPDATE =
            "CREATE TRIGGER SampleGroup_V3_OnUpdate " +
            "AFTER UPDATE OF " +
                "SampleGroupCode, " +
                "StratumCode, " +
                "CutLeave, " +
                "UOM, " +
                "PrimaryProduct, " +
                "SecondaryProduct, " +
                "BiomassProduct, " +
                "DefaultLiveDead, " +
                "SamplingFrequency, " +
                "InsuranceFrequency, " +
                "KZ, " +
                "BigBAF, " +
                "TallyMethod, " +
                "Description, " +
                "SampleSelectorType, " +
                "MinKPI, " +
                "MaxKPI, " +
                "SmallFPS " +
            "ON SampleGroup_V3 " +
            "FOR EACH ROW " +
            "BEGIN " +
                "UPDATE SampleGroup_V3 SET ModifiedDate = datetime('now', 'localtime') WHERE SampleGroup_CN = old.SampleGroup_CN;" +
                "UPDATE SampleGroup_V3 SET RowVersion = old.RowVersion WHERE SampleGroup_CN = old.SampleGroup_CN; " +
            "END;";
    }

    public partial class Updater
    {
        public const string INITIALIZE_SAMPLEGROUP_V3_FROM_SAMPLEGROUP =
            "INSERT INTO SampleGroup_V3 " +
            "SELECT " +
            "sg.Code AS SampleGroupCode, " +
            "st.Code AS StratumCode, " +
            "sg.CutLeave, " +
            "sg.UOM, " +
            "sg.PrimaryProduct, " +
            "sg.SecondaryProduct, " +
            "sg.BiomassProduct, " +
            "sg.DefaultLiveDead, " +
            "sg.SamplingFrequency, " +
            "sg.InsuranceFrequency, " +
            "sg.KZ, " +
            "sg.BigBAF, " +
            "sg.TallyMethod, " +
            "sg.Description, " +
            "sg.SampleSelectorType, " +
            "sg.MinKPI, " +
            "sg.MaxKPI, " +
            "sg.SmallFPS, " +
            "sg.CreatedBy, " +
            "sg.CreatedDate, " +
            "sg.ModifiedBy, " +
            "sg.ModifiedDate, " +
            "RowVersion " +
            "FROM SampleGroup AS sg " +
            "JOIN Stratum AS st USING (Stratum_CN);";
    }
}
