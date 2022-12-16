using CruiseDAL.Schema;
using FMSC.ORM.Core;
using System.Data.Common;

namespace CruiseDAL.Update
{
    public class UpdateTo_3_6_1 : DbUpdateBase
    {
        public UpdateTo_3_6_1()
            : base(targetVersion: "3.6.1", sourceVersions: new[] { "3.6.0", })
        {
        }

        protected override void DoUpdate(DbConnection conn, DbTransaction transaction, IExceptionProcessor exceptionProcessor)
        {
            conn.ExecuteNonQuery("DROP TRIGGER Species_OnUpdate_ContractSpecies;");
            conn.ExecuteNonQuery(SpeciesTableDefinition361.CREATE_TRIGGER_Species_OnUpdate_ContractSpecies);
        }
    }
}