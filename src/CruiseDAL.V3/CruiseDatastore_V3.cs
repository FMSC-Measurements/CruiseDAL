using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            return extension == ".crz3" || extension == ".crzt3";
        }
    }
}
