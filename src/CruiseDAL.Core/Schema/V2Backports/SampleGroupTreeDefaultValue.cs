using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.V2Backports
{
    public partial class DDL
    {
        public const string CREATE_VIEW_SAMPLEGROUPTREEDEFAULTVALUE =
            "CREATE VIEW SampleGroupTreeDefaultValue " +
            "(SampleGroup_CN, StratumCode, SampleGroupCode, TreeDefaultValue_CN, PrimaryProduct, FiaCode, LiveDead) AS " +
            "SELECT sg.SampleGroup_CN, sgsp.StratumCode, sgsp.SampleGroupCode, tdv.TreeDefaultValue_CN, sg.PrimaryProduct, sgsp.FiaCode, fiasp.Species, sgsp.LiveDead " +
            "FROM SampleGroup_Species AS sgsp " +
            "JOIN FiaCode_Species as fiasp USING (FiaCode)" +
            "JOIN SampleGroup_V3 AS sg USING (SampleGroupCode, StratumCode) " +
            "JOIN TreeDefaultValue_V3 AS tdv ON sg.PrimaryProduct = tdv.PrimaryProduct AND sgsp.FiaCode = tdv.FiaCode AND sgsp.LiveDead = tdv.LiveDead " +
            ";";


    }
}
