using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace CruiseDAL
{
    public class FileAccessException : System.IO.IOException
    {
        public FileAccessException(string message, System.Exception innerException)
            : base(message + innerException.Message, innerException)
        { }

        public FileAccessException(string message) : base(message) { }

    }


    public class SQLException : DbException
    {
        public SQLException(string message, Exception innerException) : base(message, innerException)
        { }

    }

    public class ConnectionException : SQLException
    {
        public ConnectionException(string message, Exception innerException) : base(message, innerException)
        { }

        public String ConnectionString { get; set; }
        public System.Data.ConnectionState ConnectionState { get; set; }
    }

    public class ReadOnlyException : ConnectionException
    {
        public ReadOnlyException(string message, System.Exception innerException)
            : base(message, innerException)
        { }
    }

    public class ConstraintException : SQLException
    {
        public ConstraintException(String message, Exception innerEx)
            : base(message, innerEx)
        { }

        public String FieldName { get; set; }
    }

    public class UniqueConstraintException : ConstraintException
    {
        public UniqueConstraintException(String message, Exception innerEx)
            : base(message, innerEx)
        { }
    }

    /// <summary>
    /// base exception for schema errors
    /// </summary>
    public class SchemaException : System.Exception
    {
        public SchemaException(String message, System.Exception innerEx)
            : base(message, innerEx)
        { }

        public SchemaException(String message)
            : base(message)
        { }
    }

    /// <summary>
    /// Thrown when creating instance of DataStore where file's SchemaVersion is below the MinimumCompatibleSchemaVersion of the assembly
    /// </summary>
    public class IncompatibleSchemaException : SchemaException
    {
        public IncompatibleSchemaException(String message, System.Exception innerEx)
            : base(message, innerEx)
        { }

        public IncompatibleSchemaException(String message)
            : base(message)
        { }
    }

    /// <summary>
    /// Thrown when an exception occurs in a internal mechanism of the DAL
    /// </summary>
    public class ORMException : Exception
    {
        public ORMException(string message, System.Exception innerException)
            : base(message, innerException)
        { }

        public ORMException(string message) : base(message) { }
    }       
}
