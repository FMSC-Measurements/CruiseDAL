using System;
using System.ComponentModel;
using System.Xml.Serialization;
using FMSC.ORM.Core;
using FMSC.ORM.Core.SQL;
using System.Diagnostics;
using FMSC.ORM.Core.EntityAttributes;

namespace FMSC.ORM.Core.EntityModel
{
    public enum RecordState //make protected inside DataObject?
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
        IFormattable,
        IDataObject,
        IPersistanceTracking
    {
        protected DatastoreRedux _ds = null;
        protected RecordState _recordState = RecordState.Detached;

        #region Properties

        [XmlIgnore]
        [IgnoreField]
        public DatastoreRedux DAL
        {
            get
            {
                return _ds;
            }
            set
            {
                if (_ds == value) { return; }
                //if (_ds != null)
                //{
                //    _ds._IDTable.Remove(this);
                //}
                InternalSetDAL(value);
                OnDALChanged(value);
            }
        }

        [XmlIgnore]
        [IgnoreField]
        public bool IsPersisted
        {
            get { return (this._recordState & RecordState.Persisted) == RecordState.Persisted; }
        }

        [XmlIgnore]
        [IgnoreField]
        public bool HasChanges
        {
            get { return (this._recordState & RecordState.HasChanges) == RecordState.HasChanges; }
        }

        [XmlIgnore]
        [IgnoreField]
        public bool IsDeleted
        {
            get { return (this._recordState & RecordState.Deleted) == RecordState.Deleted; }
        }

        [XmlIgnore]
        [IgnoreField]
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
        [IgnoreField]
        public bool PropertyChangedEventsDisabled { get; protected set; }


        #endregion


        #region Events 

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region ctor
        public DataObject()
        {

        }

        public DataObject(DatastoreRedux ds)
        {
            this.DAL = ds;
        }

        #endregion


        public virtual void Save()
        {
            this.Save(OnConflictOption.Fail);
        }

        public virtual void Save(OnConflictOption option)
        {
            Debug.Assert(DAL != null);
            this.DAL.Save(this, option);
        }

        public virtual void Delete()
        {
            Debug.Assert(DAL != null);
            DAL.Delete(this);
        }


        protected virtual void OnDALChanged(DatastoreRedux newDAL)
        {


        }

        internal void InternalSetDAL(DatastoreRedux newDAL)
        {
            SetIsPersisted(false);
            SetHasChanges(true);
            
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

        public void SuspendEvents()
        {
            PropertyChangedEventsDisabled = true;
        }

        public void ResumeEvents()
        {
            PropertyChangedEventsDisabled = false;
        }

        protected virtual void NotifyPropertyChanged(string name)
        {
            if (!PropertyChangedEventsDisabled)
            {
                SetHasChanges(true);
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
                }
            }
        }

        


        #region IPersistanceTracking Members
        bool IPersistanceTracking.IsPersisted
        {
            get
            {
                return this.IsPersisted;
            }

            set
            {
                SetIsPersisted(value);
            }
        }

        bool IPersistanceTracking.HasChanges
        {
            get
            {
                return this.HasChanges;
            }

            set
            {
                SetHasChanges(value);
            }
        }

        bool IPersistanceTracking.IsDeleted
        {
            get
            {
                return this.IsDeleted;
            }
            set
            {
                SetIsDeleted(value);
            }
        }

        protected void SetIsPersisted(bool value)
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

        protected void SetHasChanges(bool value)
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

        protected void SetIsDeleted(bool value)
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

        void IPersistanceTracking.OnInserted()
        {
            OnInserted();
        }

        void IPersistanceTracking.OnUpdating()
        {
            OnUpdating();
        }

        void IPersistanceTracking.OnUpdated()
        {
            OnUpdated();
        }

        void IPersistanceTracking.OnDeleting()
        {
            OnDeleting();
        }

        void IPersistanceTracking.OnDeleted()
        {
            OnDeleted();
        }

        protected virtual void OnInserted()
        { }

        protected virtual void OnUpdating()
        { }

        protected virtual void OnUpdated()
        { }

        protected virtual void OnDeleting()
        { }
        protected virtual void OnDeleted()
        { }
        #endregion

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
