using CruiseDAL.DataObjects;
using Xunit;
using CruiseDAL;
using System.Collections.Generic;
using FluentAssertions;

namespace CruiseDAL.DataObjects.Tests
{
    public class LogMatrixDOTest
    {
        ///// <summary>
        /////A test for GetByRowNumberGroupBySpecies
        /////</summary>
        //[Fact]
        //public void GetByRowNumberGroupBySpeciesTest()
        //{
        //    using (var ds = new DAL("TestResources/Test.cruise"))
        //    {
        //        string reportNum = "008";

        //        var groupedCollection = LogMatrixDO.GetByRowNumberGroupBySpecies(ds, reportNum);

        //        groupedCollection.Should().NotBeNull();//actual should allways be not null, but may be empty

        //        foreach (string species in groupedCollection.Keys)
        //        {
        //            foreach (LogMatrixDO lm in groupedCollection[species])
        //            {
        //                lm.GradeDescription.Should().NotBeNullOrWhiteSpace();
        //                //TestContext.WriteLine(lm.GradeDescription);
        //            }
        //        }
        //    }
        //}
    }
}
