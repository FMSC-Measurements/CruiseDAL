using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FMSC.ORM.ModelGenerator.Test
{
    public class Program_test
    {
        [Fact]
        public void CreateDatastore_test()
        {
            var datastore = Program.CreateDatastore("FMSC.ORM.dll", "FMSC.ORM.SQLite.SQLiteDatastore", (string[])null);
            datastore.Should().NotBeNull();
        }
    }
}
