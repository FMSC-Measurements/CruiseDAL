using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.MyXUnit
{
    public static class AssertEx
    {
        public static void NotNullOrEmpty(string str, string message = null)
        {
            Xunit.Assert.False(String.IsNullOrEmpty(str), message);
        }

        public static void NotNullOrWhitespace(string str, string message = null)
        {
            Xunit.Assert.False(String.IsNullOrWhiteSpace(str), message);
        }

        public static void Fail(String message = null)
        {
            Xunit.Assert.True(false, message);
        }
    }
}
