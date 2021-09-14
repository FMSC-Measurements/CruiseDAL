using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class BiomassEquationTableDefinition : ITableDefinition
    {
        public string TableName => "BiomassEquation";

        public string CreateTable =>
@"CREATE TABLE BiomassEquation (
	CruiseID TEXT NOT NULL COLLATE NOCASE,
	Species TEXT NOT NULL,
	Product TEXT NOT NULL,
	Component TEXT NOT NULL,
	LiveDead TEXT NOT NULL,
	FIAcode INTEGER NOT NULL,
	Equation TEXT,
	PercentMoisture REAL Default 0.0,
	PercentRemoved REAL Default 0.0,
	MetaData TEXT,
	WeightFactorPrimary REAL Default 0.0,
	WeightFactorSecondary REAL Default 0.0,

    CreatedBy TEXT DEFAULT 'none',
    Created_TS DATETIME DEFAULT (CURRENT_TIMESTAMP),
    ModifiedBy TEXT,
    Modified_TS DATETIME,

	UNIQUE (CruiseID, Species, Product, Component, LiveDead),

	FOREIGN KEY (CruiseID) REFERENCES Cruise (CruiseID) ON DELETE CASCADE

);";

        public string InitializeTable => null;

        public string CreateTombstoneTable => null;

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => null;
    }
}