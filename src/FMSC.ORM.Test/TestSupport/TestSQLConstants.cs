﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMSC.ORM.TestSupport
{
    public static class TestSQLConstants
    {
        public const string MULTI_PROP_TABLE_NAME = "MultiPropTable";

        public static readonly string[] MULTI_PROP_TABLE_FIELDS =
            {
                "ID",
                "StringField",
                "IntField",
                "NIntField",
                "LongField",
                "NLongField",
                "BoolField",
                "NBoolField",
                "FloatField",
                "NFloatField",
                "DoubleField",
                "NDoubleField",
                "GuidField",
                "DateTimeField",
                "PartialyPublicField",
                "PrivateField"
            };

        public static readonly string[] NON_REFLECTED_MULTI_PROP_TABLE_FIELDS =
            {
                "PrivateField",
            };
    }
}