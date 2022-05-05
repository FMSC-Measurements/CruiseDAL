using CruiseDAL.V3.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.V3.Sync.Comparers
{
    public class CuttingUnitComparer : EqualityComparer<CuttingUnit>
    {
        public override bool Equals(CuttingUnit x, CuttingUnit y)
        {
            return string.Equals(x.CuttingUnitCode, y.CuttingUnitCode, StringComparison.OrdinalIgnoreCase)
                && x.Area == y.Area
                && string.Equals(x.Description.Trim(), y.Description.Trim(), StringComparison.OrdinalIgnoreCase)
                && string.Equals(x.LoggingMethod, y.LoggingMethod)
                && string.Equals(x.PaymentUnit, y.PaymentUnit);
        }

        public override int GetHashCode(CuttingUnit obj)
        {
            var hashes = new[]
            {
                obj.CuttingUnitCode?.GetHashCode() ?? 0,
                obj.Area?.GetHashCode() ?? 0,
                obj.Description?.GetHashCode() ?? 0,
                obj.LoggingMethod?.GetHashCode() ?? 0,
                obj.PaymentUnit?.GetHashCode() ?? 0,
            };

            return hashes.Where(x => x > 0).Aggregate((x,y) => x ^ y);
        }
    }
}
