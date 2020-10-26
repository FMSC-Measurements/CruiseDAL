using FluentAssertions;
using FMSC.ORM.Core;
using System;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace FMSC.ORM.SQLite
{
    public class Sqlite_Integration_Test : TestBase
    {
        public Sqlite_Integration_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        // the official behavior is, if a command is executed without assigning the 
        // Transaction property an exception should be thrown. However through testing 
        // it apears this is not so. This test should fail if that behavior is changed
        // https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/local-transactions
        public void ExecuteCommandWithOutAssigningTrasaction()
        {
            using(var connection = GetOpenConnection())
            {
                var transaction = connection.BeginTransaction();
                var command = connection.CreateCommand();
                command.CommandText = "CREATE TABLE a (something TEXT);";
                var result = command.ExecuteScalar();
                transaction.Commit();
            }
        }

        [Theory]
#if !MICROSOFT_DATA_SQLITE
        [InlineData(1L, "1", "?1" )]
        [InlineData(1L, null, "?")]
        [InlineData(1L, "1", "?")]
#endif

#if !SYSTEM_DATA_SQLITE
        [InlineData(1L, "?1", "?1")]
#endif
        [InlineData(1L, "@1", "@1")]
        [InlineData(1L, "@something", "@Something")]
        public void Bind_Paramater(object value, string pName, string pExpr)
        {
            using (var connection = GetOpenConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = $"SELECT {pExpr};";
                var parm = command.CreateParameter();
                parm.ParameterName = pName;
                parm.Value = value;
                command.Parameters.Add(parm);

                var result = command.ExecuteScalar();

                Assert.Equal(value, result);
            }
        }

#if !SYSTEM_DATA_SQLITE
        // this may change see issue: https://github.com/dotnet/efcore/issues/18861
        [Theory]
        [InlineData("@something", "@Something")]
        [InlineData("@Something", "@something")]

        public void Bind_Pramater_is_case_sensitive(string pName, string pExpr)
        {
            var value = 1L;

            using (var connection = GetOpenConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = $"SELECT {pExpr};";
                var parm = command.CreateParameter();
                parm.ParameterName = pName;
                parm.Value = value;
                command.Parameters.Add(parm);

                var result = command.Invoking( x=> x.ExecuteScalar()).Should().Throw<Exception>();
            }
        }
#endif

#if MICROSOFT_DATA_SQLITE

        [Fact]
        public void Bind_Fails_with_qMark_param()
        {
            using (var connection = DbProvider.CreateConnection())
            {
                connection.ConnectionString = "Data Source=:memory:";

                var command = connection.CreateCommand();
                command.CommandText = $"SELECT ?;";

                connection.Open();

                var result = command.Invoking(x => x.ExecuteScalar())
                    .Should().Throw<InvalidOperationException>()
                    .And.Source.Should().Be("Microsoft.Data.Sqlite");
            }
        }

#endif

        [Fact]
        public void Echo_value_guid()
        {
            var pName = "@p1";

            var guid = Guid.NewGuid();

            using (var connection = GetOpenConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = $"SELECT {pName};";
                var parm = command.CreateParameter();
                parm.ParameterName = pName;
                parm.Value = guid;
                command.Parameters.Add(parm);

                var result = command.ExecuteScalar();

#if SYSTEM_DATA_SQLITE
                result.Should().BeOfType(typeof(Byte[]));
                var result_byteA = result as Byte[];
                result_byteA.Should().BeEquivalentTo(guid.ToByteArray());

                var result_guid = new Guid(result_byteA);
                result_guid.Should().Be(guid);
#elif MICROSOFT_DATA_SQLITE
                result.Should().BeOfType(typeof(string));
                var result_guid = new Guid(result as string);
                result_guid.Should().Be(guid);
#endif
            }
        }

        [Fact]
        public void Echo_value_guid_string_with_getGuid()
        {
            var pName = "@p1";

            var guid = Guid.NewGuid();

            using (var connection = GetOpenConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = $"SELECT {pName};";
                //command.CommandText = $"SELECT CAST({pName} AS TEXT);";
                //command.CommandText = $"CREATE TEMP TABLE tbl1 (guid_field TEXT);" +
                //    $"INSERT INTO tbl1 VALUES ({pName});" +
                //    $"SELECT * FROM tbl1;";
                var parm = command.CreateParameter();
                parm.ParameterName = pName;
                parm.Value = guid;
                command.Parameters.Add(parm);

                var reader = command.ExecuteReader();
                reader.Read().Should().BeTrue();

                var value = reader.GetValue(0);

#if SYSTEM_DATA_SQLITE
                reader.Invoking(x => x.GetGuid(0)).Should().Throw<FormatException>();
#endif
#if MICROSOFT_DATA_SQLITE
                reader.Invoking(x => x.GetGuid(0)).Should().NotThrow<FormatException>();
#endif
            }
        }

        [Fact]
        public void Echo_value_guid_string_with_getGuid_2()
        {
            var pName = "@p1";

            var guid = Guid.NewGuid();

            using (var connection = GetOpenConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = $"CREATE TEMP TABLE tbl1 (guid_field TEXT);" +
                    $"INSERT INTO tbl1 VALUES ({pName});" +
                    $"SELECT * FROM tbl1;";
                var parm = command.CreateParameter();
                parm.ParameterName = pName;
                parm.Value = guid;
                command.Parameters.Add(parm);

                var reader = command.ExecuteReader();
                reader.Read().Should().BeTrue();

                var value = reader.GetValue(0);

#if SYSTEM_DATA_SQLITE
                reader.Invoking(x => x.GetGuid(0)).Should().Throw<FormatException>();
#endif
#if MICROSOFT_DATA_SQLITE
                reader.Invoking(x => x.GetGuid(0)).Should().NotThrow<FormatException>();
#endif
            }
        }

        [Fact]
        // expected behavior is that the datetime values is converted to
        // a string when used in a parameter. format is in ISO-8601 https://en.wikipedia.org/wiki/ISO_8601
        public void Echo_value_datetime_1()
        {
            var pName = "@p1";
            var expectedValue = DateTime.Today;

            using (var connection = GetOpenConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT {pName};";
                    var parm = command.CreateParameter();
                    parm.ParameterName = pName;
                    parm.Value = expectedValue;
                    command.Parameters.Add(parm);

                    using (var reader = command.ExecuteReader())
                    {
                        reader.Read().Should().BeTrue();

                        var value = reader.GetValue(0);
                        value.Should().BeOfType<String>();
                        value.Should().Be(expectedValue.ToString(@"yyyy\-MM\-dd HH\:mm\:ss.FFFFFFF", CultureInfo.InvariantCulture));

                        reader.Invoking(x => x.GetString(0)).Should().NotThrow();
                        reader.Invoking(x => x.GetDateTime(0)).Should().NotThrow();
                    }
                }
            }
        }

        [Fact]
        // expected behavior is that the datetime values is converted to
        // a string when used in a parameter. format is in ISO-8601 https://en.wikipedia.org/wiki/ISO_8601
        public void Echo_value_datetime_2()
        {
            var pName = "@p1";
            var expectedValue = DateTime.Today;

            using (var connection = GetOpenConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = $"SELECT CAST({pName} as DATETIME);";
                var parm = command.CreateParameter();
                parm.ParameterName = pName;
                parm.Value = expectedValue;
                command.Parameters.Add(parm);

                //command.CommandText =
                //    $"CREATE TABLE IF NOT EXISTS tbl1 (dt_field DATETIME);" +
                //    $"INSERT INTO tbl1 VALUES ({pName});" +
                //    $"SELECT dt_field FROM tbl1 WHERE rowid = last_insert_rowid();";
                //var parm = command.CreateParameter();
                //parm.ParameterName = pName;
                //parm.Value = expectedValue;
                //command.Parameters.Add(parm);

                using (var reader = command.ExecuteReader())
                {
                    reader.Read().Should().BeTrue();

                    var value = reader.GetValue(0);
                    value.Should().BeOfType<Int64>();

                    value.Should().Be(expectedValue.Year);

                    //reader.Invoking(x => x.GetString(0)).Should().NotThrow();
                    //reader.Invoking(x => x.GetDateTime(0)).Should().NotThrow();
                }
            }
        }

        [Fact]
        // expected behavior is that the datetime values is converted to
        // a string when used in a parameter. format is in ISO-8601 https://en.wikipedia.org/wiki/ISO_8601
        public void Echo_value_datetime_3()
        {
            var pName = "@p1";
            var expectedValue = DateTime.Today;

            using (var connection = GetOpenConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText =
                    $"CREATE TABLE IF NOT EXISTS tbl1 (dt_field DATETIME);" +
                    $"INSERT INTO tbl1 VALUES ({pName});" +
                    $"SELECT dt_field FROM tbl1 WHERE rowid = last_insert_rowid();";
                var parm = command.CreateParameter();
                parm.ParameterName = pName;
                parm.Value = expectedValue;
                command.Parameters.Add(parm);

                using (var reader = command.ExecuteReader())
                {
                    reader.Read().Should().BeTrue();

                    var value = reader.GetValue(0);
#if SYSTEM_DATA_SQLITE
                    value.Should().BeOfType<DateTime>();
#elif MICROSOFT_DATA_SQLITE
                    value.Should().BeOfType<string>();
#endif

                    reader.Invoking(x => x.GetDateTime(0)).Should().NotThrow();

                    reader.Invoking(x => x.GetString(0)).Should().NotThrow();
                    var value_str = reader.GetString(0);
                    value_str.Should().Be(expectedValue.ToString(@"yyyy\-MM\-dd HH\:mm\:ss.FFFFFFF", CultureInfo.InvariantCulture));
                }
            }
        }

        [Fact]
        public void Echo_str_as_datetime()
        {
            var pName = "@p1";
            var strValue = "6/22/2015 4:20 PM";

            using (var connection = GetOpenConnection())
            {
                var command = connection.CreateCommand();
                //command.CommandText = $"SELECT CAST({pName} as DATETIME);";

                command.CommandText =
                    $"CREATE TABLE IF NOT EXISTS tbl1 (dt_field DATETIME);" +
                    $"INSERT INTO tbl1 VALUES ({pName});" +
                    $"SELECT dt_field FROM tbl1 WHERE rowid = last_insert_rowid();";
                var parm = command.CreateParameter();
                parm.ParameterName = pName;
                parm.Value = strValue;
                command.Parameters.Add(parm);

                using (var reader = command.ExecuteReader())
                {
                    reader.Read().Should().BeTrue();

                    var str = reader.GetString(0);
                    str.Should().Be(strValue);

#if SYSTEM_DATA_SQLITE
                    reader.Invoking(x => x.GetValue(0)).Should().Throw<FormatException>();
                    reader.Invoking(x => x.GetDateTime(0)).Should().Throw<FormatException>();
#endif

#if MICROSOFT_DATA_SQLITE
                    var value = reader.GetValue(0);
                    value.Should().BeOfType<String>();

                    var result = reader.GetDateTime(0);
#endif
                }
            }
        }

        [Fact]
        public void Echo_datetime_as_str_2()
        {
            var pName = "@p1";
            var strValue = "6/22/2015 4:20 PM";

            using (var connection = GetOpenConnection())
            {
                var command = connection.CreateCommand();
                //command.CommandText = $"SELECT CAST({pName} as DATETIME);";                

                command.CommandText =
                    $"CREATE TABLE IF NOT EXISTS tbl1 (dt_field DATETIME);" +
                    $"INSERT INTO tbl1 VALUES ({pName});" +
                    $"SELECT dt_field FROM tbl1 WHERE rowid = last_insert_rowid();";
                var parm = command.CreateParameter();
                parm.ParameterName = pName;
                parm.Value = strValue;
                command.Parameters.Add(parm);

                using (var reader = command.ExecuteReader())
                {
                    reader.Read().Should().BeTrue();

#if SYSTEM_DATA_SQLITE
                    reader.Invoking(x => x.GetValue(0)).Should().Throw<FormatException>();
#elif MICROSOFT_DATA_SQLITE
                    var value = reader.GetValue(0);
                    value.Should().Be(strValue);
#endif

                    var str = reader.GetString(0);
                    str.Should().Be(strValue);
                }
            }
        }

        [Fact]
        public void Echo_datetime_as_str()
        {
            var pName = "@p1";
            var datetime = DateTime.Now;

            using (var connection = GetOpenConnection())
            {
                var command = connection.CreateCommand();
                //command.CommandText = $"SELECT CAST({pName} as DATETIME);";                

                command.CommandText =
                    $"CREATE TABLE IF NOT EXISTS tbl1 (dt_field DATETIME);" +
                    $"INSERT INTO tbl1 VALUES ({pName});" +
                    $"SELECT dt_field FROM tbl1 WHERE rowid = last_insert_rowid();";
                var parm = command.CreateParameter();
                parm.ParameterName = pName;
                parm.Value = datetime;
                command.Parameters.Add(parm);

                using (var reader = command.ExecuteReader())
                {
                    reader.Read().Should().BeTrue();

                    var value = reader.GetValue(0);
                    value.Should().NotBeNull();

                    var str = reader.GetString(0);
                    str.Should().Be(datetime.ToString(@"yyyy\-MM\-dd HH\:mm\:ss.FFFFFFF", CultureInfo.InvariantCulture));
                }
            }
        }



        [Fact]
        // the guid is converted to a byte array when add to a command as a paramiter.
        // when read back as a string it will be a jiberish string.
        //
        // unfortunatly when converted to a string some of the bits get truncated, so the
        // echoed string isn't equivalant to original guid.
        //
        // we convert the byte array of guid to a string, we can not convert
        // the result string to a byte array and then back to a guid. At least I tryed
        public void Echo_value_guid_as_str()
        {
            var pName = "@p1";

            var guid = Guid.NewGuid();

            using (var connection = GetOpenConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = $"SELECT CAST({pName} AS TEXT);";
                var parm = command.CreateParameter();
                parm.ParameterName = pName;
                parm.Value = guid;
                command.Parameters.Add(parm);

                var result = command.ExecuteScalar();

#if SYSTEM_DATA_SQLITE
                result.Should().BeOfType(typeof(string));
                var result_str = result as string;

                LogEncoding(connection);

                var guid_byteArr = guid.ToByteArray();
                var guid_str = Encoding.UTF8.GetString(guid_byteArr);
                result_str.Should().Be(guid_str);

                // try to convert the result string to a byte array then to a guid
                var result_cArray = result_str
                    .PadRight(16, default(Char)) // sometimes some chars get cut off the end if they are /0, add them back
                    .ToArray();

                // convert char array to byte array
                var result_bArray = result_cArray
                    .Select(x => (byte)(x & 0x7f))
                    .ToArray();

                var result_guid = new Guid(result_bArray);
                Output.WriteLine(result_guid.ToString());

#elif MICROSOFT_DATA_SQLITE
                result.Should().BeOfType(typeof(string));
                var result_guid = new Guid(result as string);
                result_guid.Should().Be(guid);
#endif
            }
        }

        private void LogEncoding(DbConnection conn)
        {
            var command = conn.CreateCommand();
            command.CommandText = "pragma encoding;";

            var encoding = command.ExecuteScalar() as string;

            Output.WriteLine($"encoding {encoding}");
        }

#if SYSTEM_DATA_SQLITE
        [Fact]
#endif
        // when a raw guid is inserted into the database it is converted
        // to binary. Unfortuanatly
        // when read back as a string it will be a jiberish string.
        // although, this string will be the same as the string we get when
        // we convert the byte array of guid to a string, we can not convert
        // the result string to a byte array and then back to a guid. At least I tryed
        public void Echo_value_guid_as_byteArray()
        {
            using (var connection = GetOpenConnection())
            {
                foreach (var e in Enumerable.Range(0, 0xfff))
                {
                    var guid = Guid.NewGuid();

                    var bArray = EchoGuidAsByteArray(connection, guid);
                    var guid_Again = new Guid(bArray);
                    guid_Again.Should().Be(guid); // because of lossful conversion the guids should not be the same

                    var bArray_again = EchoGuidAsByteArray(connection, guid_Again);
                    //var diff = bArray_again.Diff(bArray).ToArray();
                    bArray_again.Should().BeEquivalentTo(bArray);

                    var guid_agian_again = new Guid(bArray_again);
                    guid_agian_again.Should().Be(guid_Again);

                    Output.WriteLine(".");
                }
            }

            byte[] EchoGuidAsByteArray(DbConnection connection, Guid guid)
            {
                var pName = "@p1";
                using (var command = connection.CreateCommand())
                {
                    //command.CommandText = $"SELECT CAST({pName} AS TEXT);";
                    command.CommandText =
                        $"CREATE TABLE IF NOT EXISTS tbl1 (guid_field TEXT);" +
                        $"INSERT INTO tbl1 VALUES ({pName});" +
                        $"SELECT guid_field FROM tbl1 WHERE rowid = last_insert_rowid();";
                    var parm = command.CreateParameter();
                    parm.ParameterName = pName;
                    parm.Value = guid;
                    command.Parameters.Add(parm);

                    using (var reader = command.ExecuteReader())
                    {
                        reader.Read().Should().BeTrue();

                        var value = reader.GetValue(0);
#if SYSTEM_DATA_SQLITE
                        value.Should().BeOfType<string>();
#endif

                        var bbuffer = new byte[16];
                        reader.GetBytes(0, 0, bbuffer, 0, 16);
                        return bbuffer;
                    }
                }
            }
        }

        [Theory]
        [InlineData("1", true)]
        [InlineData("0", false)]
        [InlineData("1 IS NOT NULL", true)]
        [InlineData("1 IS NULL", false)]
        public void Echo_value_bool(string quoteValue, bool expectedValue)
        {
            using (var connection = GetOpenConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = $"SELECT {quoteValue};";

                var reader = command.ExecuteReader();
                reader.Read();

                var result = reader.GetBoolean(0);
                result.Should().Be(expectedValue);
            }
        }
    }
}