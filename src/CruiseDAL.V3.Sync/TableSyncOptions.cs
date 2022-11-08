using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.V3.Sync
{
    [Flags]
    public enum SyncOption { Lock = 0, Insert = 1, Update = 2, Delete = 4, Force = 8,
        InsertUpdate = Insert | Update, InsertUpdateDelete = Insert | Update | Delete,
        ForceInsert = Insert | Force, ForceUpdate = Update | Force, ForceInsertUpdate = InsertUpdate | Force };



    public class TableSyncOptions
    {
        private static readonly PropertyInfo[] _properties = typeof(TableSyncOptions).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        public TableSyncOptions(SyncOption defaultOption = SyncOption.InsertUpdateDelete)
        {
            Design = defaultOption;
            Validation = defaultOption;
            TreeDataFlags = defaultOption;
            FieldData = defaultOption;
            Sale = defaultOption;
            Cruise = defaultOption;
            CuttingUnit = defaultOption;
            Stratum = defaultOption;
            CuttingUnitStratum = defaultOption;
            SampleGroup = defaultOption;
            Subpopulation = defaultOption;
            Species = defaultOption;
            FixCNTTallyPopulation = defaultOption;
            LogFieldSetup = defaultOption;
            LogFieldHeading = defaultOption;
            TreeFieldSetup = defaultOption;
            TreeFieldHeading = defaultOption;
            TreeAuditRule = defaultOption;
            TreeAuditRuleSelector = defaultOption;
            TreeAuditResolution = defaultOption;
            TreeDefaultValue = defaultOption;
            Template = defaultOption;
            TallyLedger = defaultOption;
            Plot = defaultOption;
            PlotLocation = defaultOption;
            PlotStratum = defaultOption;
            Tree = defaultOption;
            TreeMeasurment = defaultOption;
            TreeLocation = defaultOption;
            TreeFieldValue = defaultOption;
            Log = defaultOption;
            Stem = defaultOption;
            Device = defaultOption;
            SamplerState = defaultOption;
            Processing = defaultOption;
            LogAllOrNone = false;
            StemAllOrNone = false;
            PlotTreeAllOrNone = false;
            NonPlotTreeAllOrNone = false;
        }

        public SyncOption Design { get; set; }

        public SyncOption Validation { get; set; }

        public SyncOption TreeDataFlags { get; set; }

        public SyncOption FieldData { get; set; }


        #region Design
        public SyncOption Sale { get; set; }

        public SyncOption Cruise { get; set; }

        public SyncOption CuttingUnit { get; set; }

        public SyncOption Stratum { get; set; }

        public SyncOption CuttingUnitStratum { get; set; }

        public SyncOption SampleGroup { get; set; }

        public SyncOption Subpopulation { get; set; }

        public SyncOption Species { get; set; }

        public SyncOption FixCNTTallyPopulation { get; set; }

        public SyncOption LogFieldSetup { get; set; }

        public SyncOption LogFieldHeading { get; set; }

        public SyncOption TreeFieldSetup { get; set; }

        public SyncOption TreeFieldHeading { get; set; }

        public SyncOption TreeAuditRule { get; set; }

        public SyncOption TreeAuditRuleSelector { get; set; }

        public SyncOption TreeAuditResolution { get; set; }

        public SyncOption TreeDefaultValue { get; set; }

        public SyncOption Template { get; set; }

        #endregion


        #region FieldData
        public SyncOption TallyLedger { get; set; }
        public SyncOption Plot { get; set; }

        public SyncOption PlotLocation { get; set; }

        public SyncOption PlotStratum { get; set; }

        public SyncOption Tree { get; set; }

        public SyncOption TreeMeasurment { get; set; }

        public SyncOption TreeLocation { get; set; }

        public SyncOption TreeFieldValue { get; set; }

        public SyncOption Log { get; set; }

        public SyncOption Stem { get; set; }

        #endregion

        #region device state

        public SyncOption Device { get; set; }

        public SyncOption SamplerState { get; set; }

        #endregion


        public SyncOption Processing { get; set; }

        public SyncOption CruiseLog { get; set; }

        public bool AllowStratumDesignChanges { get; set; }

        public bool AllowSampleGroupSamplingChanges { get; set; }

        public bool LogAllOrNone { get; set; } = false;

        public bool StemAllOrNone { get; set; } = false;

        public bool PlotTreeAllOrNone { get; set; } = false;

        public bool NonPlotTreeAllOrNone { get; set; } = false;

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach(var p in _properties)
            {
                sb.AppendLine(p.Name + ": " + p.GetValue(this).ToString());
            }

            return sb.ToString();
        }
    }
}
