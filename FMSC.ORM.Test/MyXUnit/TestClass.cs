using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace FMSC.ORM.MyXUnit
{
    public class TestClassBase
    {
        protected readonly ITestOutputHelper _output;

        public TestClassBase(ITestOutputHelper output)
        {
            _output = output;
        }


    }
}
