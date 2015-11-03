using System;
using System.Collections.Generic;
using System.Text;
using CruiseDAL.MappingCollections;

namespace CruiseDAL.DataObjects
{
    public partial class LogMatrixDO
    {

        public static Dictionary<String, List<LogMatrixDO>> GetByRowNumberGroupBySpecies(DALRedux cruiseDAL, string reportNum)
        {
            Dictionary<string, List<LogMatrixDO>> dict = new Dictionary<string, List<LogMatrixDO>>();
            List<LogMatrixDO> list = cruiseDAL.Read<LogMatrixDO>("WHERE ReportNumber = ?", (object)reportNum);

            foreach (LogMatrixDO lm in list)
            {
                if (dict.ContainsKey(lm.Species))
                {
                    dict[lm.Species].Add(lm);
                }
                else
                {
                    List<LogMatrixDO> l = new List<LogMatrixDO>();
                    l.Add(lm);
                    dict.Add(lm.Species, l);
                }

            }

            return dict;


        }

    }
}