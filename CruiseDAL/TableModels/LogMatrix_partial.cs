using System;
using System.Collections.Generic;
using System.Text;
using CruiseDAL.MappingCollections;
using System.Linq;

namespace CruiseDAL.DataObjects
{
    public partial class LogMatrixDO
    {

        public static Dictionary<String, List<LogMatrixDO>> GetByRowNumberGroupBySpecies(DAL cruiseDAL, string reportNum)
        {
            var dict = new Dictionary<string, List<LogMatrixDO>>();
            var list = cruiseDAL.From<LogMatrixDO>()
                .Where("ReportNumber = ?")
                .Read(reportNum).ToList();

                //.Read<LogMatrixDO>("WHERE ReportNumber = ?", (object)reportNum);

            foreach (LogMatrixDO lm in list)
            {
                if (dict.ContainsKey(lm.Species))
                {
                    dict[lm.Species].Add(lm);
                }
                else
                {
                    var l = new List<LogMatrixDO>();
                    l.Add(lm);
                    dict.Add(lm.Species, l);
                }

            }

            return dict;


        }

    }
}