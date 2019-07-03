namespace CruiseDAL.Schema
{
    public static partial class DDL
    {
        public static string SET_DBVERSION =
            "INSERT INTO Globals (Block, Key, Value) VALUES ('Database', 'Version', '3.0.0'); ";

        public static string[] CREATE_COMMANDS = new string[]
        {
            //core tables
            CREATE_TABLE_SALE,
            CREATE_TRIGGER_SALE_ONUPDATE,

            CREATE_TABLE_CUTTINGUNIT,
            CREATE_TRIGGER_CUTTINGUNIT_ONUPDATE,

            CREATE_TABLE_STRATUM,
            CREATE_TRIGGER_STRATUM_ONUPDATE,

            CREATE_TABLE_CUTTINGUNIT_STRATUM,
            CREATE_INDEX_CUTTINGUNIT_STRATUM_STRATUMCODE,
            CREATE_INDEX_CuttingUnit_Stratum_CuttingUnitCode,

            CREATE_TABLE_TREEFIELD,
            INITIALIZE_TABLE_TREEFIELD,
            CREATE_TABLE_TREEFIELDSETUP_V3,
            CREATE_INDEX_TreeFieldSetup_V3_Field,
            CREATE_INDEX_TreeFieldSetup_V3_StratumCode,

            CREATE_TABLE_LOGFIELD,
            INITIALIZE_TABLE_LOGFIELD,
            CREATE_TABLE_LOGFIELDSETUP_V3,
            CREATE_INDEX_LogFieldSetup_V3_Field,
            CREATE_INDEX_LogFieldSetup_V3_StratumCode,

            CREATE_TABLE_SAMPLEGROUP_V3,
            CREATE_TRIGGER_SAMPLEGROUP_V3_ONUPDATE,
            CREATE_TALBE_SAMPLERSTATE,

            CREATE_TABLE_SpeciesCode,

            CREATE_TABLE_SUBPOPULATION,
            CREATE_INDEX_Subpopulation_Species,
            CREATE_INDEX_Subpopulation_StratumCode_SampleGroupCode,
            CREATE_INDEX_Subpopulation_StratumCode_SampleGroupCode_Species_LiveDead,

            //CREATE_TABLE_TALLYPOPULATION,
            CREATE_TABLE_TALLYDESCRIPTION,
            CREATE_INDEX_TallyDescription_Species,
            CREATE_INDEX_TallyDescription_StratumCode_SampleGroupCode_Species_LiveDead,
            CREATE_TABLE_TALLYHOTKEY,
            CREATE_INDEX_TallyHotKey_StratumCode_SampleGroupCode_Species_LiveDead,
            CREATE_INDEX_TallyHotKey_Species,

            CREATE_TABLE_PLOT_V3,
            CREATE_INDEX_Plot_V3_CuttingUnitCode,
            CREATE_TRIGGER_PLOT_V3_ONUPDATE,

            CREATE_TABLE_PLOT_STRATUM,
            CREATE_INDEX_Plot_Stratum_StratumCode,
            CREATE_TRIGGER_PLOT_STRATUM_ONUPDATE,

            CREATE_TABLE_TREE_V3,
            CREATE_INDEX_Tree_V3_CuttingUnitCode,
            CREATE_INDEX_Tree_V3_PlotNumber_CuttingUnitCode,
            CREATE_INDEX_Tree_V3_SampleGroupCode_StratumCode,
            CREATE_INDEX_Tree_V3_Species,
            CREATE_INDEX_Tree_V3_StratumCode,
            CREATE_INDEX_Tree_V3_TreeID_CuttingUnitCode_SampleGroupCode_StratumCode,
            CREATE_INDEX_Tree_V3_TreeID_LiveDead,
            CREATE_INDEX_Tree_V3_TreeID_Species,
            CREATE_INDEX_Tree_V3_TreeID_PlotNumber,
            CREATE_TRIGGER_TREE_V3_ONUPDATE,

            CREATE_TABLE_TREEMEASURMENT,
            CREATE_TRIGGER_TREEMEASURMENTS_ONUPDATE,

            CREATE_TABLE_TREEFIELDVALUE,
            CREATE_INDEX_TreeFieldValue_Field,
            CREATE_INDEX_TreeFieldValue_TreeID,

            CREATE_TABLE_LOG_V3,
            CREATE_INDEX_Log_V3_TreeID,
            CREATE_TRIGGER_LOG_V3_ONUPDATE,

            CREATE_TABLE_Stem_V3,
            CREATE_TRIGGER_Stem_V3_ON_UPDATE,

            CREATE_TABLE_TALLYLEDGER,
            CREATE_INDEX_TallyLedger_CuttingUnitCode,
            CREATE_INDEX_TallyLedger_SampleGroupCode_StratumCode,
            CREATE_INDEX_TallyLedger_StratumCode,
            CREATE_INDEX_TallyLedger_TreeID,

            CREATE_TABLE_FIXCNTTALLYCLASS_V3,
            CREATE_INDEX_FixCNTTallyClass_V3_Field,
            CREATE_INDEX_FixCNTTallyClass_V3_StratumCode,
            CREATE_TABLE_FIXCNTTALLYPOPULATION_V3,
            CREATE_INDEX_FixCNTTallyPopulation_V3_Species,
            CREATE_INDEX_FixCNTTallyPopulation_V3_StratumCode,

            CREATE_TABLE_TREEDEFAULTVALUE,
            CREATE_INDEX_TreeDefaultValue_Species,
            CREATE_TRIGGER_TREEDEFAULTVALUE_ONUPDATE,

            //processing tables
            CREATE_TABLE_BIOMASSEQUATION,
            CREATE_TABLE_LCD,
            CREATE_TABLE_LOGMATRIX,
            CREATE_TABLE_LOGSTOCK,
            //CREATE_TRIGGER_LOGSTOCK_ONUPDATE,
            CREATE_TABLE_POP,
            CREATE_TABLE_PRO,
            CREATE_TABLE_QUALITYADJEQUATION,
            CREATE_TABLE_REGRESSION,
            CREATE_TABLE_REPORTS,
            CREATE_TABLE_TREECALCULATEDVALUES,
            CREATE_TABLE_VALUEEQUATION,
            CREATE_TABLE_VOLUMEEQUATION,

            // design tables
            CREATE_TABLE_STRATUMSTATS,
            CREATE_INDEX_StratumStats_Stratum_CN,
            CREATE_TABLE_SAMPLEGROUPSTATS,
            CREATE_TABLE_SAMPLEGROUPSTATSTREEDEFAULTVALUE,
            CREATE_INDEX_SampleGroupStatsTreeDefaultValue_SampleGroupStats_CN,


            //setup tables
            CREATE_TABLE_LOGFIELDSETUPDEFAULT,
            CREATE_TABLE_TREEFIELDSETUPDEFAULT,

            //utility tables
            CREATE_TABLE_GLOBALS,
            CREATE_TABLE_MESSAGELOG,

            //validation
            CREATE_TABLE_LOGGRADEAUDITRULE_V3,
            CREATE_INDEX_LogGradeAuditRule_V3_Species,
            CREATE_INDEX_LogGradeAuditRule_V3_Species_DefectMax_Grade,
            CREATE_TABLE_TREEAUDITRULE,
            CREATE_INDEX_TreeAuditRule_Field,
            CREATE_TABLE_TREEDEFAULTVALUE_TREEAUDITVALUE,
            CREATE_INDEX_TreeDefaultValue_TreeAuditRule_Species,
            CREATE_INDEX_TreeDefaultValue_TreeAuditRule_Species_LiveDead_PrimaryProduct,
            CREATE_INDEX_TreeDefaultValue_TreeAuditRule_TreeAuditRuleID,
            CREATE_TABLE_ERRORLOG_LEGACY,
            CREATE_TABLE_TREEAUDITRESOLUTION,
            CREATE_INDEX_TreeAuditResolution_TreeAuditRuleID,
            CREATE_INDEX_TreeAuditResolution_TreeID,

            //views
            CREATE_VIEW_TALLYPOPULATION,
            CREATE_VIEW_TallyLedger_Totals,
            CREATE_VIEW_TreeFieldValue_TreeMeasurment,
            CREATE_VIEW_TreeFieldValue_TreeMeasurment_Filtered,
            CREATE_VIEW_TREEERROR,
            CREATE_VIEW_LOGGRADEERROR,
            CREATE_VIEW_TREEAUDITERROR,
            CREATE_VIEW_PLOTERROR,
            CREATE_VIEW_TREEFIELDVALUE_ALL,

            // back ports
            CREATE_VIEW_COUNTTREE,
            CREATE_VIEW_CUTTINGUNITSTRATUM,
            CREATE_VIEW_LOG,
            CREATE_VIEW_PLOT,
            CREATE_VIEW_SAMPLEGROUP,
            CREATE_VIEW_SAMPLEGROUPTREEDEFAULTVALUE,
            CREATE_VIEW_TREE,
            CTEATE_TRIGGER_TREE_ONUPDATE,
            CREATE_VIEW_TREEDEFAULTVALUETREEAUDITVALUE,
            CREATE_VIEW_TREEESTIMATE,
            CREATE_VIEW_ERRORLOG,
            CREATE_TRIGGER_ERRORLOG_INSERT,
            CREATE_TRIGGER_ERRORLOG_UPDATE,
            CREATE_TRIGGER_ERRORLOG_DELETE,

            SET_DBVERSION,
        };
    }
}