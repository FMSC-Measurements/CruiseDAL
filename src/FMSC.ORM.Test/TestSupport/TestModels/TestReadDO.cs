using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMSC.ORM.EntityModel.Attributes;
using FMSC.ORM.EntityModel;

namespace FMSC.ORM.TestSupport.TestModels
{
    [EntitySource(SourceName = "TestRead")]
    public class TestReadDO : DataObject_Base
    {
        [Field( Name="StringTest" )]
        public String StringTest { get; set; }

        [Field( Name="Int32Test" )]
        public Int32 Int32Test { get; set; }

        [Field(Name = "Int64Test")]
        public Int64 Int64Test { get; set; }

        [Field(Name = "FloatTest")]
        public float FloatTest { get; set; }

        [Field(Name="DoubleTest")]
        public double DoubleTest { get; set; }

        [Field(Name = "DateTimeTest")]
        public DateTime DateTimeTest { get; set; }

    }
}
