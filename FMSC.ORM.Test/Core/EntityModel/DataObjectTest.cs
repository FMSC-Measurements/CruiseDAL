using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FMSC.ORM.Core.EntityModel
{
    public class DataObjectTest
    {

        [Fact]
        public void RecordStateTest()
        {
            var data = new ConcreateDataObject();
            var pData = data as IPersistanceTracking;

            Assert.NotNull(pData);

            //Assert.True(data.IsDetached);
            Assert.False(data.IsDeleted);
            Assert.False(data.IsPersisted);
            Assert.False(data.HasChanges);
            

            pData.HasChanges = true;
            //Assert.True(data.IsDetached);
            Assert.False(data.IsDeleted);
            Assert.False(data.IsPersisted);
            Assert.True(data.HasChanges);

            pData.IsPersisted = true;
            //Assert.True(data.IsDetached);
            Assert.False(data.IsDeleted);
            Assert.True(data.IsPersisted);
            Assert.False(data.HasChanges);

        }

        [Fact]
        public void IsDeletedTest()
        {
            var data = new ConcreateDataObject();
            var pData = data as IPersistanceTracking;

            Assert.NotNull(pData);

            pData.IsDeleted = true;
            //Assert.True(data.IsDetached);//fails
            Assert.True(data.IsDeleted);
            Assert.False(data.IsPersisted);
            Assert.False(data.HasChanges);

            data = new ConcreateDataObject();
            pData = data as IPersistanceTracking;

            pData.IsPersisted = true;
            pData.IsDeleted = true;
            //Assert.True(data.IsDetached);
            Assert.True(data.IsDeleted);
            Assert.False(data.IsPersisted);
            Assert.False(data.HasChanges);

            data = new ConcreateDataObject();
            pData = data as IPersistanceTracking;

            pData.IsPersisted = true;
            pData.HasChanges = true;
            pData.IsDeleted = true;
            //Assert.True(data.IsDetached);
            Assert.True(data.IsDeleted);
            Assert.False(data.IsPersisted);
            Assert.True(data.HasChanges);
        }

    }

    public class ConcreateDataObject : DataObject
    {
        public override void SetValues(DataObject obj)
        {
            throw new NotImplementedException();
        }
    }
}
