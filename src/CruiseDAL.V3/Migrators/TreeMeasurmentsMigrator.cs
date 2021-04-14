using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public class TreeMeasurmentsMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID, string deviceID)
        {
            return
$@"INSERT INTO {toDbName}.TreeMeasurment (
        TreeMeasurment_CN,
        TreeID,
        SeenDefectPrimary,
        SeenDefectSecondary,
        RecoverablePrimary,
        HiddenPrimary,
        Grade,
        HeightToFirstLiveLimb,
        PoleLength,
        ClearFace,
        CrownRatio,
        DBH,
        DRC,
        TotalHeight,
        MerchHeightPrimary,
        MerchHeightSecondary,
        FormClass,
        --UpperStemDOB,
        UpperStemDiameter,
        UpperStemHeight,
        DBHDoubleBarkThickness,
        TopDIBPrimary,
        TopDIBSecondary,
        DefectCode,
        DiameterAtDefect,
        VoidPercent,
        Slope,
        Aspect,
        Remarks,
        MetaData,
        IsFallBuckScale,
        Initials,
        CreatedBy
    )
    SELECT
        t.Tree_CN AS TreeMeasurment_CN,
        t3.TreeID AS TreeID,
        t.SeenDefectPrimary,
        t.SeenDefectSecondary,
        t.RecoverablePrimary,
        t.HiddenPrimary,
        t.Grade,
        t.HeightToFirstLiveLimb,
        t.PoleLength,
        t.ClearFace,
        t.CrownRatio,
        t.DBH,
        t.DRC,
        t.TotalHeight,
        t.MerchHeightPrimary,
        t.MerchHeightSecondary,
        t.FormClass,
        --UpperStemDOB,
        t.UpperStemDiameter,
        t.UpperStemHeight,
        t.DBHDoubleBarkThickness,
        t.TopDIBPrimary,
        t.TopDIBSecondary,
        t.DefectCode,
        t.DiameterAtDefect,
        t.VoidPercent,
        t.Slope,
        t.Aspect,
        t.Remarks,
        t.MetaData,
        t.IsFallBuckScale,
        t.Initials,
        '{deviceID}'
    FROM {fromDbName}.Tree AS t
    JOIN {toDbName}.Tree AS t3 USING (Tree_CN);";
        }
    }
}
