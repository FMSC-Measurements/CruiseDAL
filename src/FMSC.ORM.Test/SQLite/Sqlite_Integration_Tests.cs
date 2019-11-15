using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FMSC.ORM.SQLite
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
                var result_byteA = result as Byte[];
                result_byteA.Should().BeEquivalentTo(guid.ToByteArray());

                var result_guid = new Guid(result_byteA);
                result_guid.Should().Be(guid);
                
            }
        }

        [Fact]
        // expected behavior is that the datetime values is converted to 
        // a string when used in a parameter. format is in ISO-8601 https://en.wikipedia.org/wiki/ISO_8601
        public void Echo_value_datetime()
        {
            var pName = "@p1";
            var expectedValue = DateTime.Now;

            using (var connection = DbProvider.CreateConnection())
            {
                connection.ConnectionString = "Data Source=:memory:";

                var command = connection.CreateCommand();
                command.CommandText = $"SELECT {pName};";
                var parm = command.CreateParameter();
                parm.ParameterName = pName;
                parm.Value = expectedValue;
                command.Parameters.Add(parm);

                connection.Open();

                var result = command.ExecuteScalar();
                Output.WriteLine(result.ToString());

                result.Should().BeOfType(typeof(string));
                var resultDT = DateTime.Parse(result as string);
                expectedValue.Should().Be(resultDT);
            }
        }

        [Fact]
        // expected behavor is that the guid is converted to a byte array
        // when add to a command as a paramiter. 
        // when read back as a string it will be a jiberish string.
        // although, this string will be the same as the string we get when 
        // we convert the byte array of guid to a string, we can not convert 
        // the result string to a byte array and then back to a guid. At least I tryed
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
                var result_str = result as string;
                
                LogEncoding(connection);

                var guid_byteArr = guid.ToByteArray();
                var guid_str = Encoding.Default.GetString(guid_byteArr);
                guid_str.Should().Be(result_str);

                // try to convert the result string to a byte array then to a guid
                ////var result_byteArr = result_str.Select(x => (Byte)x).ToArray();
                //var result_byteArr = Encoding.Default.GetBytes(result_str);
                //var result_guid = new Guid(result_byteArr);
                //result_guid.Should().Be(guid);
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
