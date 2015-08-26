using System;
using System.Collections.Generic;
using System.Text;
using Logger;
using System.Collections; 

namespace CruiseDAL
{
    public class ObjectCache : IEnumerable
    {
        public enum AddBehavior { OVERWRITE, DONT_OVERWRITE, THROWEXCEPTION };

        protected Dictionary<long, WeakReference> idDict = null;

        protected DataObjectFactory DOFactory { get; set; }
        public int Count
        {
            get { return idDict.Count; }
        }

        public ObjectCache(DataObjectFactory DOFactory)
        {
            if (DOFactory == null)
            {
                throw new ArgumentNullException("DOFactory");
            }

            this.DOFactory = DOFactory;
            idDict = new Dictionary<long, WeakReference>();
        }

        public DataObject GetByID(long id)
        {
            WeakReference wkRef = InternalGetByID(id);
            if(wkRef != null && wkRef.IsAlive)
            {
                return (DataObject)wkRef.Target;
            }
            else
            {
                return null;
            }
        }

        private WeakReference InternalGetByID(long id)
        {
            if (idDict.ContainsKey(id))
            {
                WeakReference wkRef = idDict[id];
                return wkRef;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Retrives a Object from the cache by ID, If an object doesn't exist 
        /// with the given ID, it creates a new object using the type provided 
        /// by defaultType and returns the new object.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="defaultType"></param>
        /// <returns></returns>
        public DataObject GetByID<defaultType>(long id) where defaultType : DataObject, new()
        {
            lock (this.idDict)
            {
                WeakReference wkRef = InternalGetByID(id);
                DataObject obj = null;
                if (wkRef != null && wkRef.IsAlive)
                {
                    obj = (DataObject)wkRef.Target;
                    if (!(obj is defaultType))
                    {
                        return DOFactory.GetNew<defaultType>();
                    }
                }
                else
                {
                    Log.V(string.Format("New DO Created | {0:20} | {1}", typeof(defaultType).Name, id));
                    obj = DOFactory.GetNew<defaultType>();
                    if (wkRef != null)
                    {
                        wkRef.Target = obj;
                    }
                    else
                    {
                        idDict.Add(id, new WeakReference(obj));
                    }
                }

                return obj;
            }
        }


        public void Flush()
        {
            lock (idDict)
            {
                foreach (WeakReference obj in idDict.Values)
                {
                    if (obj.IsAlive)
                    {
                        ((DataObject)obj.Target).InternalSetDAL(null);
                    }
                }

                this.idDict.Clear();
            }
        }

        public bool Remove(DataObject dataObject)
        {
            lock (idDict)
            {
                return idDict.Remove(dataObject.GetID());
            }
       
        }

        //public bool Update(long oldID, DataObject dataObject)
        //{
        //    //TODO change throwexception to dont overwrite when not debugging 
        //    if (Add(dataObject, AddBehavior.THROWEXCEPTION))
        //    {
        //        idDict.Remove(oldID);
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}


        public bool Add(DataObject dataObject, AddBehavior addBehavior)
        {
            
            long ID = dataObject.GetID();
            WeakReference wkRef = new WeakReference(dataObject);
            if (idDict.ContainsKey(ID) && idDict[ID].IsAlive)
            {
                switch (addBehavior)
                {
                    case AddBehavior.OVERWRITE:
                        {
                            idDict[ID] = wkRef;
                            return true;
                        }
                    case AddBehavior.THROWEXCEPTION:
                        {
                            throw new Exception(string.Format("ID conflict (ID:{0}, Existing Data:{1})", ID, idDict[ID]));
                        }
                    case AddBehavior.DONT_OVERWRITE:
                        {
                            return false;
                        }
                    default:
                        {
                            throw new ArgumentException("AddBehavior not supported");
                        }
                }
            }
            else
            {
                idDict[ID] = wkRef;
                return true;
            }
        }







        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.idDict.Values.GetEnumerator();
        }

        #endregion
    }
}
