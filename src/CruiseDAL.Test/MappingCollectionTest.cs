using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using CruiseDAL.DataObjects;
using Xunit;
using CruiseDAL;
using FluentAssertions;

namespace CruiseDAL.MappingCollections
{
    public class MappingCollectionTest
    {
        DAL MakeTestDB()
        {
            var testDB = new DAL();
            Random rand = new Random(0);

            List<CuttingUnitDO> units = new List<CuttingUnitDO>();
            for (int i = 1; i <= 10; i++)
            {
                CuttingUnitDO cu = new CuttingUnitDO(testDB)
                {
                    Code = i.ToString(),
                    Area = rand.Next(9) + 1.0f
                };
                cu.Save();
                units.Add(cu);
            }

            List<StratumDO> strata = new List<StratumDO>();
            for (int i = 0; i <= 10; i++)
            {
                StratumDO st = new StratumDO(testDB)
                {
                    Code = i.ToString(),
                    Method = "something"
                };
                st.Save();
                strata.Add(st);
            }

            foreach (CuttingUnitDO cu in units)
            {
                for (int i = 0; i < cu.Area; i++)
                {
                    CuttingUnitStratumDO cust = new CuttingUnitStratumDO(testDB)
                    {
                        CuttingUnit_CN = cu.CuttingUnit_CN,
                        Stratum_CN = strata[i].Stratum_CN
                    };
                    cust.Save();
                }
            }

            return testDB;
        }

        [Fact]
        public void MappingCollectionReadTest()
        {
            using (var testDB = MakeTestDB())
            {
                var units = testDB.From<CuttingUnitDO>().Read();
                foreach (CuttingUnitDO u in units)
                {
                    u.Strata.Populate();

                    u.Strata.Count.Should().Be((int)u.Area);
                }
            }
        }

        //public void MappingCollectionIndexTest()
        //{
        //   using (var testDB = MakeTestDB())
        //   {
        //       var units = testDB.From<CuttingUnitDO>().Read();
        //       foreach (CuttingUnitDO u in units)
        //       {
        //           u.Strata.Populate();
        //           for (int i = 0; i < u.Strata.Count; i++)
        //           {
        //               Assert.True(u.Strata[i] != null);
        //           }
        //       }

        //       int expectedCount = units[0].Strata.Count + 10;
        //       for (int i = 0; i < 10; i++)
        //       {
        //           units[0].Strata.Add(new StratumDO());
        //       }
        //       Assert.True(units[0].Strata.Count == expectedCount);
        //   }
        //}

        [Fact]
        public void MappingCollectionPopulateTest()
        {
            using (var testDB = MakeTestDB())
            {
                var units = testDB.From<CuttingUnitDO>().Read();
                var strata = testDB.From<StratumDO>().Read();

                foreach (CuttingUnitDO c in units)
                {
                    c.Strata.Populate();
                    foreach (StratumDO s in c.Strata)
                    {
                        strata.Should().Contain(s);
                    }
                }

                foreach (StratumDO s in strata)
                {
                    s.CuttingUnits.Populate();
                    foreach (CuttingUnitDO c in s.CuttingUnits)
                    {
                        units.Should().Contain(c);
                    }
                }

                CuttingUnitDO cu = units.First();
                StratumDO st = strata.First();
                cu.Strata.Populate();
                cu.Strata.Add(st);
                cu.Strata.Populate();
                cu.Strata.Should().Contain(st);
            }
        }
    }
}