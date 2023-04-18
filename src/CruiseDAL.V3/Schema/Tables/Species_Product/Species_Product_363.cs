using System;
using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class Species_ProductTableDefinition_363 : ITableDefinition
    {
        public string TableName => "Species_Product";

        public string CreateTable => GetCreateTable(TableName);

        public string InitializeTable => null;

        public string CreateTombstoneTable => null;

        public string CreateIndexes =>
@"CREATE UNIQUE INDEX UIX_Species_Product_CruiseID_SpeciesCode_PrimaryProduct ON Species_Product
(CruiseID, SpeciesCode, ifnull(PrimaryProduct, '') COLLATE NOCASE);";

        public IEnumerable<string> CreateTriggers => null;

        public string GetCreateTable(string tableName)
        {
            return $@"CREATE TABLE {tableName} (
    Species_Product_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    SpeciesCode TEXT NOT NULL COLLATE NOCASE,
    PrimaryProduct TEXT COLLATE NOCASE,
    ContractSpecies TEXT NOT NULL COLLATE NOCASE,

    CHECK (length(trim(ContractSpecies)) > 0)

    FOREIGN KEY (SpeciesCode, CruiseID) REFERENCES Species (SpeciesCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (PrimaryProduct) REFERENCES LK_Product (Product)
);
    ";
        }
    }
}