using CruiseDAL.Schema;
using System.Collections.Generic;

namespace CruiseDAL
{
    public static class DALValidationExtentions
    {
        public static bool HasCruiseErrors(this CruiseDatastore dal, out string[] errors)
        {
            bool hasErrors = false;
            var errorList = new List<string>();

            if (dal.HasForeignKeyErrors(null))
            {
                errorList.Add("File contains Foreign Key errors");
                hasErrors = true;
            }

            if (HasMismatchSpeciesOrLiveDead(dal))
            {
                FixMismatchSpecies(dal);
                if (HasMismatchSpeciesOrLiveDead(dal))
                {
                    errorList.Add("Tree table has mismatch species codes");
                    hasErrors = true;
                }
            }

            if (dal.HasSampleGroupUOMErrors())
            {
                errorList.Add("Sample Group table has invalid mix of UOM");
                hasErrors = true;
            }

            if (dal.HasBlankCountOrMeasure())
            {
                errorList.Add("Tree table has record(s) with blank Count or Measure value");
                hasErrors = true;
            }
            if (dal.HasBlankDefaultLiveDead())
            {
                errorList.Add("Sample Group table has record(s) with blank default live dead vaule");
                hasErrors = true;
            }
            if (dal.HasBlankLiveDead())
            {
                errorList.Add("Tree table has record(s) with blank Live Dead value");
                hasErrors = true;
            }
            if (dal.HasBlankSpeciesCodes())
            {
                dal.Execute(
                @"Update Tree
                SET Species =
                    (Select Species FROM TreeDefaultValue
                        WHERE TreeDefaultValue.TreeDefaultValue_CN = Tree.TreeDefaultValue_CN)
                WHERE ifnull(Tree.Species, '') = ''
                AND ifnull(Tree.TreeDefaultValue_CN, 0) != 0;");
                if (dal.HasBlankSpeciesCodes())
                {
                    errorList.Add("Tree table has record(s) with blank species or no tree default");
                    hasErrors = true;
                }
            }

            if (dal.HasOrphanedStrata())
            {
                errorList.Add("Stratum table has record(s) that have not been assigned to a cutting unit");
                hasErrors = true;
            }
            if (dal.HasStrataWithNoSampleGroups())
            {
                errorList.Add("Stratum table has record(s) that have not been assigned any sample groups");
                hasErrors = true;
            }

            errors = errorList.ToArray();
            return hasErrors;
        }

        public static bool HasCruiseErrors(this CruiseDatastore dal)
        {
            string[] errors;
            return dal.HasCruiseErrors(out errors);
        }

        private static bool HasBlankSpeciesCodes(this CruiseDatastore dal)
        {
            return dal.GetRowCount(TREE._NAME, "WHERE ifnull(Species, '') = ''") > 0;
        }

        private static bool HasBlankLiveDead(this CruiseDatastore dal)
        {
            return dal.GetRowCount(TREE._NAME, "WHERE ifnull(LiveDead, '') = ''") > 0;
        }

        private static bool HasBlankCountOrMeasure(this CruiseDatastore dal)
        {
            return dal.GetRowCount(TREE._NAME, "WHERE ifnull(CountOrMeasure, '') = ''") > 0;
        }

        private static bool HasBlankDefaultLiveDead(this CruiseDatastore dal)
        {
            return dal.GetRowCount(SAMPLEGROUP._NAME, "WHERE ifnull(DefaultLiveDead, '') = ''") > 0;
        }

        private static bool HasMismatchSpeciesOrLiveDead(CruiseDatastore dal)
        {
            return dal.GetRowCount("Tree", "JOIN TreeDefaultValue AS tdv USING (TreeDefaultValue_CN) WHERE Tree.Species != tdv.Species OR Tree.LiveDead != tdv.LiveDead;") > 0;
        }

        private static void FixMismatchSpecies(CruiseDatastore dal)
        {
            dal.Execute(
@"UPDATE Tree
SET [Species] =
        (SELECT [Species]
        FROM TreeDefaultValue AS tdv
        WHERE tdv.TreeDefaultValue_CN = Tree.TreeDefaultValue_CN),
    LiveDead =
        (SELECT [LiveDead]
        FROM TreeDefaultValue AS tdv
        WHERE tdv.TreeDefaultValue_CN = Tree.TreeDefaultValue_CN)
WHERE Tree_CN IN
    (SELECT Tree_CN from Tree
    JOIN TreeDefaultValue as tdv using (TreeDefaultVAlue_CN)
    WHERE tree.[Species] != tdv.[Species]
        AND tree.LiveDead != tdv.LiveDead);");
        }

        private static bool HasSampleGroupUOMErrors(this CruiseDatastore dal)
        {
            return (dal.ExecuteScalar<long>("Select Count(DISTINCT UOM) FROM SampleGroup WHERE UOM != '04';")) > 1L;
            //return this.GetRowCount("SampleGroup", "WHERE UOM != '04' GROUP BY UOM") > 1;
        }

        private static bool HasOrphanedStrata(this CruiseDatastore dal)
        {
            return dal.GetRowCount("Stratum", "LEFT JOIN CuttingUnitStratum USING (Stratum_CN) WHERE CuttingUnitStratum.Stratum_CN IS NULL") > 0;
        }

        private static bool HasStrataWithNoSampleGroups(this CruiseDatastore dal)
        {
            return dal.GetRowCount("Stratum", "LEFT JOIN SampleGroup USING (Stratum_CN) WHERE SampleGroup.Stratum_CN IS NULL") > 0;
        }
    }
}