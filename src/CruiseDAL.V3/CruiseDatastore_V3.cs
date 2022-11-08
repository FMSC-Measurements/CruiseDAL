using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using FMSC.ORM.Core;

namespace CruiseDAL
{
    public class CruiseDatastore_V3 : CruiseDatastore
    {
        public CruiseDatastore_V3()
            : this(IN_MEMORY_DB_PATH, false)
        { }

        public CruiseDatastore_V3(string path)
            : this(path, false)
        { }

        public CruiseDatastore_V3(string path, bool makeNew) 
            : base(path, makeNew, new CruiseDatastoreBuilder_V3(), new Updater_V3())
        {
        }

        protected override bool IsExtentionValid(string path)
        {
            var extension = System.IO.Path.GetExtension(path).ToLower();
            return extension == ".crz3" || extension == ".crz3t" || extension == ".crz3db";
        }

#pragma warning disable S1185 // Overriding members should do more than simply call the same member in the base class
        protected override void OnConnectionOpened(DbConnection connection)
        {
            base.OnConnectionOpened(connection);

#if SYSTEM_DATA_SQLITE
            connection.ExecuteNonQuery("PRAGMA foreign_keys=on;", exceptionProcessor: ExceptionProcessor);
#endif
        }
#pragma warning restore S1185 // Overriding members should do more than simply call the same member in the base class

        public void LogCruiseLog(string cruiseID, string message, string level, 
            string cuttingUnitID = null, string stratumID = null, string sampleGroupID = null, 
            string plotID = null, string treeID = null, string logID = null, string tallyLedgerID = null)
        {
            var cruiseLog = new
                {
                    CruiseLogID = Guid.NewGuid().ToString(),
                    CruiseID = cruiseID ?? throw new ArgumentNullException(nameof(cruiseID)),
                    Message = message ?? throw new ArgumentNullException(nameof(message)),
                    Program = GetCallingProgram(),
                    Level = level ?? throw new ArgumentNullException(level),

                    CuttingUnitID = cuttingUnitID,
                    StratumID = stratumID,
                    SampleGroupID = sampleGroupID,
                    PlotID = plotID,
                    TallyLedgerID = tallyLedgerID,
                    TreeID = treeID,
                    LogID = logID,
                };

            Execute2(
@"INSERT INTO CruiseLog (
    CruiseLogID,
    CruiseID,

    Message,
    Program,
    Level,

    CuttingUnitID,
    StratumID,
    SampleGroupID,
    PlotID,
    TallyLedgerID,
    TreeID,
    LogID
) VALUES (
    @CruiseLogID,
    @CruiseID,
    
    @Message,
    @Program,
    @Level, 

    @CuttingUnitID,
    @StratumID,
    @SampleGroupID,
    @PlotID,
    @TallyLedgerID,
    @TreeID,
    @LogID
)", cruiseLog);
        }
    }
}
