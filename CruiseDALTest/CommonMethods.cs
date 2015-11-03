using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMSCORM.Tests
{
    public static class CommonMethods
    {
        public static string TEST_FILE_PATH = "Test.cruise";

        public static DAL GetTestDAL()
        {
            return new DAL(TEST_FILE_PATH);
        }

    }
}
