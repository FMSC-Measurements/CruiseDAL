namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_REGRESSION =
            "CREATE TABLE Regression( " +
                "Regression_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "rVolume TEXT, " +
                "rVolType TEXT, " +
                "rSpeices TEXT, " +
                "rProduct TEXT, " +
                "rLiveDead TEXT, " +
                "CoefficientA REAL Default 0.0, " +
                "CoefficientB REAL Default 0.0, " +
                "CoefficientC REAL Default 0.0, " +
                "TotalTrees INTEGER Default 0, " +
                "MeanSE REAL Default 0.0, " +
                "Rsquared REAL Default 0.0, " +
                "RegressModel TEXT, " +
                "rMinDbh REAL Default 0.0, " +
                "rMaxDbh REAL Default 0.0" +
            ");";
    }
}