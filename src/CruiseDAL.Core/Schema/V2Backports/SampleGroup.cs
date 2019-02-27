using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.V2Backports
{
    public partial class DDL
    {
        public const string CREATE_VIEW_SAMPLEGROUP =
            "CREATE VIEW SampleGroup AS " +
            "SELECT " +
            "SampleGroup_CN, " +
            "SampleGroupCode AS Code, " +
            "SampleGroupCode, " +
            "st.Stratum_CN, " +
            "StratumCode, " +
            "CutLeave, " +
            "UOM, " +
            "PrimaryProduct, " +
            "SecondaryProduct, " +
            "BiomassProduct, " +
            "DefaultLiveDead, " +
            "SamplingFrequency, " +
            "InsuranceFrequency, " +
            "KZ, " +
            "BigBAF, " +
            "0 AS TallyBySubPop, " +
            "TallyMethod, " +
            "Description, " +
            "SampleSelectorType, " +
            "SampleSelectorState, " +
            "MinKPI, " +
            "MaxKPI, " +
            "CreatedBy, " +
            "CreatedDate, " +
            "ModifiedBy, " +
            "ModifiedDate, " +
            "RowVersion, " +
            "SmallFPS " +
            "FROM SampleGroup_V3 " +
            "JOIN Stratum ON StratumCode = Stratum.Code;";
    }
}
