using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FMSC.ORM.Test.SQLite
{
    public class Sqlite_Integration_Test : TestBase
    {
        public Sqlite_Integration_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData(1L, "1", "?1"
#if MICROSOFT_DATA_SQLITE
            , Skip = "not supported my Microsoft.Data.Sqlite"
#endif
            )]//system.data.sqlite works this way

        [InlineData(1L, "?1", "?1"
#if SYSTEM_DATA_SQLITE
            ,Skip ="not supported my System.Data.SQLite" 
#endif
            )]//microsoft.data.sqlite works this way
        [InlineData(1L, "@1", "@1")]
        [InlineData(1L, "@something", "@something")]
        public void Bind_Paramater(object value, string pName, string pExpr)
        {
            using (var connection = DbProvider.CreateConnection())
            {
                connection.ConnectionString = "Data Source=:memory:";

                var command = connection.CreateCommand();
                command.CommandText = $"SELECT {pExpr};";
                var parm = command.CreateParameter();
                parm.ParameterName = pName;
                parm.Value = value;
                command.Parameters.Add(parm);

                connection.Open();

                var result = command.ExecuteScalar();

                Assert.Equal(value, result);
            }
        }

        [Fact]
        public void Echo_value_guid()
        {
            var pName = "@p1";

            var guid = Guid.NewGuid();

            using (var connection = DbProvider.CreateConnection())
            {
                connection.ConnectionString = "Data Source=:memory:";

                var command = connection.CreateCommand();
                command.CommandText = $"SELECT {pName};";
                var parm = command.CreateParameter();
                parm.ParameterName = pName;
                parm.Value = guid;
                command.Parameters.Add(parm);

                connection.Open();

                var result = command.ExecuteScalar();

                result.Should().BeOfType(typeof(Byte[]));

                Assert.Equal(guid.ToByteArray(), result);
            }
        }

        [Fact]
        public void Echo_value_guid_as_str()
        {
            var pName = "@p1";

            var guid = Guid.NewGuid();

            using (var connection = DbProvider.CreateConnection())
            {
                connection.ConnectionString = "Data Source=:memory:";

                var command = connection.CreateCommand();
                command.CommandText = $"SELECT CAST({pName} AS TEXT);";
                var parm = command.CreateParameter();
                parm.ParameterName = pName;
                parm.Value = guid;
                command.Parameters.Add(parm);

                connection.Open();

                var result = command.ExecuteScalar();

                result.Should().BeOfType(typeof(string));

                LogEncoding(connection);

                var guid_str = Encoding.UTF8.GetString(guid.ToByteArray());
                Assert.Equal(guid_str, result);
            }
        }

        [Theory]
        [InlineData("1", true)]
        [InlineData("0", false)]
        [InlineData("1 IS NOT NULL", true)]
        [InlineData("1 IS NULL", false)]
        public void Echo_value_bool(string quoteValue, bool expectedValue)
        {
            using (var connection = DbProvider.CreateConnection())
            {
                connection.ConnectionString = "Data Source=:memory:";

                var command = connection.CreateCommand();
                command.CommandText = $"SELECT {quoteValue};";

                connection.Open();

                var reader = command.ExecuteReader();
                reader.Read();

                var result = reader.GetBoolean(0);
                result.Should().Be(expectedValue);
            }
        }

        void LogEncoding(DbConnection conn)
        {
            var command = conn.CreateCommand();
            command.CommandText = "pragma encoding;";

            var encoding = command.ExecuteScalar() as string;

            Output.WriteLine($"encoding {encoding}");
        }
    }
}
