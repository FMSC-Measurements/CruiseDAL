using CruiseDAL.DownMigrators;
using FMSC.ORM.Core;
using System.Collections.Generic;
using System.Data.Common;

namespace CruiseDAL
{
    public class DownMigrator
    {
        public static readonly IEnumerable<IDownMigrator> DOWN_MIGRATORS = new IDownMigrator[]
        {
            new SaleDownMigrate(),
            new CuttingUnitDownMigrate(),
            new StratumDownMigrate(),
            new CuttingUnit_StratumDownMigrator(),
            new SampleGroupDownMigrate(),
            new TreeDefaultValueDownMigrate(),
            new SampleGroupTreeDefaultValueDownMigrate(),
            new PlotDownMigrator(),
            new TallyDownMigrator(),
            new TreeDownMigrate(),
            new LogDownMigrator(),
            new CountTreeDownMigrator(),
            new ErrorLogDownMigrator(),
            new VolumeEquationDownMigrator(),
            new ReportsDownMigrator(),
        };

        public IEnumerable<IDownMigrator> Migrators { get; }

        public DownMigrator() : this(DOWN_MIGRATORS)
        { }

        public DownMigrator(IEnumerable<IDownMigrator> migrators)
        {
            Migrators = migrators ?? throw new System.ArgumentNullException(nameof(migrators));
        }

        public void MigrateFromV3ToV2(string cruiseID, CruiseDatastore_V3 v3db, CruiseDatastore v2db, string createdBy = null)
        {
            var v3DbAlias = "v3";
            v2db.AttachDB(v3db, v3DbAlias);

            try
            {
                var connection = v2db.OpenConnection();
                MigrateFromV3ToV2(cruiseID, connection, createdBy, v3DbAlias, v2db.ExceptionProcessor, Migrators);
            }
            finally
            {
                v3db.DetachDB(v3DbAlias);
                v2db.ReleaseConnection();
            }
        }

        public static void MigrateFromV3ToV2(string cruiseID, DbConnection connection, string createdBy, string from = "v3", IExceptionProcessor exceptionProcessor = null, IEnumerable<IDownMigrator> migrators = null)
        {
            var to = "main";

            DbTransaction transaction = null;

            migrators ??= DOWN_MIGRATORS;
            foreach (var migrator in migrators)
            {
                var command = migrator.CreateCommand(to, from, cruiseID, createdBy);
                connection.ExecuteNonQuery(command, transaction: transaction, exceptionProcessor: exceptionProcessor);
            }
        }

        public bool EnsureCanMigrate(string cruiseID, CruiseDatastore_V3 v3db, out string errorMsg)
        {
            errorMsg = null;

            if (CheckHasTreeDefaults(cruiseID, v3db) is false)
            {
                return true;
            }
            else
            {
                errorMsg += "Cruise Has No Tree Default Values";
                return false;
            }
        }

        private static bool CheckHasTreeDefaults(string cruiseID, CruiseDatastore_V3 v3db)
        {
            var hasTrees = v3db.GetRowCount("Tree", "WHERE CruiseID = @p1", cruiseID) > 0;
            var hasTallyBySp = v3db.GetRowCount("TallyPopulation", "WHERE CruiseID = @p1 AND SpeciesCode IS NOT NULL", cruiseID) > 0;

            if (hasTrees || hasTallyBySp)
            {
                var hasTDV = v3db.GetRowCount("TreeDefaultValue", "WHERE CruiseID = @p1", cruiseID) > 0;
                return hasTDV is false;
            }
            else { return true; }
        }
    }
}