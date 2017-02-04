using FMSC.ORM.SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FMSC.ORM.TestSupport
{
    public class TestDBFixture : IDisposable
    {
        public string CreatedDBPath { get; set; } = "CreatedTestDB.db";


        public TestDBFixture()
        {
            File.Delete(CreatedDBPath);
        }

        public void Dispose()
        {

        }

    }
}
