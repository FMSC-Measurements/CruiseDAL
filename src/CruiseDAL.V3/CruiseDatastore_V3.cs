﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using FMSC.ORM.Core;

namespace CruiseDAL
{
    public class CruiseDatastore_V3 : CruiseDatastore
    {
        public CruiseDatastore_V3()
            : this(IN_MEMORY_DB_PATH, false)
        { }

        public CruiseDatastore_V3(string path)
            : this(path, false)
        { }

        public CruiseDatastore_V3(string path, bool makeNew) 
            : base(path, makeNew, new CruiseDatastoreBuilder_V3(), new Updater_V3())
        {
        }

        protected override bool IsExtentionValid(string path)
        {
            var extension = System.IO.Path.GetExtension(path).ToLower();
            return extension == ".crz3" || extension == ".crz3t";
        }

        protected override void OnConnectionOpened(DbConnection connection)
        {
            base.OnConnectionOpened(connection);

#if SYSTEM_DATA_SQLITE
            connection.ExecuteNonQuery("PRAGMA foreign_keys=on;", exceptionProcessor: ExceptionProcessor);
#endif
        }
    }
}
