﻿using System;
using System.ComponentModel;
using System.Xml.Serialization;
using FMSC.ORM.Core;
using FMSC.ORM.Core.SQL;
using System.Diagnostics;
using FMSC.ORM.EntityModel.Attributes;
using FMSC.ORM.EntityModel.Support;

namespace FMSC.ORM.EntityModel
{
    [Flags]
    public enum RecordState //make protected inside DataObject?
    {
        Persisted = 1,
        HasChanges = 2,
        Detached = 4,
        Deleted = 8,
        Validated = 16
    }

    [Serializable]
    public abstract class DataObject_Base : 
        INotifyPropertyChanged, 
        ISupportInitialize,  
        IFormattable,
        IDataObject
    {
        protected DatastoreRedux _ds = null;
        protected RecordState _recordState = RecordState.Detached;

        bool _isDeleted;
        bool _isPersisted;
        bool _isChanged;

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

        
        #endregion


        #region ctor
        protected DataObject_Base()
        {

        }

        protected DataObject_Base(DatastoreRedux ds)
        {
            this.DAL = ds;
        }

        #endregion


        public virtual void Save()
        {
            this.Save(OnConflictOption.Default);
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
            IsPersisted = false;
            _isChanged = true;
        }

        internal void InternalSetDAL(DatastoreRedux newDAL)
        {
            _ds = newDAL;
        }

        

        public void SuspendEvents()
        {
            PropertyChangedEventsDisabled = true;
        }

        public void ResumeEvents()
        {
            PropertyChangedEventsDisabled = false;
        }


        #region INotifyPropertyChanged
        [XmlIgnore]
        [IgnoreField]
        public bool PropertyChangedEventsDisabled { get; protected set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string name)
        {
            if (!PropertyChangedEventsDisabled)
            {
                _isChanged = true;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
                }
            }
        }
        #endregion

        #region IPersistanceTracking Members
        [XmlIgnore]
        [IgnoreField]
        public bool IsPersisted
        {
            get { return _isPersisted; }
            protected set
            {
                if (_isPersisted == value) { return; }
                _isPersisted = value;
            }
        }

        [XmlIgnore]
        [IgnoreField]
        public bool IsDeleted
        {
            get { return _isDeleted; }
            protected set
            {
                if (_isDeleted == value) { return; }
                _isDeleted = value;
            }
        }

        public virtual void OnRead()
        {
            _isChanged = false;
            _isPersisted = true;
        }

        public virtual void OnInserting()
        { }

        public virtual void OnInserted()
        {
            _isChanged = false;
            _isPersisted = true;
        }

        public virtual void OnUpdating()
        { }

        public virtual void OnUpdated()
        {
            AcceptChanges();
        }

        public virtual void OnDeleting()
        { }

        public virtual void OnDeleted()
        {
            _isPersisted = false;
        }
        #endregion

        #region IChangeTracking 
        [XmlIgnore]
        [IgnoreField]
        [Obsolete("use IsChanged")]
        public bool HasChanges
        {
            get { return IsChanged; }
        }

        [XmlIgnore]
        [IgnoreField]
        public bool IsChanged
        {
            get { return _isChanged; }
            protected set
            {
                if (_isChanged == value) { return; }
                _isChanged = value;
            }
        }


        public virtual void AcceptChanges()
        {
            this._isChanged = false;
        }
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
            ICustomFormatter formatter;
            if (formatProvider == null)
            {
                var description = DatastoreRedux.LookUpEntityByType(this.GetType());
                formatter = new EntityFormatter(description);
            }
            else
            {
                formatter = formatProvider.GetFormat(this.GetType()) as ICustomFormatter;
            }

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
        