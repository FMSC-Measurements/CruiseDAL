namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_TREECALCULATEDVALUES =
            "CREATE TABLE TreeCalculatedValues(" +
                "TreeCalcValues_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "Tree_CN INTEGER NOT NULL, " +
                "TotalCubicVolume REAL Default 0.0, " +
                "GrossBDFTPP REAL Default 0.0, " +
                "NetBDFTPP REAL Default 0.0, " +
                "GrossCUFTPP REAL Default 0.0, " +
                "NetCUFTPP REAL Default 0.0, " +
                "CordsPP REAL Default 0.0, " +
                "GrossBDFTRemvPP REAL Default 0.0, " +
                "GrossCUFTRemvPP REAL Default 0.0, " +
                "GrossBDFTSP REAL Default 0.0, " +
                "NetBDFTSP REAL Default 0.0, " +
                "GrossCUFTSP REAL Default 0.0, " +
                "NetCUFTSP REAL Default 0.0, " +
                "CordsSP REAL Default 0.0, " +
                "GrossCUFTRemvSP REAL Default 0.0, " +
                "NumberlogsMS REAL Default 0.0, " +
                "NumberlogsTPW REAL Default 0.0, " +
                "GrossBDFTRP REAL Default 0.0, " +
                "GrossCUFTRP REAL Default 0.0, " +
                "CordsRP REAL Default 0.0, " +
                "GrossBDFTIntl REAL Default 0.0, " +
                "NetBDFTIntl REAL Default 0.0, " +
                "BiomassMainStemPrimary REAL Default 0.0, " +
                "BiomassMainStemSecondary REAL Default 0.0, " +
                "ValuePP REAL Default 0.0, " +
                "ValueSP REAL Default 0.0, " +
                "ValueRP REAL Default 0.0, " +
                "BiomassProd REAL Default 0.0, " +
                "Biomasstotalstem REAL Default 0.0, " +
                "Biomasslivebranches REAL Default 0.0, " +
                "Biomassdeadbranches REAL Default 0.0, " +
                "Biomassfoliage REAL Default 0.0, " +
                "BiomassTip REAL Default 0.0, " +
                "TipwoodVolume REAL Default 0.0, " +
                "ExpansionFactor REAL Default 0.0, " +
                "TreeFactor REAL Default 0.0, " +
                "PointFactor REAL Default 0.0, " +
                "UNIQUE (Tree_CN)," +
                "FOREIGN KEY (Tree_CN) REFERENCES Tree_V3 (Tree_CN) ON DELETE CASCADE ON UPDATE CASCADE " +
            ");";
    }
}