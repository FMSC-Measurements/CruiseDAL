﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.V2Backports
{
    public partial class DDL
    {
        public const string CREATE_VIEW_PLOT =
            "CREATE VIEW Plot AS " +
            "SELECT " +
                "Plot_Stratum_CN AS Plot_CN, " +
                "st.Stratum_CN, " +
                "cu.CuttingUnit_CN, " +
                "ps.PlotNumber, " +
                "(CASE ps.IsEmpty WHEN 0 THEN 'False' ELSE 'True' END) AS IsEmpty, " +
                "p.Slope, " +
                "ps.KPI, " +
                "ps.ThreePRandomValue" +
                "p.Aspect, " +
                "p.Remarks, " +
                "p.XCoordinate, " +
                "p.YCoordinate, " +
                "p.ZCoordinate, " +
                "null AS MetaData, " +
                "null AS Blob, " +
                "ps.CreatedBy, " +
                "ps.CreatedDate, " +
                "ps.ModifiedBy, " +
                "ps.RowVersion " +
            "FROM Plot_Stratum AS ps " +
            "JOIN Plot_V3 AS p  USING (PlotNumber, CuttingUnitCode);";
    }
}