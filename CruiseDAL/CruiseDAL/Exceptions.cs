using System;
using System.Collections.Generic;
using System.Text;

namespace CruiseDAL
{
    /// <summary>
    /// Thrown when two processes try to access the same database using the DAL
    /// </summary>
    public class DatabaseShareException : System.IO.IOException
    {
        public DatabaseShareException(string message, System.Exception innerException)
            : base(message, innerException)
        { }

        public DatabaseShareException(string message) : base(message) { }

    }
}
