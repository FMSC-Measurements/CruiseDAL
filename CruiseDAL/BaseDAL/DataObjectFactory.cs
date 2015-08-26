
using System;
using CruiseDAL.DataObjects;

namespace CruiseDAL

{
	public class DataObjectFactory
	{
		public DatastoreBase Datastore { get; set; }
		public DataObjectFactory(DatastoreBase ds)
		{
            this.Datastore = ds;
		}


		 public DataObject GetNew<J>() where J : DataObject, new() 
		{
            DataObject obj = new J();
            obj.DAL = Datastore;
            return obj;
        }
    }
}
