using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace FMSC.ORM
{
    public class FileAccessException : System.IO.IOException
    {
        public FileAccessException(string message, System.Exception innerException)
            : base(message + innerException.Message, innerException)
        { }

        public FileAccessException(string message) : base(message) { }

    }

#if NetCF
    public class SQLException : Exception
#else
    public class SQLException : DbException
#endif
    {
        public SQLException(Exception innerException) : base(null, innerException)
        { }

        public SQLException(string message, Exception innerException)
            : base(message, innerException)
        { }

        public SQLException(DbConnection connection, DbCommand command, Exception innerException)
        {
            AddConnectionInfo(connection);
            AddCommandInfo(command);        
        }

        public void AddConnectionInfo(DbConnection connection)
        {
            if (connection != null)
            {
                try
                {
                    ConnectionString = connection.ConnectionString;
                    ConnectionState = connection.State.ToString();
                }
                catch (ObjectDisposedException)
                {
                    ConnectionState = "Disposed";
                }
            }
        }

        public void AddCommandInfo(DbCommand command)
        {
            if (command != null)
            {
                CommandText = command.CommandText;
            }
        }

        public override string Message
        {
            get
            {
                return String.Format("{0}\r\n: ConnectionString={1}, ConnectionState={2}, CommandText={3}"
                    ,base.Message , ConnectionString, ConnectionState, CommandText);
            }
        }

#if NetCF
        public string ConnectionString { get; set; }
        public string ConnectionState { get; set; }
        public string CommandText { get; set; }
#else
        public string ConnectionString 
        { 
            get { return Data.Contains("ConnectionString") ? Data["ConnectionString"].ToString() : string.Empty; }
            set 
            { 
                if( Data.Contains("ConnectionString"))
                {
                    Data["ConnectionString"] = value;
                }
                else
                {
                    Data.Add("ConnectionString", value);
                }
            }
        }
        public string ConnectionState 
        { 
            get { return Data.Contains("ConnectionState") ? Data["ConnectionState"].ToString() : string.Empty; }
            set 
            { 
                if( Data.Contains("ConnectionState"))
                {
                    Data["ConnectionState"] = value;
                }
                else
                {
                    Data.Add("ConnectionState", value);
                }
            }
        }
        public string CommandText 
        { 
            get { return Data.Contains("CommandText") ? Data["CommandText"].ToString() : string.Empty; }
            set 
            { 
                if( Data.Contains("CommandText"))
                {
                    Data["CommandText"] = value;
                }
                else
                {
                    Data.Add("CommandText", value);
                }
            }
        }
#endif

    }

    public class ConnectionException : SQLException
    {
        public ConnectionException(string message, Exception innerException) : base(message, innerException)
        { }


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

        public SchemaException() : base()
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

    }

    public class SchemaUpdateException : SchemaException
    {
        public SchemaUpdateException(String currentVersion, string targetVersion, Exception innerEx)
            : base(String.Format("Failed updating database from {0} to {1}", currentVersion, targetVersion),
                  innerEx)
        {
            this.CurrentVersion = currentVersion;
            this.TargetVersion = targetVersion;
        }

        string CurrentVersion { get; set; }
        string TargetVersion { get; set; }

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
