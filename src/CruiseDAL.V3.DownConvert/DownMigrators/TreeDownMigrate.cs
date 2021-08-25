namespace CruiseDAL.DownMigrators
{
    public class TreeDownMigrate : IDownMigrator
    {
        public string CreateCommand(string toDbName, string fromDbName, string cruiseID, string createdBy)
        {
            return
$@"INSERT INTO {toDbName}.Tree (
    Tree_CN,
    Tree_GUID,
    CuttingUnit_CN,
    Stratum_CN,
    SampleGroup_CN,
    TreeDefaultValue_CN,
    Plot_CN,
    Species,
    LiveDead,
    TreeNumber,
    CountOrMeasure,
    TreeCount,
    KPI,
    STM,

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
    IsFallBuckScale,

    MetaData,
    Initials
)
SELECT
    t.Tree_CN,
    t.TreeID AS Tree_GUID,
    cu.CuttingUnit_CN,
    st.Stratum_CN,
    sg.SampleGroup_CN,
    tdv.TreeDefaultValue_CN,
    Plot_Stratum_CN AS Plot_CN,
    t.SpeciesCode AS Species,
    t.LiveDead,
    t.TreeNumber,
    t.CountOrMeasure,
    CAST (
        (CASE WHEN plt.Plot_Stratum_CN IS NULL
                THEN 0
                ELSE (ifnull(tl.TreeCount, 0))
        END)
        AS REAL) AS TreeCount, -- in v2 TreeCount and kpi had a type of REAL
    CAST (ifnull(tl.KPI, 0) AS REAL) AS KPI,
    (CASE ifnull(tl.STM, 0) WHEN 0 THEN 'N' WHEN 1 THEN 'Y' END) AS STM,

ifnull(tm.SeenDefectPrimary, 
            (SELECT tfs.DefaultValueReal FROM {fromDbName}.TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'SeenDefectPrimary' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)),
        ifnull(tm.SeenDefectSecondary, 
            (SELECT DefaultValueReal FROM {fromDbName}.TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'SeenDefectSecondary' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)),
        ifnull(tm.RecoverablePrimary, 
            (SELECT DefaultValueReal FROM {fromDbName}.TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'RecoverablePrimary' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)),
        ifnull(tm.HiddenPrimary, 
            (SELECT DefaultValueReal FROM {fromDbName}.TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'HiddenPrimary' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)),
        ifnull(tm.Grade, 
            (SELECT DefaultValueText FROM {fromDbName}.TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'Grade' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)),

        ifnull(tm.HeightToFirstLiveLimb, 
            (SELECT DefaultValueReal FROM {fromDbName}.TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'HeightToFirstLiveLimb' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)),
        ifnull(tm.PoleLength, 
            (SELECT DefaultValueReal FROM {fromDbName}.TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'PoleLength' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)),
        ifnull(tm.ClearFace, 
            (SELECT DefaultValueText FROM {fromDbName}.TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'ClearFace' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)),
        ifnull(tm.CrownRatio, 
            (SELECT DefaultValueReal FROM {fromDbName}.TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'CrownRatio' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)),
        ifnull(tm.DBH, 
            (SELECT DefaultValueReal FROM {fromDbName}.TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'DBH' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)),

        ifnull(tm.DRC, 
            (SELECT DefaultValueReal FROM {fromDbName}.TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'DRC' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)),
        ifnull(tm.TotalHeight, 
            (SELECT DefaultValueReal FROM {fromDbName}.TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'TotalHeight' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)),
        ifnull(tm.MerchHeightPrimary, 
            (SELECT DefaultValueReal FROM {fromDbName}.TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'MerchHeightPrimary' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)),
        ifnull(tm.MerchHeightSecondary, 
            (SELECT DefaultValueReal FROM {fromDbName}.TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'MerchHeightSecondary' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)),
        ifnull(tm.FormClass, 
            (SELECT DefaultValueReal FROM {fromDbName}.TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'FormClass' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)),

        ifnull(tm.UpperStemDiameter, 
            (SELECT DefaultValueReal FROM {fromDbName}.TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'UpperStemDiameter' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)),
        ifnull(tm.UpperStemHeight, 
            (SELECT DefaultValueReal FROM {fromDbName}.TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'UpperStemHeight' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)),
        ifnull(tm.DBHDoubleBarkThickness, 
            (SELECT DefaultValueReal FROM {fromDbName}.TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'DBHDoubleBarkThickness' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)),
        ifnull(tm.TopDIBPrimary, 
            (SELECT DefaultValueReal FROM {fromDbName}.TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'TopDIBPrimary' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)),
        ifnull(tm.TopDIBSecondary, 
            (SELECT DefaultValueReal FROM {fromDbName}.TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'TopDIBSecondary' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)),

        ifnull(tm.DefectCode, 
            (SELECT DefaultValueText FROM {fromDbName}.TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'DefectCode' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)),
        ifnull(tm.DiameterAtDefect, 
            (SELECT DefaultValueReal FROM {fromDbName}.TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'DiameterAtDefect' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)),
        ifnull(tm.VoidPercent, 
            (SELECT DefaultValueReal FROM {fromDbName}.TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'VoidPercent' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)),
        ifnull(tm.Slope, 
            (SELECT DefaultValueReal FROM {fromDbName}.TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'Slope' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)),
        ifnull(tm.Aspect, 
            (SELECT DefaultValueReal FROM {fromDbName}.TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'Aspect' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)),

        ifnull(tm.Remarks, 
            (SELECT DefaultValueText FROM {fromDbName}.TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'Remarks' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)),
        ifnull(tm.IsFallBuckScale, 
            (SELECT DefaultValueText FROM {fromDbName}.TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'IsFallBuckScale' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)),

        ifnull(tm.MetaData, 
            (SELECT DefaultValueText FROM {fromDbName}.TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'MetaData' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)),
        ifnull(tm.Initials, 
            (SELECT DefaultValueText FROM {fromDbName}.TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'Initials' ORDER BY tfs.SampleGroupCode DESC LIMIT 1))

FROM {fromDbName}.Tree AS t
JOIN {fromDbName}.SampleGroup AS sg USING (StratumCode, SampleGroupCode, CruiseID)
JOIN {fromDbName}.Stratum AS st USING (StratumCode, CruiseID)
JOIN {fromDbName}.CuttingUnit AS cu USING (CuttingUnitCode, CruiseID)
LEFT JOIN {fromDbName}.Plot_Stratum AS plt USING (CuttingUnitCode, StratumCode, PlotNumber, CruiseID)
LEFT JOIN {toDbName}.TreeDefaultValue AS tdv ON
            tdv.Species = t.SpeciesCode
            AND tdv.LiveDead = t.LiveDead
            AND tdv.PrimaryProduct = sg.PrimaryProduct
LEFT JOIN {fromDbName}.TallyLedger_Tree_Totals AS tl ON tl.TreeID = t.TreeID
LEFT JOIN {fromDbName}.TreeMeasurment AS tm ON t.TreeID = tm.TreeID
WHERE t.CruiseID = '{cruiseID}'
;";
        }
    }
}