using System;
using System.Collections.Generic;
using System.Text;

namespace CruiseDAL.Enums
{
    [Flags]
    public enum TallyMode { Unknown = 0, None = 1, BySpecies = 2, BySampleGroup = 4, Locked = 8, Manual = 16 }

    public enum SampleSelectorType { Unknown = 0, Systematic, Block, SRS, ThreeP };
}
