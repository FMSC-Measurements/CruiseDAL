using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backpack.SqlBuilder.Dialects;
using FMSC.ORM.EntityModel.Support;

namespace FMSC.ORM.Core
{
    [Obsolete()]
    public abstract class DatastoreRedux : Datastore
    {
        protected DatastoreRedux(ISqlDialect dialect, IExceptionProcessor exceptionProcessor, DbProviderFactory providerFactory) 
            : base(dialect, exceptionProcessor, providerFactory)
        {
        }

        #region read methods

        [Obsolete("use From<T>().Read() style instead")]
#pragma warning disable RECS0154 // Parameter tableName is never used
        public List<T> Read<T>(string tableName, string selection, params Object[] selectionArgs) where T : class, new()
#pragma warning restore RECS0154 // Parameter is never used
        {
            EntityDescription entityDescription = GlobalEntityDescriptionLookup.Instance.LookUpEntityByType(typeof(T));
            EntityCommandBuilder commandBuilder = entityDescription.CommandBuilder;

            return Read<T>(commandBuilder.BuildSelectLegacy(selection), selectionArgs).ToList();
        }

        [Obsolete("use From<T>().Read().FirstOrDefault() style instead")]
#pragma warning disable RECS0154 // Parameter is never used
        public T ReadSingleRow<T>(string tableName, string selection, params Object[] selectionArgs) where T : class, new()
#pragma warning restore RECS0154 // Parameter is never used
        {
            EntityDescription entityDescription = GlobalEntityDescriptionLookup.Instance.LookUpEntityByType(typeof(T));
            EntityCommandBuilder commandBuilder = entityDescription.CommandBuilder;

            var query = commandBuilder.BuildSelectLegacy(selection);
            return Read<T>(query, selectionArgs).FirstOrDefault();
        }

        #endregion read methods

        #region query methods

        public T QuerySingleRecord<T>(string selectCommand, params Object[] selectionArgs) where T : new()
        {
            return Query<T>(selectCommand, selectionArgs).FirstOrDefault();
        }

        //public List<T> Query<T>(string selectCommand, params Object[] selectionArgs) where T : new()
        //{
        //    DbCommand command = Provider.CreateCommand(selectCommand);

        //    //Add selection Arguments to command parameter list
        //    if (selectionArgs != null)
        //    {
        //        foreach (object obj in selectionArgs)
        //        {
        //            command.Parameters.Add(Provider.CreateParameter(null, obj));
        //        }
        //    }

        //    EntityDescription entityType = LookUpEntityByType(typeof(T));
        //    return Query<T>(command, entityType);
        //}

        //protected List<T> Query<T>(IDbCommand command, EntityDescription entityDescription)
        //    where T : new()
        //{
        //    var inflator = entityDescription.Inflator;
        //    var dataList = new List<T>();
        //    IDataReader reader = null;
        //    lock (_persistentConnectionSyncLock)
        //    {
        //        var conn = OpenConnection();
        //        try
        //        {
        //            command.Connection = conn;
        //            reader = command.ExecuteReader();
        //            inflator.CheckOrdinals(reader);

        //            while (reader.Read())
        //            {
        //                Object newDO = inflator.CreateInstanceOfEntity();
        //                if (newDO is IDataObject)
        //                {
        //                    ((IDataObject)newDO).DAL = this;
        //                }
        //                inflator.ReadData(reader, newDO);
        //                dataList.Add((T)newDO);
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            throw this.ThrowExceptionHelper(conn, command, e);
        //        }
        //        finally
        //        {
        //            if (reader != null) { reader.Dispose(); }
        //            ReleaseConnection();
        //        }
        //        return dataList;
        //    }
        //}

        #endregion query methods
    }
}
