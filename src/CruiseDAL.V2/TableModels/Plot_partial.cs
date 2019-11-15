using FMSC.ORM.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.DataObjects
{
    public partial class PlotDO
    {
        public static List<PlotDO> ReadyByUnitStratum(Datastore dal, String unit, String stratum)
        {
            if (dal == null) { return null; }
            if (String.IsNullOrEmpty(unit))
            {
                return dal.From<PlotDO>()
                    .Join("Stratum", "USING (Stratum_CN)")
                    .Where("Stratum.Code = @p1")
                    .Read(stratum).ToList();

                //.Read<PlotDO>("JOIN Stratum WHERE Plot.Stratum_CN = Stratum.Stratum_CN AND Stratum.Code = ?;", (object)stratum);
            }
            else if (String.IsNullOrEmpty(stratum))
            {
                return dal.From<PlotDO>()
                    .Join("CuttingUnit", "USING (CuttingUnit_CN)")
                    .Where("CuttingUnit.Code = @p1")
                    .Read(unit).ToList();

                //.Read<PlotDO>("JOIN CuttingUnit WHERE Plot.CuttingUnit_CN = CuttingUnit.CuttingUnit_CN AND CuttingUnit.Code = ?;", (object)unit);
            }
            return dal.From<PlotDO>()
                .Join("CuttingUnit", "USING (CuttingUnit_CN)")
                .Join("Stratum", "USING (Stratum_CN)")
                .Where("CuttingUnit.Code = @p1 AND Stratum.Code = @p2")
                .Read(unit, stratum).ToList();

            //.Read<PlotDO>("JOIN CuttingUnit JOIN Stratum WHERE Plot.CuttingUnit_CN = CuttingUnit.CuttingUnit_CN AND CuttingUnit.Code = ? AND Plot.Stratum_CN = Stratum.Stratum_CN AND Stratum.Code = ?;", (object)unit, stratum);
        }

        public static bool RecursiveDeletePlot(PlotDO plot)
        {
            Datastore dal = plot.DAL;
            string command = string.Format(@"
DELETE FROM LogStock;
DELETE FROM TreeCalculatedValues;
DELETE FROM Log WHERE EXISTS (SELECT 1 FROM Tree WHERE Tree.Tree_CN = Log.Tree_CN AND Tree.Plot_CN = {0});
DELETE FROM Tree WHERE Plot_CN = {0};", plot.Plot_CN);
            dal.Execute(command);
            plot.Delete();
            return true;
        }
    }
}