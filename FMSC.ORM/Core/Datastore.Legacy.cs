using FMSC.ORM.EntityModel.Support;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.Core
{
    public partial class DatastoreRedux
    {

        #region read methods
        [Obsolete("use From<T>().Read() style instead")]
#pragma warning disable RECS0154 // Parameter tableName is never used
        public List<T> Read<T>(string tableName, string selection, params Object[] selectionArgs) where T : new()
#pragma warning restore RECS0154 // Parameter is never used
        {
            EntityDescription entityDescription = LookUpEntityByType(typeof(T));
            EntityCommandBuilder commandBuilder = entityDescription.CommandBuilder;

            using (DbCommand command = commandBuilder.BuildSelectLegacy(Provider, selection))
            {
                //Add selection Arguments to command parameter list
                if (selectionArgs != null)
                {
                    foreach (object obj in selectionArgs)
                    {
                        command.Parameters.Add(Provider.CreateParameter(null, obj));
                    }
                }

                return Read<T>(command, entityDescription);
            }
        }

        //        [Obsolete("use ReadSingleRow<T>(object primaryKey) instead")]
        //#pragma warning disable RECS0154 // Parameter is never used
        //        public T ReadSingleRow<T>(string tableName, long? rowID) where T : new()
        //#pragma warning restore RECS0154 // Parameter is never used
        //        {
        //            return ReadSingleRow<T>(rowID);
        //        }


        [Obsolete("use From<T>().Read().FirstOrDefault() style instead")]
#pragma warning disable RECS0154 // Parameter is never used
        public T ReadSingleRow<T>(string tableName, string selection, params Object[] selectionArgs) where T : new()
#pragma warning restore RECS0154 // Parameter is never used
        {
            EntityDescription entityDescription = LookUpEntityByType(typeof(T));
            EntityCommandBuilder commandBuilder = entityDescription.CommandBuilder;

            using (DbCommand command = commandBuilder.BuildSelectLegacy(Provider, selection))
            {
                //Add selection Arguments to command parameter list
                if (selectionArgs != null)
                {
                    foreach (object obj in selectionArgs)
                    {
                        command.Parameters.Add(Provider.CreateParameter(null, obj));
                    }
                }

                return ReadSingleRow<T>(command, entityDescription);
            }
        }
        #endregion

        #region general purpose command execution

        public int Execute(String command, IEnumerable<KeyValuePair<String, object>> parameters)
        {
            using (DbCommand comm = Provider.CreateCommand(command))
            {
                if (parameters != null)
                {
                    foreach (var pair in parameters)
                    {
                        var param = Provider.CreateParameter(pair.Key, pair.Value);
                        comm.Parameters.Add(param);
                    }
                }
                return ExecuteSQL(comm);
            }
        }


        public T ExecuteScalar<T>(String query, IEnumerable<KeyValuePair<String, object>> parameters)
        {
            using (DbCommand comm = Provider.CreateCommand(query))
            {
                if (parameters != null)
                {
                    foreach (var pair in parameters)
                    {
                        var param = Provider.CreateParameter(pair.Key, pair.Value);
                        comm.Parameters.Add(param);
                    }
                }
                return ExecuteScalar<T>(comm);
            }
        }

        #endregion
    }
}
