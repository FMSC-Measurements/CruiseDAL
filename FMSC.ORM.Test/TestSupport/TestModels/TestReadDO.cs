﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMSC.ORM;
using FMSC.ORM.Core.EntityAttributes;
using FMSC.ORM.Core.EntityModel;

namespace FMSC.ORM.TestSupport.TestModels
{
    [SQLEntity(SourceName = "TestRead")]
    public class TestReadDO : DataObject
    {
        [Field( FieldName="StringTest" )]
        public String StringTest { get; set; }

        [Field( FieldName="Int32Test" )]
        public Int32 Int32Test { get; set; }

        [Field(FieldName = "Int64Test")]
        public Int64 Int64Test { get; set; }

        [Field(FieldName = "FloatTest")]
        public float FloatTest { get; set; }

        [Field(FieldName="DoubleTest")]
        public double DoubleTest { get; set; }

        [Field(FieldName = "DateTimeTest")]
        public DateTime DateTimeTest { get; set; }

        public override void SetValues(DataObject obj)
        {
            throw new NotImplementedException();
        }
    }
}