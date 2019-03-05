using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.Common
{
    public partial class DDL
    {
        public const string CREATE_TABLE_FIXCNTTALLYPOPULATION =
            "CREATE TABLE FixCNTTallyPopulation( " +
                "FixCNTTallyPopulation_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "FixCNTTallyClass_CN INTEGER REFERENCES FixCNTTallyClass NOT NULL, " +
                "SampleGroup_CN INTEGER REFERENCES SampleGroup NOT NULL, " +
                "TreeDefaultValue_CN INTEGER REFERENCES TreeDefaultValue NOT NULL, " +
                "IntervalSize INTEGER Default 0, " +
                "Min INTEGER Default 0, " +
                "Max INTEGER Default 0" +
            ");"; 
    }
}
