﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.Common
{
    public partial class DDL
    {
        public const string CREATE_TABLE_SAMPLEGROUPSTATSTREEDEFAULTVALUE =
            "CREATE TABLE SampleGroupStatsTreeDefaultValue( " +
                "TreeDefaultValue_CN INTEGER NOT NULL, " +
                "SampleGroupStats_CN INTEGER NOT NULL, " +
                "UNIQUE (TreeDefaultValue_CN, SampleGroupStats_CN)," +
                "FOREIGN KEY (TreeDefaultValue_CN) REFERENCES TreeDefaultValue (TreeDefaultValue_CN) ON DELETE CASCADE," +
                "FOREIGN KEY (SampleGroupStats_CN) REFERENCES SampleGroupStats (SampleGroupStats_CN) ON DELETE CASCADE" +
            ");";
    }
}