using System;
using System.Collections.Generic;
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
    /// Thrown when creating instace of DataStore where file's SchemaVersion is below the MinimumCompatibleSchemaVersion of the assembly
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

    public class ConstraintException : DatabaseExecutionException
    {
        public ConstraintException(String message, Exception innerEx)
            : base(message, innerEx)
        { }
        public ConstraintException(String message)
            : base(message)
        { }
    }

    public class UniqueConstraintException : ConstraintException
    {
        public UniqueConstraintException(String message, Exception innerEx)
            : base(message, innerEx)
        { }
    }


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

    /// <summary>
    /// Thrown when an exception ocures in a internal mechinism of the DAL
    /// </summary>
    public class ORMException : Exception
    {
        public ORMException(string message, System.Exception innerException)
            : base(message, innerException)
        { }

        public ORMException(string message) : base(message) { }
    }

    public class ReadOnlyException : DatabaseExecutionException
    {
        public ReadOnlyException(string message, System.Exception innerException)
            : base(message, innerException)
        { }

        public ReadOnlyException(string message)
            : base(message)
        { }
    }

    /// <summary>
    /// Thrown when there is an error executing a command on the database
    /// </summary>
#if!Mobile
    public class DatabaseExecutionException : System.Data.Common.DbException
#else
    public class DatabaseExecutionException : Exception
#endif
    {
        public DatabaseExecutionException(string message, System.Exception innerException)
            : base(message + innerException.Message, innerException)
        { }

        public DatabaseExecutionException(string message)
            : base(message)
        { }
    }
}
