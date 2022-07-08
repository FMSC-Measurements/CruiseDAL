using CruiseDAL.Schema;
using FMSC.ORM.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Update
{
    // update 3.3.2 notes:
    // added Biomass and ValueEquation tables
    // changed TallyPopulation view so that 3p methods are always treaded as tally by subpop
    public class UpdateTo_3_3_2 : DbUpdateBase
    {
        public UpdateTo_3_3_2() 
            : base(targetVersion: "3.3.2", sourceVersions: new[] {"3.3.1"})
        {
        }

        protected override void DoUpdate(DbConnection conn, DbTransaction transaction, IExceptionProcessor exceptionProcessor)
        {
            var biomassTableDef = new BiomassEquationTableDefinition();
            var valueEqTableDef = new ValueEquationTableDefinition();

            CreateTable(conn, transaction, exceptionProcessor, biomassTableDef);
            CreateTable(conn, transaction, exceptionProcessor, valueEqTableDef);

            // always treat 3p methods as tally by subpop
            var tallyPopViewDef = new TallyPopulationViewDefinition_3_3_2();
            RecreateView(conn, tallyPopViewDef, transaction, exceptionProcessor);
        }
    }
}
