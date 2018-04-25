using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FMSC.ORM.Test.SQLite
{
    public class SqliteTest : TestBase
    {
        public SqliteTest(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData(1L, "1", "?1" 
#if MICROSOFT_DATA_SQLITE 
            ,Skip ="not supported my Microsoft.Data.Sqlite" 
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
    }
}
