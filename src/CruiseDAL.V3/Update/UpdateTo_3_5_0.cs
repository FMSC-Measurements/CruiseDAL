using CruiseDAL.Schema;
using CruiseDAL.Schema.Views;
using FMSC.ORM.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Update
{
    public class UpdateTo_3_5_0 : DbUpdateBase
    {
        public UpdateTo_3_5_0() 
            : base(targetVersion: "3.5.0", sourceVersions: new[] {"3.4.4"})
        {
        }

        protected override void DoUpdate(DbConnection conn, DbTransaction transaction, IExceptionProcessor exceptionProcessor)
        {
            // create Species_Product table
            CreateTable(conn, transaction, exceptionProcessor, new Species_ProductTableDefinition());

            // populate Species_Product table from Species table
            conn.ExecuteNonQuery("INSERT INTO Species_Product (" +
                "CruiseID, " +
                "SpeciesCode, " +
                "PrimaryProduct, " +
                "ContractSpecies " +
                ") " +
                "SELECT CruiseID, SpeciesCode, null AS PrimaryProduct, ContractSpecies " +
                "FROM Species WHERE ContractSpecies IS NOT NULL;", transaction: transaction, exceptionProcessor: exceptionProcessor);

            // add Trigger to keep Species and Species_Product in sync
            conn.ExecuteNonQuery(SpeciesTableDefinition.CREATE_TRIGGER_Species_OnUpdate_ContractSpecies, transaction: transaction, exceptionProcessor: exceptionProcessor);

            // rebuild TallyLedger_Tree_Totals view
            RecreateView(conn, new TallyLedger_Tree_TotalsViewDefinition(), transaction: transaction, exceptionProcessor: exceptionProcessor);
        }
    }
}
