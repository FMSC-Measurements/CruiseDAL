using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Schema.Views
{
    public class TreeFieldValue_TreeMeasurmentViewDefinition : IViewDefinition
    {
        public string ViewName => "TreeFieldValue_TreeMeasurment";

        public string CreateView =>
@"CREATE VIEW TreeFieldValue_TreeMeasurment AS
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
    NULL AS CreatedDate

FROM TreeMeasurment AS tm
CROSS JOIN TreeField AS tf
WHERE tf.IsTreeMeasurmentField = 1;";

    }
}
