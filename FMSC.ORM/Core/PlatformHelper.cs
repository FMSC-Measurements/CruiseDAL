using System;

using System.Collections.Generic;
using System.Text;

namespace FMSC.ORM.Core
{
    public class PlatformHelper
    {
        static PlatformHelper()
        {
            
#if NetCF
            NewLine = "\r\n";
#else
            NewLine = Environment.NewLine;
#endif
        }

        public static readonly string NewLine; 
        

    }
}
