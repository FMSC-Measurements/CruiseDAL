﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.XUnit
{
    public static class AssertEx
    {
        public static void NotNullOrEmpty(string str)
        {
            Xunit.Assert.False(String.IsNullOrEmpty(str));
        }

        public static void NotNullOrWhitespace(string str)
        {
            Xunit.Assert.False(String.IsNullOrWhiteSpace(str));
        }
    }
}