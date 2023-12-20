using CruiseDAL.TestCommon;
using CruiseDAL.TestCommon.V3;
using CruiseDAL.V3.Models;
using Xunit.Abstractions;

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
            templateCopier.CheckIsTableConfigValid(out var errors).Should().BeTrue();

            var cruiseID = init.CruiseID;
            destTemplateDb.Insert(new Sale { SaleID = init.SaleID, SaleNumber = "1234" });
            destTemplateDb.Insert(new Cruise { CruiseID = cruiseID, SaleID = init.SaleID, CruiseNumber = "1234", SaleNumber = "1234" });

            templateCopier.Copy(templateDb, destTemplateDb, cruiseID);
        }

        [Fact]
        public void Copy_WithExisting()
        {
            var destTemplatePath = GetTempFilePath("DestTemplate.crz3t");

            var init = new TemplateDatabaseInitializer();
            using var templateDb = init.CreateDatabase();

            using var destTemplateDb = new CruiseDatastore_V3(destTemplatePath, true);
            var cruiseID = init.CruiseID;
            destTemplateDb.Insert(new Sale { SaleID = init.SaleID, SaleNumber = "1234" });
            destTemplateDb.Insert(new Cruise { CruiseID = cruiseID, SaleID = init.SaleID, CruiseNumber = "1234", SaleNumber = "1234" });

            // add some pre existing records, this is to test copied data isn't creating conflicts on RowID values
            // note some tables have other unique constraints,  
            //destTemplateDb.Insert(new TreeFieldHeading { CruiseID = init.CruiseID, Field = nameof(TreeMeasurment.DBH), Heading = "something" });
            //destTemplateDb.Insert(new LogFieldHeading { CruiseID = init.CruiseID, Field = nameof(Log.Grade), Heading = "something" });
            destTemplateDb.Insert(new StratumTemplate { CruiseID = init.CruiseID, StratumTemplateName = "something" });
            destTemplateDb.Insert(new Species { CruiseID = init.CruiseID, SpeciesCode = "something" });
            destTemplateDb.Insert(new Species_Product { CruiseID = init.CruiseID, SpeciesCode = "something", ContractSpecies = "something" });
            destTemplateDb.Insert(new TreeDefaultValue { CruiseID = init.CruiseID, SpeciesCode = "something", PrimaryProduct = "something" });


            var templateCopier = new TemplateCopier();
            templateCopier.CheckIsTableConfigValid(out var errors).Should().BeTrue();
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