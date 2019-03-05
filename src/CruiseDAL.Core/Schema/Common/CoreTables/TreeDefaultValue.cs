using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.Common
{
    public class TreeDefaultValue
    {
        public const string CREATE_TABLE_TREEDEFAULTVALUE =
            "CREATE TABLE TreeDefaultValue ( " +
                "TreeDefaultValue_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "PrimaryProduct TEXT NOT NULL COLLATE NOCASE, " +
                "Species TEXT NOT NULL COLLATE NOCASE, " +
                "LiveDead TEXT NOT NULL COLLATE NOCASE, " +
                "Chargeable TEXT, " +
                "FIAcode INTEGER DEFAULT 0, " +
                "CullPrimary REAL DEFAULT 0.0, " +
                "HiddenPrimary REAL DEFAULT 0.0, " +
                "CullSecondary REAL DEFAULT 0.0, " +
                "HiddenSecondary REAL DEFAULT 0.0, " +
                "Recoverable REAL DEFAULT 0.0, " +                
                "ContractSpecies TEXT, " +
                "TreeGrade TEXT DEFAULT '0' COLLATE NOCASE, " +
                "MerchHeightLogLength INTEGER DEFAULT 0, " +
                "MerchHeightType TEXT DEFAULT 'F' , " +
                "FormClass REAL DEFAULT 0.0, " +
                "BarkThicknessRatio REAL DEFAULT 0.0, " +
                "AverageZ REAL DEFAULT 0.0, " +
                "ReferenceHeightPercent REAL DEFAULT 0.0, " +
                "CreatedBy TEXT DEFAULT 'none', " +
                "CreatedDate DateTime DEFAULT (datetime( 'now', 'localtime')), " +
                "ModifiedBy TEXT, " +
                "ModifiedDate DateTime , " +
                "RowVersion INTEGER DEFAULT 0, " +
                "UNIQUE(PrimaryProduct, Species, LiveDead, Chargeable)" +
            ");";

        public const string CREATE_TRIGGER_TREEDEFAULTVALUE_ONUPDATE =
            "CREATE TRIGGER TreeDefaultValue_OnUpdate " +
            "AFTER UPDATE OF " +
                "PrimaryProduct, " +
                "Species, " +
                "LiveDead, " +
                "Chargeable, " +
                "FIAcode, " +
                "CullPrimary, " +
                "HiddenPrimary, " +
                "CullSecondary, " +
                "HiddenSecondary, " +
                "Recoverable, " +
                "ContractSpecies, " +
                "TreeGrade, " +
                "MerchHeightLogLength, " +
                "MerchHeightType, " +
                "FormClass, " +
                "BarkThicknessRatio, " +
                "AverageZ, " +
                "ReferenceHeightPercent " +
            "ON TreeDefaultValue " +
            "FOR EACH ROW " +
            "BEGIN " +
                "UPDATE TreeDefaultValue SET ModifiedDate = datetime( 'now', 'localtime') WHERE TreeDefaultValue_CN = new.TreeDefaultValue_CN; " +
                "UPDATE TreeDefaultValue SET RowVersion = old.RowVersion + 1 WHERE TreeDefaultValue_CN = old.TreeDefaultValue_CN; " +
            "END; ";
    }
}
