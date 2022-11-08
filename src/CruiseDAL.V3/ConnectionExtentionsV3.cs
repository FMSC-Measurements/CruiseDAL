using FMSC.ORM.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL
{
    public static class ConnectionExtentionsV3
    {
        public static void LogCruiseLog(this DbConnection @this, string cruiseID, string message, string level,
            string cuttingUnitID = null, string stratumID = null, string sampleGroupID = null,
            string plotID = null, string treeID = null, string logID = null, string tallyLedgerID = null, 
            IExceptionProcessor exceptionProcessor = null)
        {
            var cruiseLog = new
            {
                CruiseLogID = Guid.NewGuid().ToString(),
                CruiseID = cruiseID ?? throw new ArgumentNullException(nameof(cruiseID)),
                Message = message ?? throw new ArgumentNullException(nameof(message)),
                Program = CruiseDatastore.GetCallingProgram(),
                Level = level ?? throw new ArgumentNullException(level),

                CuttingUnitID = cuttingUnitID,
                StratumID = stratumID,
                SampleGroupID = sampleGroupID,
                PlotID = plotID,
                TallyLedgerID = tallyLedgerID,
                TreeID = treeID,
                LogID = logID,
            };

            @this.ExecuteNonQuery2(
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
)", cruiseLog, exceptionProcessor: exceptionProcessor);
        }
    }
}
