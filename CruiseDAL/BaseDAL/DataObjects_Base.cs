using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Reflection;
using CruiseDAL.DataObjects;
using System.Text;
using CruiseDAL.BaseDAL;

namespace CruiseDAL
{
    public enum RecordState
    {
        Persisted = 1,
        HasChanges = 2,
        Detached = 4,
        Deleted = 8,
        Validated = 16
    }

    [Serializable]
    public abstract class DataObject : 
        INotifyPropertyChanged, 
        ISupportInitialize,  
        
        IFormattable
    {
        #region Fields 
        #region protected fields 
        protected DatastoreBase _ds = null;
        protected Dictionary<String, ErrorLogDO> errors;
        protected object _errorsSyncLock = new object();
        #endregion 
        #endregion

        #region Properties

        #region persistance utility properties

        [XmlIgnore]
        public Int64? rowID
        {
            get;
            set;
        }

        [XmlIgnore]
        public DatastoreBase DAL
        {
            get
            {
                return _ds;
            }
            set
            {
                if (_ds == value) { return; }
                if (_ds != null)
                {
                    _ds._IDTable.Remove(this);
                }
                InternalSetDAL(value);
                OnDALChanged(value);
            }
        }


        //public abstract Persister Persister { get; }
        protected RecordState _recordState = RecordState.Detached;
        //protected bool inWriteMode = false;
        //protected bool _hasChanges = false;

        [XmlIgnore]
        internal RecordState RecordState
        {
            get { return _recordState; }
            set { this._recordState = value; }
        }

        [XmlIgnore]
        public bool HasChanges 
        {
            get { return (this._recordState & RecordState.HasChanges) == RecordState.HasChanges; }
            internal set 
            {
                if (value == true)
                {
                    this._recordState = _recordState | RecordState.HasChanges;
                }
                else
                {
                    this._recordState = _recordState & ~RecordState.HasChanges;
                }
            }
        }

        [XmlIgnore]
        public bool IsPersisted 
        {
            get { return (this._recordState & RecordState.Persisted) == RecordState.Persisted; }
            internal set
            {
                if (value == true)
                {
                    this._recordState = (_recordState & RecordState.Validated) | RecordState.Persisted;//override all other states except Validated
                }
                else
                {
                    this._recordState = _recordState & ~RecordState.Persisted;
                }
            }
        }

        [XmlIgnore]
        public bool IsDetached
        {
            get { return (this._recordState & RecordState.Detached) == RecordState.Detached; }
            internal set
            {
                if (value == true)
                {
                    this._recordState = RecordState.Detached;//override all other states 
                }
                else
                {
                    this._recordState = _recordState & ~RecordState.Detached;
                }
            }
        }

        [XmlIgnore]
        public bool IsDeleted
        {
            get { return (this._recordState & RecordState.Deleted) == RecordState.Deleted; }
            internal set
            {
                if (value == true)
                {
                    this._recordState = RecordState.Deleted;//override all other states
                }
                else
                {
                    this._recordState = _recordState & ~RecordState.Deleted;
                }
            }

        }

        
        #endregion

       

        

        

        //private RowValidator _validator;
        //[XmlIgnore]
        //public RowValidator Validator
        //{
        //    get
        //    {

        //        return _validator;
        //    }
        //    set
        //    {
        //        errors.Clear();
        //        _validator = value;
        //    }
        //}
        #endregion

        #region Events 

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region ctor
        public DataObject()
        {

        }

        public DataObject(DatastoreBase ds)
        {
            this.DAL = ds;
        }

        #endregion

       

        public virtual void Save()
        {

            this.Save(OnConflictOption.Fail);
//            if (DAL == null) 
//            {
//                throw new InvalidOperationException("DAL must be set before calling save");
//            }
//            if (HasChanges == false) { return; }
//#if !WindowsCE
//            Trace.Write(string.Format("Saving DO | {0:20} | {1:15} | IsPersisted = {2}\r\n", this.GetType().Name, this.GetID(), this.IsPersisted), "Info");
//#endif
//            //Log.V(string.Format("Saving DO | {0:20} | {1:15} | IsPersisted = {2}", this.GetType().Name, this.GetID(), this.IsPersisted));
//            if (IsPersisted)
//            {
//                DAL.ExecuteSQL(Persister.CreateSQLUpdate(this, DAL.User));
//            }
//            else
//            {
//                rowID = DAL.ExecuteScalar(Persister.CreateSQLInsert(this, DAL.User));
//                IsPersisted = true;
//            }
//            HasChanges = false;
        }

        public virtual void Save(OnConflictOption option)
        {
            if (DAL == null)
            {
                throw new InvalidOperationException("DAL must be set before calling save");
            }

            this.DAL.Save(this, option);
            //if (HasChanges == false) { return; }

            //Logger.Log.V(string.Format("Saving DO | {0:20} | {1:15} | IsPersisted = {2}\r\n", this.GetType().Name, this.GetID(), this.IsPersisted));

            //if (IsPersisted)
            //{
            //    DAL.ExecuteSQL(ObjectDescription.CreateSQLUpdate(this, DAL.User, option));
            //}
            //else
            //{
            //    rowID = DAL.ExecuteScalar(ObjectDescription.CreateSQLInsert(this, DAL.User, option));
            //    IsPersisted = true;
            //}
            //HasChanges = false;

            


        }

        public virtual void Delete()
        {
            Debug.Assert(DAL != null);
            DAL.Delete(this);

            //if (IsPersisted)
            //{
            //    Logger.Log.V(string.Format("Deleting DO | {0:20} | {1:15} | IsPersisted = {2}\r\n", this.GetType().Name, this.GetID(), this.IsPersisted));

            //    DAL.ExecuteSQL(ObjectDescription.CreateSQLDelete(this));
            //    DAL._IDTable.Remove(this);

            //    this.IsDeleted = true;
            //}
        }

        protected virtual void OnDALChanged(DatastoreBase newDAL)
        {


        }

        internal void InternalSetDAL(DatastoreBase newDAL)
        {
            IsPersisted = false;
            HasChanges = true;

            if (newDAL == null)
            {
                IsDetached = true;
            }
            else
            {
                IsDetached = false;
            }
            _ds = newDAL;
        }

        public abstract void SetValues(DataObject obj);

        /// <summary>
        /// Disables Property Changed events from fireing. 
        /// This is useful when changing many propertys on an object 
        /// and you need to protect against events fireing during the write process. 
        /// </summary>
        [Obsolete("Use SuspendEvents instead")]
        public void StartWrite()
        {
            this.PropertyChangedEventsDisabled = true;
        }

        /// <summary>
        /// Re-enables property changed events
        /// </summary>
        [Obsolete("Use RsumeEvents instead")]
        public void EndWrite()
        {
            this.PropertyChangedEventsDisabled = false;
        }

        /// <summary>
        /// Returns the object id
        /// </summary>
        /// <returns></returns>
        public long GetID()
        {
            if (DAL != null && rowID != null)
            {
                return DatastoreBase.GetObjectDiscription(this.GetType()).GetID(this.rowID);
            }
            else
            {
                return -1;
            }
        }

        

        
        

        //protected void NotifyPropertyChanged(string name, object value)
        //{
        //    NotifyPropertyChanged(name);
        //}

        protected virtual void NotifyPropertyChanged(string name)
        {
            if (!PropertyChangedEventsDisabled)
            {
                HasChanges = true;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
                }
            }
        }

        [XmlIgnore]
        public bool PropertyChangedEventsDisabled { get; protected set; }

        public void SuspendEvents()
        {
            PropertyChangedEventsDisabled = true;
        }

        public void ResumeEvents()
        {
            PropertyChangedEventsDisabled = false;
        }


        #region ISupportInitialize Members
        public void BeginInit()
        {
            this.SuspendEvents();
        }

        public void EndInit()
        {
            this.ResumeEvents();
        }
        #endregion

        #region IFormattable Members
        /// <summary>
        /// replaces placeholders with property values in format string. 
        /// [propertyName], [propertyName:nullValue], [propertyName:nullValue:pad], [propertyName::pad] 
        /// pad option can include prefix U | L | C . for Uppercase, lowercase, capitalize
        /// </summary>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            ICustomFormatter formatter = formatProvider.GetFormat(this.GetType()) as ICustomFormatter;
            if(formatter != null)
            {
                return formatter.Format(format, this, formatProvider);
            }
            else
            {
                return base.ToString();
            }
        }
        #endregion


    }
}
        

    //blueprint for data objects
    //public class SaleObject : DataObject
    //{
    //    public int? Sale_CN {
    //        get { return base.rowID; }
    //        set { base.rowID = value; } 
    //    }

    //    public string SaleNumber { get; set; }
    //    public string Name { get; set; } 
    //    public string Purpose { get; set; }
    //    public string Region { get; set; }
    //    public string Forest { get; set; }
    //    public string District { get; set; }


    //    public SaleObject()
    //    {
    //        base.persister = new SalePersister();
    //    }

    //    public SaleObject(SaleObject sale) : this()
    //    {
    //        SetValues(sale);
    //    }

    //    public SaleObject(DAL DAL) : this()
    //    {
    //        this.DAL = DAL;
    //    }

    //    public void SetValues(SaleObject sale)
    //    {
    //        SaleNumber = sale.SaleNumber;
    //        Name = sale.Name;
    //        Purpose = sale.Purpose;
    //    }

    //    public static long GetID(string saleNumber)
    //    {
    //        return (typeof(SaleObject).GetHashCode() % 1000) * 1000 + (saleNumber.GetHashCode() % 1000);
    //    }

    //    public override long GetID()
    //    {
    //        return GetID(SaleNumber);
    //    }
    //}

    //blueprint for dataObject with forgen keys 
    //public partial class CruiseObject : DataObject
    //{
    //    public CruiseObject()
    //    {
    //        base.persister = new CruisePersister();
    //    }
    //    public CruiseObject(CruiseObject Cruise) : this()
    //    {
    //        SetValues(Cruise);
    //    }
    //    public CruiseObject(DAL DAL) : this()
    //    {
    //        this.DAL = DAL;
    //    }

    //    private SaleObject mySale = null;
    //    public SaleObject Sale
    //    {
    //        get
    //        {
    //            if (mySale == null)
    //            {
    //                mySale = GetSale();
    //            }
    //            return mySale;
    //        }
    //        set
    //        {

    //            if (value.rowID != null)
    //            {
    //                Sale_CN = value.Sale_CN;
    //                mySale = value;
    //            }
    //            else
    //            {
    //                throw new Exception("can not set forgen row to a dataobject that has not been persisted");
    //            }
    //            
    //        }
    //    }

    //    public Int64? Cruise_CN
    //    {
    //        get { return base.rowID; }
    //        set { base.rowID = (Int64)value; }
    //    }


    //    //forgenkeys
    //    private Int64? mySale_CN = null;
    //    public Int64? Sale_CN {
    //        get
    //        {
    //            return mySale_CN;
    //        }
    //        set
    //        {
    //            mySale_CN = value;
    //            mySale = null;
    //        }
    //    }

    //}
