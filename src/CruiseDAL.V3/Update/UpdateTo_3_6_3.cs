using CruiseDAL.Schema;
using FMSC.ORM.Core;
using System.Data.Common;

namespace CruiseDAL.Update
{
    public class UpdateTo_3_6_3 : DbUpdateBase
    {
        public UpdateTo_3_6_3()
            : base("3.6.3", new[] { "3.6.2", })
        { }

        protected override void BeforeBegin(DbConnection conn, IExceptionProcessor exceptionProcessor)
        {
            conn.ExecuteNonQuery("PRAGMA foreign_keys=OFF;", exceptionProcessor: exceptionProcessor);
        }

        protected override void AfterCommit(DbConnection conn, IExceptionProcessor exceptionProcessor)
        {
            conn.ExecuteNonQuery("PRAGMA foreign_keys=ON;", exceptionProcessor: exceptionProcessor);
        }

        protected override void DoUpdate(DbConnection conn, DbTransaction transaction, IExceptionProcessor exceptionProcessor)
        {
            conn.ExecuteNonQuery("DELETE FROM Species_Product WHERE length(trim(ContractSpecies)) == 0;");
            conn.ExecuteNonQuery("DROP TRIGGER Species_OnUpdate_ContractSpecies;");

            RebuildTable(conn, transaction, exceptionProcessor, new Species_ProductTableDefinition_363());

            conn.ExecuteNonQuery(SpeciesTableDefinition361.CREATE_TRIGGER_Species_OnUpdate_ContractSpecies);
        }
    }
}