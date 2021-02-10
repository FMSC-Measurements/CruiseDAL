using CruiseDAL.V2.Test;
using FluentAssertions;
using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V2.DataObjects
{
    [Table("FixCNTTallyClass")]
    public class FixCNTTallyClassDTO
    {
        [PrimaryKeyField(Name = "FixCNTTallyClass_CN")]
        public long? FixCNTTallyClass_CN { get; set; }

        // type for FieldName is Integer but value stored is a string
        // cast value to text to tell System.Data.Sqlite to retrive value as string
        [Field(Alias = "FieldNameStr", SQLExpression = "CAST (FieldName as Text)")]
        public string Field { get; set; }

        [Field(Name = "Stratum_CN")]
        public long? Stratum_CN { get; set; }
    }

    public class FixCNTTallyClass_Tests : TestBase
    {
        public FixCNTTallyClass_Tests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void QuertyFixCNTTallyClass_Test()
        {
            var filePath = Path.Combine(TestFilesDirectory, "test_FixCNT.cruise");
            using(var db = new DAL(filePath))
            {
                var result = db.From<FixCNTTallyClassDTO>().Query().ToArray();
                result.Should().NotBeEmpty();

                var firstResult = result.First();
                firstResult.Should().NotBeNull();
            }
        }
    }
}
