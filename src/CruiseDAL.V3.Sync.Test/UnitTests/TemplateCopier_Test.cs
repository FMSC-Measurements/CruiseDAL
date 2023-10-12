using CruiseDAL.TestCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using FMSC.ORM.Core;
using CruiseDAL.V3.Models;
using CruiseDAL.TestCommon.V3;

namespace CruiseDAL.V3.Sync.Test.UnitTests
{
    public class TemplateCopier_Test : TestBase
    {
        public TemplateCopier_Test(ITestOutputHelper output) : base(output)
        {
        }

        //[Fact]
        //public void Copy()
        //{
        //    var templateDbPath = GetTestFile("TestTemplate.crz3t");
        //    var destTemplatePath = GetTempFilePath("DestTemplate.crz3t");

        //    using var destTemplateDb = new CruiseDatastore_V3(destTemplatePath, true);
        //    using var templateDb = new CruiseDatastore_V3(templateDbPath);

        //    var templateCopier = new TemplateCopier();

        //    using var templateConn = templateDb.OpenConnection();
        //    using var destTemplateConn = destTemplateDb.OpenConnection();

        //    var cruiseID = templateConn.From<Cruise>().Query().Single().CruiseID;

        //    var saleID = Guid.NewGuid().ToString();
        //    destTemplateConn.Insert(new Sale { SaleNumber = "1234", SaleID = saleID });
        //    destTemplateConn.Insert(new Cruise { CruiseID = cruiseID, SaleID = saleID, CruiseNumber = "1234", SaleNumber = "1234" });

        //    templateCopier.Copy(templateDb, destTemplateDb, cruiseID);
        //}

        [Fact]
        public void Copy()
        {
            var destTemplatePath = GetTempFilePath("DestTemplate.crz3t");

            var init = new TemplateDatabaseInitializer();

            using var destTemplateDb = new CruiseDatastore_V3(destTemplatePath, true);
            using var templateDb = init.CreateDatabase();

            var templateCopier = new TemplateCopier();

            var cruiseID = init.CruiseID;
            destTemplateDb.Insert(new Sale { SaleID = init.SaleID, SaleNumber = "1234" });
            destTemplateDb.Insert(new Cruise { CruiseID = cruiseID, SaleID = init.SaleID, CruiseNumber = "1234", SaleNumber = "1234" });

            templateCopier.Copy(templateDb, destTemplateDb, cruiseID);
        }

        [Fact]
        public void Copy_differentDestCruiseID()
        {
            var destTemplatePath = GetTempFilePath("DestTemplate.crz3t");

            var init = new TemplateDatabaseInitializer();

            using var destTemplateDb = new CruiseDatastore_V3(destTemplatePath, true);
            using var templateDb = init.CreateDatabase();

            var templateCopier = new TemplateCopier();

            var destCruiseID = Guid.NewGuid().ToString();
            var destSaleID = Guid.NewGuid().ToString();
            destTemplateDb.Insert(new Sale { SaleID = destSaleID, SaleNumber = "1234" });
            destTemplateDb.Insert(new Cruise { CruiseID = destCruiseID, SaleID = destSaleID, CruiseNumber = "1234", SaleNumber = "1234" });

            templateCopier.Copy(templateDb, destTemplateDb, init.CruiseID, destCruiseID);
        }
    }
}
