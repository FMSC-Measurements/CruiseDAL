using CruiseDAL.Schema;
using CruiseDAL.Update;
using FMSC.ORM;
using FMSC.ORM.Core;
using FMSC.ORM.Logging;
using System;
using System.Data.Common;
using System.Linq;

namespace CruiseDAL
{
    public partial class Updater_V3 : IUpdater
    {
        public static readonly IDbUpdate[] UPDATES = new IDbUpdate[]
        {
            new UpdateTo_3_3_1(),
            new UpdateTo_3_3_2(),
            new UpdateTo_3_3_3(),
            new UpdateTo_3_3_4(),
            new UpdateTo_3_4_0(),
            new UpdateTo_3_4_1(),
            new UpdateTo_3_4_2(),
            new UpdateTo_3_4_3(),
            new UpdateTo_3_4_4(),
            new UpdateTo_3_5_0(),
            new UpdateTo_3_5_1(),
            new UpdateTo_3_5_2(),
            new UpdateTo_3_5_3(),
            new UpdateTo_3_5_4(),
        };

        public static ILogger Logger { get; set; } = LoggerProvider.Get();

        public void Update(DbConnection connection, IExceptionProcessor exceptionProcessor)
        {
            // grab initial database version
            var dbVersion = GetDbVersion(connection);

            try
            {
                var v = new Version(dbVersion);
                if (v.Major == 3 && v.Minor < 4)
                {
                    connection.ExecuteNonQuery("DROP TRIGGER TreeLocation_OnUpdate;");
                    connection.ExecuteNonQuery(TreeLocationTableDefinition.CREATE_TRIGGER_TreeLocation_ONUPDATE);
                }
            }
            // handle exception thrown when parsing version code
            catch (ArgumentException ex)
            {
                throw new SchemaUpdateException(dbVersion, "TreeLocation_OnUpdate Patch", ex);
            }

#pragma warning disable S3267 // Loops should be simplified with "LINQ" expressions
            foreach (var update in UPDATES)
#pragma warning restore S3267 // Loops should be simplified with "LINQ" expressions
            {
                if (update.SourceVersions.Any(x => x == dbVersion))
                {
                    update.Update(connection, exceptionProcessor);
                    dbVersion = GetDbVersion(connection);
                }
            }
        }

        private string GetDbVersion(DbConnection connection)
        {
            var dbVersion = ReadDatabaseVersion(connection);
            if (dbVersion == null) { throw new SchemaException("Invalid Database Version Value: value was null"); }
            if (dbVersion == "") { throw new SchemaException("Invalid Database Version Value: value was empty"); }
            return dbVersion;
        }

        public static string ReadDatabaseVersion(DbConnection conn)
        {
            try
            {
                return conn.ReadGlobalValue("Database", "Version", transaction: null);
            }
            catch
            {
                return "";
            }
        }

        public bool HasUpdate(DbConnection connection)
        {
            var dbVersion = GetDbVersion(connection);

            return UPDATES
                .SelectMany(x => x.SourceVersions)
                .Any(x => x == dbVersion);
        }

        public void Update(CruiseDatastore db)
        {
            var conn = db.OpenConnection(); // make sure connection stays open during update process

            try
            {
                Update(conn, db.ExceptionProcessor);
            }
            finally
            {
                db.ReleaseConnection();
            }
        }
    }
}