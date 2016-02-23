using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Xunit
{
    public class TestClassBase
    {
        protected readonly ITestOutputHelper _output;
        protected Stopwatch _stopwatch;

        public TestClassBase(ITestOutputHelper output)
        {
            _output = output;
        }

        public void StartTimer()
        {
            _stopwatch = new Stopwatch();
            _output.WriteLine("Stopwatch Started");
            _stopwatch.Start();
        }

        public void EndTimer()
        {
            _stopwatch.Stop();
            _output.WriteLine("Stopwatch Ended:" + _stopwatch.ElapsedMilliseconds.ToString() + "ms");
        }


    }
}
