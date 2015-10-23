using CruiseDAL;
using System;
using CruiseDAL.DataObjects;
using Xunit;

namespace CruiseDAL.BaseDAL.Tests
{
    public class ObjectCacheTest
    {

        private static DAL _DAL { get; set; }
        private static DataObjectFactory _DOFactory { get; set; }

        public ObjectCacheTest()
        {
            _DAL = new DAL("ObjectCacheTest.cruise", true);
            _DOFactory = new DataObjectFactory(_DAL);
        }
        
 

        public void CountTest()
        {
            DataObjectFactory DOFactory = new DataObjectFactory(_DAL); 
            ObjectCache target = new ObjectCache(DOFactory);

            target.GetByID<SaleDO>(1);

            Assert.Equal(1, target.Count);

            var tempSale = new SaleDO { rowID = 2 };
            target.Add( tempSale, ObjectCache.AddBehavior.THROWEXCEPTION);

            Assert.Equal(2, target.Count);

            target.Remove(tempSale);

            Assert.Equal(1, target.Count);

            //target.Clear();

            //Assert.AreEqual(0, target.Count);

        }


        public void GetByIDTest()
        {
            ObjectCache target = new ObjectCache(_DOFactory);

            Assert.Null( target.GetByID(1));

            //GetByID with a type argument should return an object and added it to the cache
            SaleDO tempSale = target.GetByID<SaleDO>(1) as SaleDO;
            Assert.NotNull(tempSale);


            Assert.NotNull(target.GetByID(1));
        }


        public void AddTest()
        {
            ObjectCache target = new ObjectCache(_DOFactory);

            var tempSale = new SaleDO { rowID = 1 };

            Assert.Equal(true, target.Add(tempSale, ObjectCache.AddBehavior.DONT_OVERWRITE));
            tempSale = new SaleDO { rowID = 1 };
            Assert.Equal(false, target.Add(tempSale, ObjectCache.AddBehavior.DONT_OVERWRITE));
            tempSale = new SaleDO { rowID = 1 };
            Assert.Equal(true, target.Add(tempSale, ObjectCache.AddBehavior.OVERWRITE));
            tempSale = new SaleDO { rowID = 1 };
            try
            {
                target.Add(tempSale, ObjectCache.AddBehavior.THROWEXCEPTION);
                Assert.True(false);
            }
            catch (Exception e)
            {
                //TODO proper exception type not defined, for AddBehavior.THROW_EXCEPTION 
            }
        }

        /// <summary>
        ///A test for ObjectCache Constructor
        ///</summary>
        public void ObjectCacheConstructorTest()
        {
            ObjectCache target = new ObjectCache(_DOFactory);
            Assert.NotNull(target);
        }
    }
}
