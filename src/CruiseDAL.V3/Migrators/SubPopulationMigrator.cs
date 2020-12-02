using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public class SubPopulationMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID)
        {
            return
$@"INSERT INTO {toDbName}.Subpopulation (
        SubpopulationID,
        CruiseID,
        StratumCode,
        SampleGroupCode,
        SpeciesCode,
        LiveDead
    )
    SELECT 
        (hex( randomblob(4)) || '-' || hex( randomblob(2)) 
                || '-' || '4' || substr(hex(randomblob(2)), 2) || '-'
                || substr('AB89', 1 + (abs(random()) % 4), 1) ||
                substr(hex(randomblob(2)), 2) || '-' || hex(randomblob(6))) AS SubpupulationID,
        *
        FROM (
            SELECT DISTINCT
                '{cruiseID}',
                st.Code,
                sg.Code,
                tdv.Species,
                tdv.LiveDead
            FROM {fromDbName}.SampleGroupTreeDefaultValue as sgtdv
            JOIN {fromDbName}.SampleGroup AS sg USING (SampleGroup_CN)
            JOIN {fromDbName}.Stratum AS st USING (Stratum_CN)
            JOIN {fromDbName}.TreeDefaultValue AS tdv USING (TreeDefaultValue_CN)
        );";
        }
    }
}
