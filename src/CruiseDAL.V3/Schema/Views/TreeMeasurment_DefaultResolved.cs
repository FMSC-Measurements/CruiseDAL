using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Schema
{
    public class TreeMeasurment_DefaultResolvedViewDefinition : IViewDefinition
    {
        public string ViewName => "TreeMeasurment_DefaultResolved";

        public string CreateView =>
@"CREATE VIEW TreeMeasurment_DefaultResolved AS 
SELECT
        tm.TreeID,

        -- for each tree field we will resolve the value by taking the first non-null value in the following order
            -- first try reading the value from the TreeMeasurments table, treating the default value (0.0, 0, '') depending on data type as null
            -- next we will try getting the default value from the TreeFieldSetup table, by matching with either the stratum level or samplegroup level field setup
            -- if the field is a field in the TreeDefaultValues table we will get populate the value from the tree default value table
            -- last we will use the value in the TreeMeasurments table which we know is the default value for the field...
                -- note: should we just force the 


        -- MEASURMENT FIELDS
        coalesce(nullif(tm.SeenDefectPrimary, 0.0),
                (SELECT tfs.DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'SeenDefectPrimary' ORDER BY tfs.SampleGroupCode DESC LIMIT 1),
                tm.SeenDefectPrimary) AS SeenDefectPrimary,
        coalesce(nullif(tm.SeenDefectSecondary, 0.0), 
                (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'SeenDefectSecondary' ORDER BY tfs.SampleGroupCode DESC LIMIT 1),
                tm.SeenDefectSecondary) AS SeenDefectSecondary,
        coalesce(nullif(tm.RecoverablePrimary, 0.0), 
                (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'RecoverablePrimary' ORDER BY tfs.SampleGroupCode DESC LIMIT 1),
                tdv.Recoverable, -- if tree RecoverablePrimary value not defined get value from TDV
                tm.RecoverablePrimary) AS RecoverablePrimary,

        coalesce(nullif(tm.HiddenPrimary, 0.0), 
                (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'HiddenPrimary' ORDER BY tfs.SampleGroupCode DESC LIMIT 1),
                CASE WHEN t.LiveDead != 'D' THEN tdv.HiddenPrimary ELSE tdv.HiddenPrimaryDead END, -- if tree HiddenPrimary value not defined get value from TDV
                tm.HiddenPrimary) AS HiddenPrimary,

        coalesce(nullif(tm.Grade, ''), 
                (SELECT DefaultValueText FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'Grade' ORDER BY tfs.SampleGroupCode DESC LIMIT 1),
                CASE WHEN t.LiveDead != 'D' THEN tdv.TreeGrade ELSE tdv.TreeGradeDead END, -- if tree Grade value not defined get value from TDV
                tm.Grade,
                '0' ) AS Grade, -- if Grade not defined on tree or in defaults use '0' as default value

        coalesce(nullif(tm.HeightToFirstLiveLimb, 0.0), 
                (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'HeightToFirstLiveLimb' ORDER BY tfs.SampleGroupCode DESC LIMIT 1),
                tm.HeightToFirstLiveLimb) AS HeightToFirstLiveLimb,
        coalesce(nullif(tm.PoleLength, 0.0), 
                (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'PoleLength' ORDER BY tfs.SampleGroupCode DESC LIMIT 1),
                tm.PoleLength) AS PoleLength,
        coalesce(nullif(tm.ClearFace, ''), 
                (SELECT DefaultValueText FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'ClearFace' ORDER BY tfs.SampleGroupCode DESC LIMIT 1),
                tm.ClearFace) AS ClearFace,
        coalesce(nullif(tm.CrownRatio, 0.0), 
                (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'CrownRatio' ORDER BY tfs.SampleGroupCode DESC LIMIT 1),
                tm.CrownRatio) AS CrownRatio,
        coalesce(nullif(tm.DBH, 0.0), 
                (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'DBH' ORDER BY tfs.SampleGroupCode DESC LIMIT 1),
                tm.DBH) AS DBH,

        coalesce(nullif(tm.DRC, 0.0), 
                (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'DRC' ORDER BY tfs.SampleGroupCode DESC LIMIT 1),
                tm.DRC) AS DRC,
        coalesce(nullif(tm.TotalHeight, 0.0), 
                (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'TotalHeight' ORDER BY tfs.SampleGroupCode DESC LIMIT 1),
                tm.TotalHeight) AS TotalHeight,
        coalesce(nullif(tm.MerchHeightPrimary, 0.0), 
                (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'MerchHeightPrimary' ORDER BY tfs.SampleGroupCode DESC LIMIT 1),
                tm.MerchHeightPrimary) AS MerchHeightPrimary,
        coalesce(nullif(tm.MerchHeightSecondary, 0.0), 
                (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'MerchHeightSecondary' ORDER BY tfs.SampleGroupCode DESC LIMIT 1),
                tm.MerchHeightSecondary) AS MerchHeightSecondary,
        coalesce(nullif(tm.FormClass, 0.0), 
                (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'FormClass' ORDER BY tfs.SampleGroupCode DESC LIMIT 1),
                tdv.FormClass, -- if tree FormClass value not defined get value from TDV
                tm.FormClass) AS FormClass,

        coalesce(nullif(tm.UpperStemDiameter, 0.0), 
                (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'UpperStemDiameter' ORDER BY tfs.SampleGroupCode DESC LIMIT 1),
                tm.UpperStemDiameter) AS UpperStemDiameter,
        coalesce(nullif(tm.UpperStemHeight, 0.0), 
                (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'UpperStemHeight' ORDER BY tfs.SampleGroupCode DESC LIMIT 1),
                tm.UpperStemHeight) AS UpperStemHeight,
        coalesce(nullif(tm.DBHDoubleBarkThickness, 0.0), 
                (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'DBHDoubleBarkThickness' ORDER BY tfs.SampleGroupCode DESC LIMIT 1),
                tm.DBHDoubleBarkThickness) AS DBHDoubleBarkThickness,
        coalesce(nullif(tm.TopDIBPrimary, 0.0), 
                (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'TopDIBPrimary' ORDER BY tfs.SampleGroupCode DESC LIMIT 1),
                tm.TopDIBPrimary) AS TopDIBPrimary,
        coalesce(nullif(tm.TopDIBSecondary, 0.0), 
                (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'TopDIBSecondary' ORDER BY tfs.SampleGroupCode DESC LIMIT 1),
                tm.TopDIBSecondary) AS TopDIBSecondary,

        coalesce(nullif(tm.DefectCode, ''), 
                (SELECT DefaultValueText FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'DefectCode' ORDER BY tfs.SampleGroupCode DESC LIMIT 1),
                tm.DefectCode) AS DefectCode,
        coalesce(nullif(tm.DiameterAtDefect, 0.0), 
                (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'DiameterAtDefect' ORDER BY tfs.SampleGroupCode DESC LIMIT 1),
                tm.DiameterAtDefect) AS DiameterAtDefect,
        coalesce(nullif(tm.VoidPercent, 0.0), 
                (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'VoidPercent' ORDER BY tfs.SampleGroupCode DESC LIMIT 1),
                tm.VoidPercent) AS VoidPercent,
        coalesce(nullif(tm.Slope, 0.0), 
                (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'Slope' ORDER BY tfs.SampleGroupCode DESC LIMIT 1),
                tm.Slope) AS Slope,
        coalesce(nullif(tm.Aspect, 0.0), 
                (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'Aspect' ORDER BY tfs.SampleGroupCode DESC LIMIT 1),
                tm.Aspect) AS Aspect,

        coalesce(nullif(tm.Remarks, ''), 
                (SELECT DefaultValueText FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'Remarks' ORDER BY tfs.SampleGroupCode DESC LIMIT 1),
                tm.Remarks) AS Remarks,
        coalesce(nullif(tm.IsFallBuckScale, 0), 
                (SELECT DefaultValueText FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'IsFallBuckScale' ORDER BY tfs.SampleGroupCode DESC LIMIT 1),
                tm.IsFallBuckScale) AS IsFallBuckScale,

        coalesce(nullif(tm.MetaData, ''), 
                (SELECT DefaultValueText FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'MetaData' ORDER BY tfs.SampleGroupCode DESC LIMIT 1),
                tm.MetaData) AS MetaData, 
        coalesce(nullif(tm.Initials, ''), 
                (SELECT DefaultValueText FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'Initials' ORDER BY tfs.SampleGroupCode DESC LIMIT 1),
                tm.Initials) AS Initials

FROM TreeMeasurment AS tm
JOIN Tree AS t USING (TreeID)
JOIN Tree_TreeDefaultValue USING (TreeID)
LEFT JOIN TreeDefaultValue AS tdv USING (TreeDefaultValue_CN)
";
    }
}
