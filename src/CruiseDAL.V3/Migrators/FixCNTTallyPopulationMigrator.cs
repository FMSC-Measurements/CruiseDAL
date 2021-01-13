using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public class FixCNTTallyPopulationMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID)
        {
            return
$@"INSERT INTO {toDbName}.FixCNTTallyPopulation (
        FixCNTTallyPopulation_CN,
        CruiseID,
        StratumCode,
        SampleGroupCode,
        SpeciesCode,
        LiveDead,
        IntervalSize,
        Min,
        Max
    )
    SELECT
        fixTP.FixCNTTallyPopulation_CN,
        '{cruiseID}',
        st.Code AS StratumCode,
        sg.Code AS SampleGroupCode,
        tdv.Species,
        tdv.LiveDead,
        fixTP.IntervalSize,
        fixTP.Min,
        fixTP.Max
    FROM {fromDbName}.FixCNTTallyPopulation fixTP
    JOIN {fromDbName}.SampleGroup AS sg USING (SampleGroup_CN)
    JOIN {fromDbName}.Stratum AS st USING (Stratum_CN)
    JOIN {fromDbName}.TreeDefaultValue AS tdv USING (TreeDefaultValue_CN)
;";
        }
    }
}
