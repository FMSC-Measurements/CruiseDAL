﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.V3
{
    public partial class DDL
    {
        public const string CREATE_TABLE_TREEFIELDSETUP_V3 =
            "CREATE TABLE TreeFieldSetup_V3 ( " +
                "StratumCode TEXT NOT NULL, " +
                "Field TEXT NOT NULL, " +
                "FieldOrder INTEGER Default 0, " +
                "Heading TEXT, " +
                "Width REAL Default 0.0, " +
                "UNIQUE(StratumCode, Field), " +
                "FOREIGN KEY (StratumCode) REFERENCES Stratum (StratumCode) ON DELETE CASCADE ON UPDATE CASCADE" +
            ");";
    }

    public partial class Updater
    {
        public const string INITIALIZE_TREEFIELDSETUP_V3_FROM_TREEFIELDSETUP =
            "INSERT INTO TreeFieldSetup_V3 " +
            "SELECT " +
            "st.Code AS StratumCode, " +
            "Field, " +
            "FieldOrder, " +
            "Heading, " +
            "Width " +
            "FROM TreeFieldSetup " +
            "JOIN Stratum AS st USING (Stratum_CN);";
    }
}