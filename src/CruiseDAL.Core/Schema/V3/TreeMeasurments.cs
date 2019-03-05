using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.V3
{
    public partial class DDL
    {
        public const string CREATE_TABLE_TREEMEASURMENT =
            "CREATE TABLE TreeMeasurment ( " +
                "TreeMeasurment_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "TreeID TEXT NOT NULL, " +
                "SeenDefectPrimary REAL Default 0.0, " +
                "SeenDefectSecondary REAL Default 0.0, " +
                "RecoverablePrimary REAL Default 0.0, " +
                "HiddenPrimary REAL Default 0.0, " +
                "Grade TEXT, " +
                "HeightToFirstLiveLimb REAL Default 0.0, " +
                "PoleLength REAL Default 0.0, " +
                "ClearFace TEXT, " +
                "CrownRatio REAL Default 0.0, " +
                "DBH REAL Default 0.0, " +
                "DRC REAL Default 0.0, " +
                "TotalHeight REAL Default 0.0, " +
                "MerchHeightPrimary REAL Default 0.0, " +
                "MerchHeightSecondary REAL Default 0.0, " +
                "FormClass REAL Default 0.0, " +
                "UpperStemDOB REAL Default 0.0, " +
                "UpperStemDiameter REAL Default 0.0, " +
                "UpperStemHeight REAL Default 0.0, " +
                "DBHDoubleBarkThickness REAL Default 0.0, " +
                "TopDIBPrimary REAL Default 0.0, " +
                "TopDIBSecondary REAL Default 0.0, " +
                "DefectCode TEXT, " +
                "DiameterAtDefect REAL Default 0.0, " +
                "VoidPercent REAL Default 0.0, " +
                "Slope REAL Default 0.0, " +
                "Aspect REAL Default 0.0, " +
                "Remarks TEXT, " +
                "XCoordinate DOUBLE Default 0.0, " +
                "YCoordinate DOUBLE Default 0.0, " +
                "ZCoordinate DOUBLE Default 0.0, " +
                "MetaData TEXT, " +
                "IsFallBuckScale INTEGER Default 0, " +
                "CreatedBy TEXT DEFAULT 'none', " +
                "CreatedDate DateTime DEFAULT (datetime('now', 'localtime')) , " +
                "ModifiedBy TEXT, " +
                "ModifiedDate DateTime , " +
                "RowVersion INTEGER DEFAULT 0, " +
                "UNIQUE (TreeID), " +
                "FOREIGN KEY (TreeID) REFERENCES Tree_V3 (TreeID) ON DELETE CASCADE ON UPDATE CASCADE" +
            ")";

        public const string CREATE_TRIGGER_TREEMEASURMENTS_ONUPDATE =
            "CREATE TRIGGER TREEMEASURMENT_ONUPDATE " +
            "AFTER UPDATE OF " +
                "SeenDefectPrimary, " +
                "SeenDefectSecondary, " +
                "RecoverablePrimary, " +
                "HiddenPrimary, Grade, " +
                "HeightToFirstLiveLimb, " +
                "PoleLength, " +
                "ClearFace, " +
                "CrownRatio, " +
                "DBH, " +
                "DRC, " +
                "TotalHeight, " +
                "MerchHeightPrimary, " +
                "MerchHeightSecondary, " +
                "FormClass, " +
                "UpperStemDOB, " +
                "UpperStemDiameter, " +
                "UpperStemHeight, " +
                "DBHDoubleBarkThickness, " +
                "TopDIBPrimary, " +
                "TopDIBSecondary, " +
                "DefectCode, " +
                "DiameterAtDefect, " +
                "VoidPercent, " +
                "Slope, " +
                "Aspect, " +
                "Remarks, " +
                "XCoordinate, " +
                "YCoordinate, " +
                "ZCoordinate, " +
                "MetaData, " +
                "IsFallBuckScale " +
            "ON TreeMeasurment " +
            "FOR EACH ROW " +
            "BEGIN " +
                "UPDATE TreeMeasurment SET RowVersion = old.RowVersion + 1 WHERE TreeMeasurment_CN = old.TreeMeasusrment_CN; " +
                "UPDATE TreeMeasurment SET ModifiedDate = datetime('now', 'localtime') WHERE TreeMeasurment_CN = old.TreeMeasusrment_CN; " +
            "END;";
    }

    public partial class Updater
    {
        public const string INITIALIZE_TREEMEASURMENTS_FROM_TREE =
            "INSERT INTO TreeMeasurments " +
            "SELECT " +
            "Tree_GUID AS TreeID, " +
            "SeenDefectPrimary, " +
            "SeenDefectSecondary, " +
            "RecoverablePrimary, " +
            "HiddenPrimary, " +
            "Grade, " +
            "HeightToFirstLiveLimb, " +
            "PoleLength, " +
            "ClearFace, " +
            "CrownRatio, " +
            "DBH, " +
            "DRC, " +
            "TotalHeight, " +
            "MerchHeightPrimary, " +
            "MerchHeightSecondary, " +
            "FormClass, " +
            "UpperStemDOB, " +
            "UpperStemDiameter, " +
            "UpperStemHeight, " +
            "DBHDoubleBarkThickness, " +
            "TopDIBPrimary, " +
            "TopDIBSecondary, " +
            "DefectCode, " +
            "DiameterAtDefect, " +
            "VoidPercent, " +
            "Slope, " +
            "Aspect, " +
            "Remarks, " +
            "XCoordinate, " +
            "YCoordinate, " +
            "ZCoordinate, " +
            "MetaData, " +
            "IsFallBuckScale, " +
            "CreatedBy, " +
            "CreatedDate, " +
            "ModifiedBy, " +
            "ModifiedDate, " +
            "RowVersion " +
            "FROM Tree AS t WHERE CountOrMeasure = 'M'; ";
    }
}
