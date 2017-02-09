using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using CruiseDAL.DataObjects;
using Xunit;
using CruiseDAL;

namespace FMSCORM.Tests
{

    public class MappingCollectionTest
    {
        public MappingCollectionTest()
        {
            testDB = new DAL("MappingCollectionTest" + ".cruise", true);
            Random rand = new Random(0);

            List<CuttingUnitDO> units = new List<CuttingUnitDO>();
            for (int i = 1; i <= 10; i++)
            {
                CuttingUnitDO cu = new CuttingUnitDO(testDB);
                cu.Code = i.ToString();
                cu.Area = rand.Next(9) + 1.0f;
                cu.Save();
                units.Add(cu);
            }

            List<StratumDO> strata = new List<StratumDO>();
            for (int i = 0; i <= 10; i++)
            {
                StratumDO st = new StratumDO(testDB);
                st.Code = i.ToString();
                st.Method = "something";
                st.Save();
                strata.Add(st);
            }

            foreach (CuttingUnitDO cu in units)
            {
                for (int i = 0; i < cu.Area; i++)
                {
                    CuttingUnitStratumDO cust = new CuttingUnitStratumDO(testDB);
                    cust.CuttingUnit_CN = cu.CuttingUnit_CN;
                    cust.Stratum_CN = strata[i].Stratum_CN;
                    cust.Save();
                }
            }

            //testDB.FlushCache();
        }

        private DAL testDB;




        public void MappingCollectionReadTest()
        {
            List<CuttingUnitDO> units = testDB.Read<CuttingUnitDO>(null, null);
            foreach (CuttingUnitDO u in units)
            {
                u.Strata.Populate();
                Assert.True(u.Strata.Count == (int)u.Area);
            }
        }

         public void MappingCollectionIndexTest()
         {             

             List<CuttingUnitDO> units = testDB.Read<CuttingUnitDO>(null, null);
             foreach (CuttingUnitDO u in units)
             {
                 u.Strata.Populate();
                 for (int i = 0; i < u.Strata.Count; i++)
                 {
                     Assert.True(u.Strata[i] != null);
                 }
             }

             int expectedCount = units[0].Strata.Count + 10;
             for (int i = 0; i < 10; i++)
             {
                 units[0].Strata.Add(new StratumDO());
             }
             Assert.True(units[0].Strata.Count == expectedCount);
         }


         public void MappingCollectionPopulateTest()
         {
             List<CuttingUnitDO> units = testDB.Read<CuttingUnitDO>(null, null);
             List<StratumDO> strata = testDB.Read<StratumDO>(null, null);

             foreach (CuttingUnitDO c in units)
             {
                 c.Strata.Populate();
                 foreach (StratumDO s in c.Strata)
                 {
                     Assert.True(strata.Contains(s));
                 }
             }

             foreach (StratumDO s in strata)
             {
                 s.CuttingUnits.Populate();
                 foreach (CuttingUnitDO c in s.CuttingUnits)
                 {
                     Assert.True(units.Contains(c));
                 }
             }

             CuttingUnitDO cu = units[0];
             StratumDO st = strata[0];
             cu.Strata.Populate();
             cu.Strata.Add(st);
             cu.Strata.Populate();
             Assert.True(cu.Strata.Contains(st));

         }
    }
}
