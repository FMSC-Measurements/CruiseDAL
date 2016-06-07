using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;
using FMSC.ORM.Core;
using FMSC.ORM.EntityModel;
using FMSC.ORM.EntityModel.Attributes;

namespace CruiseDAL
{
    public abstract class DataObject : DataObject_Base, IDataErrorInfo, IValidatable, INotifyDataErrorInfo
    {
        protected DataObject()
            : this(null)
        { }

        protected DataObject(DatastoreRedux ds)
            : base(ds)
        {
            string tableName = DatastoreRedux.LookUpEntityByType(this.GetType()).SourceName;
            ErrorCollection = new CruiseDALErrorCollection(this, tableName);
        }

        internal CruiseDALErrorCollection ErrorCollection;

        //protected Dictionary<String, ErrorLogDO> errors;
        //protected object _errorsSyncLock = new object();
        //protected bool _errorsLoaded = false;

        [XmlIgnore]
        [IgnoreField]
        public Int64? rowID
        {
            get;
            set;
        }

        //[XmlIgnore]
        //[IgnoreField]
        //public object PrimaryKey
        //{
        //    get; set;
        //}

        /// <summary>
        /// Property returns an instance of its self. Useful when using data binding
        /// </summary>
        [XmlIgnore]
        [IgnoreField]
        public DataObject_Base Self
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Tag allows any supplemental object to
        /// be attached to a data object.
        /// </summary>
        [XmlIgnore]
        [IgnoreField]
        public Object Tag { get; set; }

        public abstract void SetValues(DataObject obj);

        protected override void NotifyPropertyChanged(string name)
        {
            base.NotifyPropertyChanged(name);
            if (!PropertyChangedEventsDisabled)
            {
                IsValidated = false;
            }
        }

        #region validation

        //requires Net45 or higher
        //event EventHandler<DataErrorsChangedEventArgs> INotifyDataErrorInfo.ErrorsChanged
        //{
        //    add
        //    {
        //        this.ErrorCollection.ErrorsChanged += value;
        //    }

        //    remove
        //    {
        //        this.ErrorCollection.ErrorsChanged -= value;
        //    }
        //}
        //bool INotifyDataErrorInfo.HasErrors
        //{
        //    get
        //    {
        //        return ErrorCollection.HasErrors;
        //    }
        //}
        //IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
        //{
        //    return this.ErrorCollection.GetErrors(propertyName);
        //}

        [XmlIgnore]
        [IgnoreField]
        public abstract RowValidator Validator
        {
            get;
        }

        [XmlIgnore]
        [IgnoreField]
        public bool IsValidated
        {
            get { return (this._recordState & RecordState.Validated) == RecordState.Validated; }
            internal set
            {
                if (value == true)
                {
                    this._recordState = this._recordState | RecordState.Validated;
                }
                else
                {
                    this._recordState = this._recordState & ~RecordState.Validated;
                }
            }
        }

        #region IDataErrorInfo Members

        string IDataErrorInfo.Error
        {
            get
            {
                return this.ErrorCollection.GetErrors();
            }
        }

        public string Error
        {
            get
            {
                return ((IDataErrorInfo)this).Error;

                //lock (this._errorsSyncLock)
                //{
                //    if (this.HasErrors() == false) { return String.Empty; }
                //    StringBuilder b = new StringBuilder();
                //    foreach (ErrorLogDO e in errors.Values)
                //    {
                //        if (e.Suppress == false)
                //        {
                //            b.Append(e.Message + "; ");
                //        }
                //    }
                //    return b.ToString();
                //}
            }
        }

        /// <summary>
        /// Explicitly implemented accessors, for retrieving error info by field Name
        /// Note:*** to use this accessors you must cast DataObject to IDataErrorInfo first ***
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                var sb = new StringBuilder();
                foreach (string s in ErrorCollection.GetErrors(columnName))
                {
                    sb.Append(s + ";");
                }

                return sb.ToString();

                //lock (this._errorsSyncLock)
                //{
                //    if (this.HasErrors() == false) { return string.Empty; }
                //    if (!errors.ContainsKey(columnName)) { return string.Empty; }
                //    ErrorLogDO e = errors[columnName];
                //    if (e.Suppress == false)
                //    {
                //        return errors[columnName].Message;
                //    }
                //    else
                //    {
                //        return string.Empty;
                //    }
                //}
            }
        }

        #endregion IDataErrorInfo Members

        public void SaveErrors()
        {
            this.ErrorCollection.SaveErrors();

            //if (this.rowID == null)
            //{
            //    if (this.DAL == null) { return; }
            //    this.Save();
            //}
            //lock (this._errorsSyncLock)
            //{
            //    if (this.errors == null) { return; }
            //    foreach (ErrorLogDO e in errors.Values)
            //    {
            //        if (e.DAL == null || e.DAL != this.DAL)
            //        {
            //            e.DAL = (DAL)this.DAL;
            //        }

            //        e.CN_Number = this.rowID.Value;
            //        e.Save(OnConflictOption.Replace);
            //    }
            //}
        }

        public void AddError(string propName, string error)
        {
            this.ErrorCollection.AddError(propName, error);

            //ErrorLogDO e = new ErrorLogDO();
            //if (error.StartsWith("Warning::"))
            //{
            //    e.Level = "W";
            //}
            //else
            //{
            //    e.Level = "E";
            //}

            //e.TableName = DatastoreBase.GetObjectDiscription(this.GetType()).ReadSource;
            //e.ColumnName = propName;
            //e.Message = error;
            //e.Program = AppDomain.CurrentDomain.FriendlyName;
            //AddError(propName, e);
        }

        //private void AddError(string propName, ErrorLogDO e)
        //{
        //    lock (this._errorsSyncLock)
        //    {
        //        if (this.errors == null)
        //        {
        //            this.errors = new Dictionary<string, ErrorLogDO>();
        //        }

        //        if (errors.ContainsKey(propName) == true)
        //        {
        //            ErrorLogDO e1 = errors[propName];
        //            if (e1.Level[0] != e.Level[0] && e1.Suppress == true)
        //            {
        //                errors[propName] = e;
        //            }
        //            else if (e1.Message == e.Message && (e.Suppress == true && e1.Suppress == false))
        //            {
        //                if (e1.IsPersisted)
        //                {
        //                    e1.Delete();
        //                }
        //                errors[propName] = e;
        //            }
        //            else if (e1.Level.StartsWith("W") && e.Level.StartsWith("E"))
        //            {
        //                errors[propName] = e;
        //            }

        //        }
        //        else
        //        {
        //            errors[propName] = e;
        //        }
        //    }
        //}

        public void RemoveError(string propName, string error)
        {
            this.ErrorCollection.RemoveError(propName, error);

            //lock (this._errorsSyncLock)
            //{
            //    if (errors == null) { return; }
            //    if (errors.ContainsKey(propName) == true)
            //    {
            //        //if (errors[propName] != error) { throw new InvalidOperationException(); }
            //        ErrorLogDO e = errors[propName];
            //        if (e.Message == error)
            //        {
            //            if (e.Suppress == true) { return; }

            //            this.errors.Remove(propName);
            //            if (e.IsPersisted)
            //            {
            //                e.Delete();
            //            }
            //        }
            //    }
            //    if (this.errors.Count == 0)
            //    {
            //        this.errors = null;
            //    }
            //}
        }

        //internal void ClearErrors()
        //{
        //    lock (this._errorsSyncLock)
        //    {
        //        if (this.HasErrors() == false) { return; }
        //        List<string> keysToRemove = new List<string>();
        //        foreach (KeyValuePair<string, ErrorLogDO> kv in this.errors)
        //        {
        //            ErrorLogDO e = kv.Value;
        //            if (e != null)
        //            {
        //                if (e.Suppress == true) { continue; }
        //                if (e.IsPersisted)
        //                {
        //                    e.Delete();
        //                }
        //            }
        //            keysToRemove.Add(kv.Key);

        //        }

        //        foreach (string k in keysToRemove)
        //        {
        //            this.errors.Remove(k);
        //        }
        //    }
        //}

        //internal void ClearWarnings()
        //{
        //    lock (this._errorsSyncLock)
        //    {
        //        if (this.HasErrors() == false) { return; }
        //        List<string> keysToRemove = new List<string>();
        //        foreach (KeyValuePair<string, ErrorLogDO> kv in this.errors)
        //        {
        //            ErrorLogDO e = kv.Value;
        //            if (e != null)
        //            {
        //                if (e.Suppress == true) { continue; }
        //                if (e.Level == "E") { continue; }
        //                if (e.IsPersisted)
        //                {
        //                    e.Delete();
        //                }
        //            }
        //            keysToRemove.Add(kv.Key);

        //        }

        //        foreach (string k in keysToRemove)
        //        {
        //            this.errors.Remove(k);
        //        }
        //    }
        //}

        //internal void ClearErrors(string level)
        //{
        //    if (this.errors.Count == 0) { return; }
        //    List<string> keyList = new List<string>();
        //    foreach (KeyValuePair<string, ErrorLogDO> kv in errors)
        //    {
        //        if (kv.Value.Suppress == true) { continue; }
        //        if (level != null && kv.Value.Level.StartsWith(level)) { continue; }
        //        if (kv.Value.IsPersisted)
        //        {
        //            kv.Value.Delete();
        //        }
        //        keyList.Add(kv.Key);

        //    }
        //    foreach (string key in keyList)
        //    {
        //        errors.Remove(key);
        //    }
        //}

        [Obsolete]
        public bool HasErrors(string propName)
        {
            return this.ErrorCollection.HasError(propName);

            //if (HasErrors() == false) { return false; }
            ////if (this.errors == null) { return false; }
            //return this.errors.ContainsKey(propName) && this.errors[propName].Suppress == false;
        }

        public bool HasErrors()
        {
            return this.ErrorCollection.HasErrors;

            //if (this.errors == null) { return false; }
            //return errors.Count > 0;
        }

        [Obsolete]
        public String GetError(string fieldName)
        {
            return String.Concat(this.ErrorCollection.GetErrors(fieldName));

            //return (this as IDataErrorInfo)[fieldName];
        }

        public void PurgeErrorList()
        {
            this.ErrorCollection.PurgeErrorList();
            //lock (_errorsSyncLock)
            //{
            //    if (this.errors != null)
            //    {
            //        this.errors.Clear();
            //    }
            //    this.IsValidated = false;
            //    this._errorsLoaded = false;
            //}
        }

        //protected void PopulateErrorList()
        //{
        //    if (this.rowID == null) { return; }
        //    string tableName = DatastoreBase.GetObjectDiscription(this.GetType()).ReadSource;
        //    List<ErrorLogDO> errorList = this.DAL.Read<ErrorLogDO>("ErrorLog", "WHERE TableName = ? AND CN_Number = ?", tableName, this.rowID);
        //    foreach (ErrorLogDO e in errorList)
        //    {
        //        this.AddError(e.ColumnName, e);
        //    }
        //    this._errorsLoaded = true;
        //}

        protected abstract bool DoValidate();

        public virtual bool Validate()
        {
            if (!this.IsValidated)
            {
                bool isValid = DoValidate();
                this.IsValidated = true;

                return isValid;
            }
            else
            {
                return !this.ErrorCollection.HasErrors;
            }
        }

        public virtual bool Validate(IEnumerable<String> fields)
        {
            bool isValid = true;
            foreach (String f in fields)
            {
                isValid = this.ValidateProperty(f) && isValid;
            }
            return isValid;
        }

        public virtual bool ValidateProperty(string name)
        {
            if (PropertyChangedEventsDisabled) { return true; }
            object value = null;
            try
            {
                value = DatastoreRedux.LookUpEntityByType(this.GetType())
                    .Properties[name].GetValue(this);
                //PropertyInfo property = this.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
                ////if (property == null) { return true; }
                //value = property.GetValue(this, null);
            }
            catch
            {
                return true;
            }
            return this.ValidateProperty(name, value);
        }

        protected virtual bool ValidateProperty(string name, object value)
        {
            if (PropertyChangedEventsDisabled) { return true; }
            if (Validator != null) { return Validator.Validate(this, name, value); }
            else { return true; }
        }

        #endregion validation

        bool INotifyDataErrorInfo.HasErrors { get { return this.ErrorCollection.HasErrors; } }

        public IEnumerable GetErrors(String propertyName)
        {
            yield return String.Concat(this.ErrorCollection.GetErrors(propertyName));
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
        {
            add
            {
                lock (ErrorCollection)
                {
                    ErrorCollection.ErrorsChanged += value;
                }
            }

            remove
            {
                lock (ErrorCollection)
                {
                    ErrorCollection.ErrorsChanged -= value;
                }
            }
        }
    }
}