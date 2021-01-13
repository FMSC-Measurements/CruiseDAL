using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Schema.Views
{
    public class TreeFieldValue_TreeMeasurment_FilteredViewDefinition : IViewDefinition
    {
        public string ViewName => "TreeFieldValue_TreeMeasurment_Filtered";

        // only returns tree field values that match fields in tree field setup
        public string CreateView =>
@"CREATE VIEW TreeFieldValue_TreeMeasurment_Filtered AS
SELECT
    tm.TreeID,
    tf.Field,
    tf.DbType,
    (CASE tf.Field
        WHEN 'SeenDefectPrimary' THEN SeenDefectPrimary
        WHEN 'SeenDefectSecondary' THEN SeenDefectSecondary
        WHEN 'RecoverablePrimary' THEN RecoverablePrimary
        WHEN 'HiddenPrimary' THEN HiddenPrimary
        WHEN 'HeightToFirstLiveLimb' THEN HeightToFirstLiveLimb
        WHEN 'PoleLength' THEN PoleLength
        WHEN 'CrownRatio' THEN CrownRatio
        WHEN 'DBH' THEN DBH
        WHEN 'DRC' THEN DRC
        WHEN 'TotalHeight' THEN TotalHeight
        WHEN 'MerchHeightPrimary' THEN MerchHeightPrimary
        WHEN 'MerchHeightSecondary' THEN MerchHeightSecondary
        WHEN 'FormClass' THEN FormClass
        WHEN 'UpperStemDiameter' THEN UpperStemDiameter
        WHEN 'UpperStemHeight' THEN UpperStemHeight
        WHEN 'DBHDoubleBarkThickness' THEN DBHDoubleBarkThickness
        WHEN 'TopDIBPrimary' THEN TopDIBPrimary
        WHEN 'TopDIBSecondary' THEN TopDIBSecondary
        WHEN 'DiameterAtDefect' THEN DiameterAtDefect
        WHEN 'VoidPercent' THEN VoidPercent
        WHEN 'Slope' THEN Slope
        WHEN 'Aspect' THEN Aspect
        --WHEN 'XCoordinate' THEN XCoordinate
        --WHEN 'YCoordinate' THEN YCoordinate
        --WHEN 'ZCoordinate' THEN ZCoordinate
        ELSE NULL END) AS ValueReal,

    (CASE tf.Field
        WHEN 'IsFallBuckScale' THEN IsFallBuckScale
        ELSE NULL END) AS ValueBool,

    (CASE tf.Field
        WHEN 'Grade' THEN Grade
        WHEN 'ClearFace' THEN ClearFace
        WHEN 'DefectCode' THEN DefectCode
        WHEN 'Remarks' THEN Remarks
        WHEN 'MetaData' THEN MetaData
        WHEN 'Initials' THEN Initials
        ELSE NULL END) AS ValueText,

    NULL AS ValueInt,
    tm.CreatedBy,
    tm.Created_TS,
    tm.ModifiedBy,
    tm.Modified_TS

FROM TreeMeasurment AS tm
JOIN Tree AS t USING (TreeID)
CROSS JOIN TreeFieldSetup AS tfs
JOIN TreeField AS tf USING (Field)
WHERE tf.IsTreeMeasurmentField = 1;";
    }
}
