using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_Cruise =
@"CREATE TABLE Cruise ( 
    CruiseID TEXT NOT NULL COLLATE NOCASE, 
    SaleID TEXT NOT NULL COLLATE NOCASE, 
    Purpose TEXT,
    Remarks TEXT,
    DefaultUOM TEXT,
    LogGradingEnabled BOOLEAN Default 0,
    CreatedBy TEXT DEFAULT 'none',
    CreatedDate DateTime DEFAULT (datetime('now', 'localtime')),
    ModifiedBy TEXT,
    ModifiedDate DateTime,
    
    CHECK (CruiseID LIKE '________-____-____-____-____________'),

    FOREIGN KEY (SaleID) REFERENCES Sale (SaleID) ON DELETE CASCADE,
    UNIQUE (CruiseID)
);";

        public const string CREATE_TRIGGER_CRUISE_ONUPDATE =
@"CREATE TRIGGER OnUpdateCruise 
AFTER UPDATE OF 
    SaleID, 
    Purpose, 
    LogGradingEnabled, 
    Remarks, 
    DefaultUOM 
ON Sale 
BEGIN 
    UPDATE Sale SET ModifiedDate = datetime('now', 'localtime') WHERE Sale_CN = old.Sale_CN; 
    UPDATE Sale SET RowVersion = old.RowVersion + 1 WHERE Sale_CN = old.Sale_CN;
END;";

    }

    public partial class Migrations
    {
        public const string MIGRATE_Cruise_FORMAT_STR =
@"INSERT INTO {0}.Cruise ( 
        CruiseID,
        SaleID, 
        Purpose, 
        DefaultUOM,
        LogGradingEnabled,
        Remarks,
        CreatedBy, 
        CreatedDate, 
        ModifiedBy 
    ) 
    SELECT 
        '{3}',
        '{2}',
        Purpose, 
        DefaultUOM,
        LogGradingEnabled,
        Remarks,
        CreatedBy, 
        CreatedDate, 
        ModifiedBy
    FROM {1}.Sale;";
    }
}
