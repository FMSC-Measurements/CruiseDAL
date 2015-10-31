using System;
using System.Collections.Generic;

using CruiseDAL.DataObjects;

namespace CruiseDAL
{
    public static class DALValidationExtentions
    {

        public static bool HasCruiseErrors(this DALRedux dal, out string[] errors)
        {
            bool hasErrors = false;
            List<string> errorList = new List<string>();

            if (dal.HasForeignKeyErrors(null))
            {
                errorList.Add("File contains Foreign Key errors");
                hasErrors = true;
            }

            //if (HasMismatchSpecies())
            //{
            //    errorList.Add("Tree table has mismatch species codes");
            //    hasErrors = true;
            //}

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
                AND ifnull(Tree.TreeDefaultValue_CN, 0) = 0;");
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

        public static bool HasCruiseErrors(this DALRedux dal)
        {
            string[] errors;
            return dal.HasCruiseErrors(out errors);
        }

        private static bool HasBlankSpeciesCodes(this DALRedux dal)
        {
            return dal.GetRowCount(TREE._NAME, "WHERE ifnull(Species, '') = ''") > 0;
        }

        private static bool HasBlankLiveDead(this DALRedux dal)
        {
            return dal.GetRowCount(TREE._NAME, "WHERE ifnull(LiveDead, '') = ''") > 0;
        }

        private static bool HasBlankCountOrMeasure(this DALRedux dal)
        {
            return dal.GetRowCount(TREE._NAME, "WHERE ifnull(CountOrMeasure, '') = ''") > 0;
        }

        private static bool HasBlankDefaultLiveDead(this DALRedux dal)
        {
            return dal.GetRowCount(SAMPLEGROUP._NAME, "WHERE ifnull(DefaultLiveDead, '') = ''") > 0;
        }

        //private bool HasMismatchSpecies()
        //{
        //    return this.GetRowCount("Tree", "JOIN TreeDefaultValue USING (TreeDefaultValue_CN) WHERE Tree.Species != TreeDefaultValue.Species") > 0;
        //}

        private static bool HasSampleGroupUOMErrors(this DALRedux dal)
        {
            return (dal.ExecuteScalar<long>("Select Count(DISTINCT UOM) FROM SampleGroup WHERE UOM != '04';")) > 1L;
            //return this.GetRowCount("SampleGroup", "WHERE UOM != '04' GROUP BY UOM") > 1;
        }

        private static bool HasOrphanedStrata(this DALRedux dal)
        {
            return dal.GetRowCount(STRATUM._NAME, "LEFT JOIN CuttingUnitStratum USING (Stratum_CN) WHERE CuttingUnitStratum.Stratum_CN IS NULL") > 0;
        }

        private static bool HasStrataWithNoSampleGroups(this DALRedux dal)
        {
            return dal.GetRowCount(STRATUM._NAME, "LEFT JOIN SampleGroup USING (Stratum_CN) WHERE SampleGroup.Stratum_CN IS NULL") > 0;
        }

        public static bool HasForeignKeyErrors(this DALRedux dal, string table_name)
        {
            bool hasErrors = false;
            string comStr;
            if (string.IsNullOrEmpty(table_name))
            {
                comStr = "PRAGMA foreign_key_check;";
            }
            else
            {
                comStr = string.Format("PRAGMA foreign_key_check({0});", table_name);
            }
            lock (this._connectionSyncLock)
            {
                DbConnection conn = OpenConnection();
                DbDataReader reader = null;
                DbCommand command = this.CreateCommand(conn, comStr);
                try
                {
                    reader = command.ExecuteReader();
                    hasErrors = reader.Read();
                }
                catch (Exception e)
                {
                    throw this.ThrowDatastoreExceptionHelper(conn, command, e);
                }
                finally
                {
                    if (reader != null) { reader.Dispose(); }
                    if (command != null) { command.Dispose(); }
                    ReleaseConnection();
                }

                return hasErrors;
            }

        }
    }
}
