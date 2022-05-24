using CruiseDAL.Schema;
using CruiseDAL.Schema.Cruise.Lookup;
using CruiseDAL.Schema.Views;
using FMSC.ORM.Core;
using FMSC.ORM.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace CruiseDAL
{
    public class CruiseDatastoreBuilder_V3 : IDatastoreBuilder
    {
        public static readonly Version DATABASE_VERSION = new Version("3.5.2");

        public static readonly IEnumerable<ITableDefinition> TABLE_DEFINITIONS =
        new ITableDefinition[]
        {
            new DeviceTableDefinition(),

            // lookup
            new FIATableDefinition(),
            new TreeFieldTableDefinition(),
            new LogFieldTableDefinition(),
            new LK_CruiseMethod(),
            new LK_LoggingMethod(),
            new LK_Product(),
            new LK_Region(),
            new LK_Forest(),
            new LK_District(),
            new LK_UOM(),
            new LK_Purpose(),
            new LK_TallyEntryType(),

            // cruise specific lookup
            new SpeciesTableDefinition(),
            new Species_ProductTableDefinition(),
            new TreeDefaultValueTableDefinition(),

            // design
            new SaleTableDefinition(),
            new CruiseTableDefinition_3_5_3(),

            new CuttingUnitTableDefinition(),
            new StratumTableDefinition(),
            new CuttingUnit_StratumTableDefinition(),
            new PlotTableDefinition(),
            new Plot_StratumTableDefinition_3_4_1(),
            new PlotLocationTableDefinition(),
            new SampleGroupTableDefinition(),

            new TreeFieldSetupTableDefinition(),
            new TreeFieldHeading(),
            new LogFieldHeading(),
            new LogFieldSetupTableDefinition(),

            new SamplerStateTableDefinition(),

            new SubPopulationTableDefinition(),
            new TallyDescriptionTableDefinition(),
            new TallyHotkeyTableDefinition(),
            new FixCNTTallyPopulationTableDefinition(),

            // field data
            new TreeTableDefinition(),
            new TreeMeasurmentsTableDefinition(),
            new TreeLocationTableDefinition(),
            new TreeFieldValueTableDefinition(),
            new LogTableDefinition(),
            new StemTableDefinition(),
            new TallyLedgerTableDefinition(),

            // validation
            new TreeAuditRuleTableDefinition(),
            new TreeAuditRuleSelectorTableDefinition(),
            new TreeAuditResolutionTableDefinition(),
            new LogGradeAuditRuleTableDefinition(),

            // processing
            new ReportsTableDefinition(),
            new VolumeEquationTableDefinition(),
            new ValueEquationTableDefinition(),
            new BiomassEquationTableDefinition(),

            //template
            new StratumTemplateTableDefinition(),
            new StratumTemplateTreeFieldSetupTableDefinition(),
            new StratumTemplateLogFieldSetupTableDefinition(),
            new SampleGroupDefaultTableDefinition(),
            
            // utility
            new MessageLogTableDefinition(),
            new GlobalsTableDefinition(),
        };

        public static readonly IEnumerable<IViewDefinition> VIEW_DEFINITIONS =
            new IViewDefinition[]
            {
                new TallyPopulationViewDefinition_3_3_2(),
                new TallyLedger_TotalsViewDefinition(),
                new TallyLedger_Plot_TotalsViewDefinition(),
                new TallyLedger_Tree_TotalsViewDefinition(),
                new TreeErrorViewDefinition(),
                new LogGradeErrorViewDefinition(),
                new PlotErrorViewDefinition(),
                new TreeAuditErrorViewDefinition(),
                new TreeFieldValue_AllViewDefinition(),
                new TreeFieldValue_TreeMeasurmentViewDefinition(),
                new TreeFieldValue_TreeMeasurment_FilteredViewDefinition(),
                new StratumDefaultViewDefinition(),
                new TreeFieldSetupDefaultViewDefinition(),
                new LogFieldSetupDefaultViewDefinition(),
                new Tree_TreeDefaultValue(),
                new TreeMeasurment_DefaultResolved(),
            };

        private ILogger Logger { get; } = LoggerProvider.Get();

        public void BuildDatabase(DbConnection connection, DbTransaction transaction, IExceptionProcessor exceptionProcessor = null)
        {
            CreateTables(connection, transaction, exceptionProcessor);
            CreateTriggers(connection, transaction, exceptionProcessor);
            CreateViews(connection, transaction, exceptionProcessor);
        }

        public void CreateViews(DbConnection connection, DbTransaction transaction, IExceptionProcessor exceptionProcessor = null)
        {
            var views = VIEW_DEFINITIONS;
            foreach (var view in views)
            {
                Logger?.Log($"Creating View {view.ViewName}", LogCategory.DbBuilder, LogLevel.Info);

                var createViewCommand = view.CreateView;
                connection.ExecuteNonQuery(createViewCommand, transaction: transaction, exceptionProcessor: exceptionProcessor);
            }
        }

        public void CreateTriggers(DbConnection connection, DbTransaction transaction, IExceptionProcessor exceptionProcessor = null)
        {
            var tables = TABLE_DEFINITIONS;
            foreach (var table in tables)
            {
                Logger?.Log($"Creating Triggers For Table {table.TableName}", LogCategory.DbBuilder, LogLevel.Info);
                var triggers = table.CreateTriggers;
                if (triggers != null)
                {
                    foreach (var trigger in triggers)
                    {
                        connection.ExecuteNonQuery(trigger, transaction: transaction, exceptionProcessor: exceptionProcessor);
                    }
                }
            }
        }

        public void CreateTables(DbConnection connection, DbTransaction transaction, IExceptionProcessor exceptionProcessor = null)
        {
            var tables = TABLE_DEFINITIONS;
            foreach (var table in tables)
            {
                Logger?.Log($"Creating Table {table.TableName}", LogCategory.DbBuilder, LogLevel.Info);

                var createCommand = table.CreateTable;
                connection.ExecuteNonQuery(createCommand, transaction: transaction, exceptionProcessor: exceptionProcessor);

                var createTombstone = table.CreateTombstoneTable;
                if (createTombstone != null)
                {
                    connection.ExecuteNonQuery(createTombstone, transaction: transaction, exceptionProcessor: exceptionProcessor);
                }

                var createIndexes = table.CreateIndexes;
                if (createIndexes != null)
                {
                    connection.ExecuteNonQuery(createIndexes, transaction: transaction, exceptionProcessor: exceptionProcessor);
                }

                var initialize = table.InitializeTable;
                if (initialize != null)
                {
                    connection.ExecuteNonQuery(initialize, transaction: transaction, exceptionProcessor: exceptionProcessor);
                }
            }

            var setDbVersion =
$@"INSERT INTO Globals (Block, Key, Value) VALUES ('Database', 'Version', '{DATABASE_VERSION.ToString()}');
INSERT INTO Globals (Block, Key, Value) VALUES ('Database', 'CreateVersion', '{DATABASE_VERSION.ToString()}');";

            connection.ExecuteNonQuery(setDbVersion, transaction: transaction, exceptionProcessor: exceptionProcessor);
        }
    }
}