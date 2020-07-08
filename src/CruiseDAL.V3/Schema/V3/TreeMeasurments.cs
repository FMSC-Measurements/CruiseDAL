namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        //        public const string CREATE_VIEW_TREEMEASURMENT =
        //@"CREATE VIEW TreeMeasurment AS
        //SELECT
        //    t.TreeID,
        //    ifnull( MAX( CASE WHEN Field = 'SeenDefectPrimary' THEN ValueReal END), 0.0) AS SeenDefectPrimary,
        //    ifnull( MAX( CASE WHEN Field = 'SeenDefectSecondary' THEN ValueReal END), 0.0) AS SeenDefectSecondary,
        //    ifnull( MAX( CASE WHEN Field = 'RecoverablePrimary' THEN ValueReal END), 0.0) AS RecoverablePrimary,
        //    ifnull( MAX( CASE WHEN Field = 'HiddenPrimary' THEN ValueReal END), 0.0) AS HiddenPrimary,
        //    ifnull( MAX( CASE WHEN Field = 'HeightToFirstLiveLimb' THEN ValueReal END), 0.0) AS HeightToFirstLiveLimb,
        //    ifnull( MAX( CASE WHEN Field = 'PoleLength' THEN ValueReal END), 0.0) AS PoleLength,
        //    ifnull( MAX( CASE WHEN Field = 'CrownRatio' THEN ValueReal END), 0.0) AS CrownRatio,
        //    ifnull( MAX( CASE WHEN Field = 'DBH' THEN ValueReal END), 0.0) AS DBH,
        //    ifnull( MAX( CASE WHEN Field = 'DRC' THEN ValueReal END), 0.0) AS DRC,
        //    ifnull( MAX( CASE WHEN Field = 'TotalHeight' THEN ValueReal END), 0.0) AS TotalHeight,
        //    ifnull( MAX( CASE WHEN Field = 'MerchHeightPrimary' THEN ValueReal END), 0.0) AS MerchHeightPrimary,
        //    ifnull( MAX( CASE WHEN Field = 'MerchHeightSecondary' THEN ValueReal END), 0.0) AS MerchHeightSecondary,
        //    ifnull( MAX( CASE WHEN Field = 'FormClass' THEN ValueReal END), 0.0) AS FormClass,
        //    ifnull( MAX( CASE WHEN Field = 'UpperStemDiameter' THEN ValueReal END), 0.0) AS UpperStemDiameter,
        //    ifnull( MAX( CASE WHEN Field = 'UpperStemHeight' THEN ValueReal END), 0.0) AS UpperStemHeight,
        //    ifnull( MAX( CASE WHEN Field = 'DBHDoubleBarkThickness' THEN ValueReal END), 0.0) AS DBHDoubleBarkThickness,
        //    ifnull( MAX( CASE WHEN Field = 'TopDIBPrimary' THEN ValueReal END), 0.0) AS TopDIBPrimary,
        //    ifnull( MAX( CASE WHEN Field = 'TopDIBSecondary' THEN ValueReal END), 0.0) AS TopDIBSecondary,
        //    ifnull( MAX( CASE WHEN Field = 'DiameterAtDefect' THEN ValueReal END), 0.0) AS DiameterAtDefect,
        //    ifnull( MAX( CASE WHEN Field = 'VoidPercent' THEN ValueReal END), 0.0) AS VoidPercent,
        //    ifnull( MAX( CASE WHEN Field = 'Slope' THEN ValueReal END), 0.0) AS Slope,
        //    ifnull( MAX( CASE WHEN Field = 'Aspect' THEN ValueReal END), 0.0) AS Aspect,
        //    ifnull( MAX( CASE WHEN Field = 'XCoordinate' THEN ValueReal END), 0.0) AS XCoordinate,
        //    ifnull( MAX( CASE WHEN Field = 'YCoordinate' THEN ValueReal END), 0.0) AS YCoordinate,
        //    ifnull( MAX( CASE WHEN Field = 'ZCoordinate' THEN ValueReal END), 0.0) AS ZCoordinate,

        //    ifnull( MAX( CASE WHEN Field = 'IsFallBuckScale' THEN ValueBool END), 0) AS IsFallBuckScale,

        //    MAX( CASE WHEN Field = 'Grade' THEN ValueText END) AS Grade,
        //    MAX( CASE WHEN Field = 'ClearFace' THEN ValueText END) AS ClearFace,
        //    MAX( CASE WHEN Field = 'DefectCode' THEN ValueText END) AS DefectCode,
        //    MAX( CASE WHEN Field = 'Remarks' THEN ValueText END) AS Remarks,
        //    MAX( CASE WHEN Field = 'MetaData' THEN ValueText END) AS MetaData
        //FROM TreeFieldValue AS tfv
        //GROUP BY TreeID;";

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

                //"UpperStemDOB REAL Default 0.0, " +

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
                "IsFallBuckScale BOOLEAN Default 0, " +

                "MetaData TEXT, " +

                "Initials TEXT, " +
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
                //"UpperStemDOB, " +
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
                "MetaData, " +
                "IsFallBuckScale," +
                "Initials " +
            "ON TreeMeasurment " +
            "FOR EACH ROW " +
            "BEGIN " +
                "UPDATE TreeMeasurment SET RowVersion = old.RowVersion + 1 WHERE TreeMeasurment_CN = old.TreeMeasurment_CN; " +
                "UPDATE TreeMeasurment SET ModifiedDate = datetime('now', 'localtime') WHERE TreeMeasurment_CN = old.TreeMeasurment_CN; " +
            "END;";
    }

    public partial class Migrations
    {
        public const string MIGRATE_TREEMEASURMENT_FROM_TREE_FORMAT_STR =
            "INSERT INTO {0}.TreeMeasurment ( " +
                    "TreeMeasurment_CN, " +
                    "TreeID, " +
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
                    //"UpperStemDOB, " +
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
                    "MetaData, " +
                    "IsFallBuckScale, " +
                    "Initials, " +
                    "CreatedBy, " +
                    "CreatedDate, " +
                    "ModifiedBy, " +
                    "ModifiedDate, " +
                    "RowVersion " +
                ") " +
                "SELECT " +
                    "t.Tree_CN AS TreeMeasurment_CN, " +
                    "t3.TreeID AS TreeID, " +
                    "t.SeenDefectPrimary, " +
                    "t.SeenDefectSecondary, " +
                    "t.RecoverablePrimary, " +
                    "t.HiddenPrimary, " +
                    "t.Grade, " +
                    "t.HeightToFirstLiveLimb, " +
                    "t.PoleLength, " +
                    "t.ClearFace, " +
                    "t.CrownRatio, " +
                    "t.DBH, " +
                    "t.DRC, " +
                    "t.TotalHeight, " +
                    "t.MerchHeightPrimary, " +
                    "t.MerchHeightSecondary, " +
                    "t.FormClass, " +
                    //"UpperStemDOB, " +
                    "t.UpperStemDiameter, " +
                    "t.UpperStemHeight, " +
                    "t.DBHDoubleBarkThickness, " +
                    "t.TopDIBPrimary, " +
                    "t.TopDIBSecondary, " +
                    "t.DefectCode, " +
                    "t.DiameterAtDefect, " +
                    "t.VoidPercent, " +
                    "t.Slope, " +
                    "t.Aspect, " +
                    "t.Remarks, " +
                    "t.MetaData, " +
                    "t.IsFallBuckScale, " +
                    "t.Initials, " +
                    "t.CreatedBy, " +
                    "t.CreatedDate, " +
                    "t.ModifiedBy, " +
                    "t.ModifiedDate, " +
                    "t.RowVersion " +
                "FROM {1}.Tree AS t " +
                "JOIN {0}.Tree_V3 AS t3 USING (Tree_CN);";
    }

    //public partial class Updater
    //{
    //    public const string INITIALIZE_TREEMEASURMENTS_FROM_TREE =
    //        "INSERT INTO TreeMeasurment " +
    //        "SELECT " +
    //            "null AS TreeMeasurment_CN, " +
    //            "Tree_GUID AS TreeID, " +
    //            "SeenDefectPrimary, " +
    //            "SeenDefectSecondary, " +
    //            "RecoverablePrimary, " +
    //            "HiddenPrimary, " +
    //            "Grade, " +
    //            "HeightToFirstLiveLimb, " +
    //            "PoleLength, " +
    //            "ClearFace, " +
    //            "CrownRatio, " +
    //            "DBH, " +
    //            "DRC, " +
    //            "TotalHeight, " +
    //            "MerchHeightPrimary, " +
    //            "MerchHeightSecondary, " +
    //            "FormClass, " +
    //            //"UpperStemDOB, " +
    //            "UpperStemDiameter, " +
    //            "UpperStemHeight, " +
    //            "DBHDoubleBarkThickness, " +
    //            "TopDIBPrimary, " +
    //            "TopDIBSecondary, " +
    //            "DefectCode, " +
    //            "DiameterAtDefect, " +
    //            "VoidPercent, " +
    //            "Slope, " +
    //            "Aspect, " +
    //            "Remarks, " +
    //            "XCoordinate, " +
    //            "YCoordinate, " +
    //            "ZCoordinate, " +
    //            "MetaData, " +
    //            "IsFallBuckScale, " +
    //            "CreatedBy, " +
    //            "CreatedDate, " +
    //            "ModifiedBy, " +
    //            "ModifiedDate, " +
    //            "RowVersion " +
    //        "FROM Tree AS t WHERE CountOrMeasure = 'M'; ";
    //}
}