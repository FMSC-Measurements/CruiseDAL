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

        tm.SeenDefectPrimary, 
        tm.SeenDefectSecondary, 
        tm.RecoverablePrimary, 
        tm.HiddenPrimary, 
        tm.Grade, 

        tm.HeightToFirstLiveLimb, 
        tm.PoleLength, 
        tm.ClearFace, 
        tm.CrownRatio, 
        tm.DBH, 

        tm.DRC, 
        tm.TotalHeight, 
        tm.MerchHeightPrimary, 
        tm.MerchHeightSecondary, 
        tm.FormClass, 

        tm.UpperStemDiameter, 
        tm.UpperStemHeight, 
        tm.DBHDoubleBarkThickness, 
        tm.TopDIBPrimary, 
        tm.TopDIBSecondary, 

        tm.DefectCode, 
        tm.DiameterAtDefect, 
        tm.VoidPercent, 
        tm.Slope, 
        tm.Aspect, 

        tm.Remarks, 
        tm.IsFallBuckScale, 

        tm.MetaData, 
        tm.Initials 

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
LEFT JOIN {fromDbName}.TreeMeasurment_DefaultResolved AS tm USING (TreeID)
WHERE t.CruiseID = '{cruiseID}'
;";
        }
    }
}