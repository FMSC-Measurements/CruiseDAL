using FMSC.ORM.Core.SQL;
using FMSC.ORM.EntityModel;
using Backpack.SqlBuilder;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace FMSC.ORM.Core
{
    public partial class DatastoreRedux
    {
        public object Insert(object data)
        { return Insert(data, (object)null, OnConflictOption.Default); }

        public object Insert(object data, object keyData)
        { return Insert(data, keyData, OnConflictOption.Default); }

        public void Update(object data)
        { Update(data, OnConflictOption.Default); }

        public void Save(IPersistanceTracking data)
        { Save(data, OnConflictOption.Default, true); }

        public void Save(IPersistanceTracking data, OnConflictOption option)
        { Save(data, option, true); }

        public void Save(DbConnection connection, IPersistanceTracking data, DbTransaction transaction)
        { Save(connection, data, transaction, OnConflictOption.Default, true); }

        public void CreateTable(string tableName, IEnumerable<ColumnInfo> cols)
        {
            CreateTable(tableName, cols, false);
        }

        public void CreateTable(DbConnection connection, string tableName, IEnumerable<ColumnInfo> cols)
        {
            CreateTable(connection, tableName, cols, false, (DbTransaction)null);
        }

        public void CreateTable(DbConnection connection, string tableName, IEnumerable<ColumnInfo> cols, bool isTemp)
        {
            CreateTable(connection, tableName, cols, isTemp, (DbTransaction)null);
        }
    }
}