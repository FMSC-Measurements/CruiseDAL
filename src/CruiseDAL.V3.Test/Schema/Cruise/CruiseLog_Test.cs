using CruiseDAL.TestCommon;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Test.Schema.Cruise
{
    public class CruiseLog_Test : TestBase
    {
        public CruiseLog_Test(ITestOutputHelper output) : base(output)
        {
        }


        [Fact]
        public void Insert_GenerateDefault_CruiseLogID()
        {
            var init = new DatabaseInitializer();
            using var db = init.CreateDatabase();

            foreach (var i in Enumerable.Range(0, 10))
            {
                db.Execute("INSERT INTO CruiseLog (CruiseID, Message) VALUES (@p1, @p2)", init.CruiseID, "Hi");
            }

            var cruiseLogIDs = db.QueryScalar<string>("SELECT CruiseLogID FROM CruiseLog;");
            var cruiseLogIDSet = new HashSet<string>();
            foreach(var id in cruiseLogIDs)
            {
                var guid = new Guid(id).ToString();
                id.Should().Be(guid.ToUpperInvariant());
                Output.WriteLine(guid.ToString() + "|" + id);
                cruiseLogIDSet.Contains(guid).Should().BeFalse();
                cruiseLogIDSet.Add(guid);
            }
            
        }
    }
}
