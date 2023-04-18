using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public class FixCNTTallyPopulationMigrator : IMigrator
    {
        // in Cruise Manager When a subpopulation is removed
        // the FixCNTTallyPopulation record if there is one 
        // is not deleted. This can create a ForeignKey error
        // when converting FixCNTTallyPopulation records 
        // because in V3 a FixCNTTallyPopulation must have
        // a subpop record. To mitigate this error we join 
        // with the SampleGroupTreeDefaultValue table
        // so that we only convert valid subpopulations. 

        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID, string deviceID)
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
    JOIN {fromDbName}.SampleGroupTreeDefaultValue as sgtdv USING (SampleGroup_CN, TreeDefaultValue_CN)
    JOIN {fromDbName}.SampleGroup AS sg USING (SampleGroup_CN)
    JOIN {fromDbName}.Stratum AS st USING (Stratum_CN)
    JOIN {fromDbName}.TreeDefaultValue AS tdv USING (TreeDefaultValue_CN)
;";
        }
    }
}
