﻿using System.Collections.Generic;
using System.Linq;

namespace CruiseDAL.Schema
{
    public class SpeciesTableDefinition : ITableDefinition
    {
        public string TableName => "Species";

        public string CreateTable => GetCreateTable(TableName);

        public string GetCreateTable(string tableName)
        {
            return $@"CREATE TABLE {tableName} (
    Species_cn INTEGER PRIMARY KEY AUTOINCREMENT,
    SpeciesCode TEXT COLLATE NOCASE,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    ContractSpecies TEXT,
    FIACode TEXT COLLATE NOCASE,
    CHECK (length(SpeciesCode) > 0),

    --FOREIGN KEY (FIACode) REFERENCES LK_FIA (FIACode),

    UNIQUE (SpeciesCode, CruiseID)
);";
        }

        public string InitializeTable => null;

        public string CreateTombstoneTable => null;

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => new[] { CREATE_TRIGGER_Species_OnUpdate_ContractSpecies,};

        public const string CREATE_TRIGGER_Species_OnUpdate_ContractSpecies =
@"CREATE TRIGGER Species_OnUpdate_ContractSpecies 
AFTER UPDATE OF ContractSpecies 
ON Species
FOR EACH ROW
BEGIN
    INSERT OR REPLACE INTO Species_Product (
        CruiseID, 
        SpeciesCode, 
        PrimaryProduct, 
        ContractSpecies
    ) VALUES (
        NEW.CruiseID,
        NEW.SpeciesCode,
        NULL,
        NEW.ContractSpecies
    );
END;";
    }
}