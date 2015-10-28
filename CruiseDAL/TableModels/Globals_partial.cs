using System;
using System.Collections.Generic;
using System.Text;
using CruiseDAL.MappingCollections;

namespace CruiseDAL.DataObjects
{
    public partial class GlobalsDO
    {
        public static bool IsCruiseComponentMaster(DAL dal)
        {
            GlobalsDO g = dal.ReadSingleRow<GlobalsDO>("Globals", "WHERE Block = 'Comp' AND Key = 'ChildComponents'");
            if(g != null && !string.IsNullOrEmpty(g.Value))
            {
                return true;
            }
            return false;
        }


    }

}