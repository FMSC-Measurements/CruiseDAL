﻿using CruiseDAL.DataObjects;
using CruiseDAL.V2.Test;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V2.DataObjects
{
    public class TreeFieldSetupDO_Test : TestBase
    {
        public TreeFieldSetupDO_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void QueryTest()
        {
            using (var database = new DAL())
            {
                database.Insert(new StratumDO() { Code = "01", Method = "something" });
                database.Insert(new TreeFieldSetupDO() { Field = "something", FieldOrder = 1, Stratum_CN = 1 });

                database.From<TreeFieldSetupDefaultDO>().Query();
            }
        }
    }
}