namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_VALUEEQUATION =
            "CREATE TABLE ValueEquation( " +
                "Species TEXT NOT NULL, " +
                "PrimaryProduct TEXT NOT NULL, " +
                "ValueEquationNumber TEXT, " +
                "Grade TEXT, " +
                "Coefficient1 REAL Default 0.0, " +
                "Coefficient2 REAL Default 0.0, " +
                "Coefficient3 REAL Default 0.0, " +
                "Coefficient4 REAL Default 0.0, " +
                "Coefficient5 REAL Default 0.0, " +
                "Coefficient6 REAL Default 0.0, " +
                "UNIQUE (Species, PrimaryProduct, ValueEquationNumber)" +
            ");";
    }
}