using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

using CruiseDAL.Core.EntityModel;
using CruiseDAL.DataObjects;

namespace CruiseDAL.BaseDAL
{
    public abstract class DataObjectExtra : DataObject, IDataErrorInfo, IValidatable
    {
        public DataObjectExtra() : base()
        { }


        public DataObjectExtra(DatastoreBase ds) : base(ds)
        { }


        protected bool _errorsLoaded = false;


        /// <summary>
        /// Property returns the dataObject referenced. Useful when using data binding 
        /// </summary>
        [XmlIgnore]
        public DataObject Self
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Tag allows any supplemental object to 
        /// be attached to a dataobject. 
        /// </summary>
        [XmlIgnore]
        public Object Tag { get; set; }

        [XmlIgnore]
        public abstract RowValidator Validator
        {
            get;
        }

        [XmlIgnore]
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

        #region validation
        #region IDataErrorInfo Members


        public string Error
        {
            get
            {
                lock (this._errorsSyncLock)
                {
                    if (this.HasErrors() == false) { return String.Empty; }
                    StringBuilder b = new StringBuilder();
                    foreach (ErrorLogDO e in errors.Values)
                    {
                        if (e.Suppress == false)
                        {
                            b.Append(e.Message + "; ");
                        }
                    }
                    return b.ToString();
                }

            }
        }

        /// <summary>
        /// Explicitly implemented accessor, for retrieving error info by field Name
        /// Note:*** to use this accessor you must cast DataObject to IDataErrorInfo first ***
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                lock (this._errorsSyncLock)
                {
                    if (this.HasErrors() == false) { return string.Empty; }
                    if (!errors.ContainsKey(columnName)) { return string.Empty; }
                    ErrorLogDO e = errors[columnName];
                    if (e.Suppress == false)
                    {
                        return errors[columnName].Message;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
        }

        #endregion

        public void SaveErrors()
        {
            if (this.rowID == null)
            {
                if (this.DAL == null) { return; }
                this.Save();
            }
            lock (this._errorsSyncLock)
            {
                if (this.errors == null) { return; }
                foreach (ErrorLogDO e in errors.Values)
                {
                    if (e.DAL == null || e.DAL != this.DAL)
                    {
                        e.DAL = (DAL)this.DAL;
                    }

                    e.CN_Number = this.rowID.Value;
                    e.Save(OnConflictOption.Replace);
                }
            }

        }

        public void AddError(string propName, string error)
        {
            ErrorLogDO e = new ErrorLogDO();
            if (error.StartsWith("Warning::"))
            {
                e.Level = "W";
            }
            else
            {
                e.Level = "E";
            }

            e.TableName = DatastoreBase.GetObjectDiscription(this.GetType()).ReadSource;
            e.ColumnName = propName;
            e.Message = error;
            e.Program = AppDomain.CurrentDomain.FriendlyName;
            AddError(propName, e);
        }

        private void AddError(string propName, ErrorLogDO e)
        {
            lock (this._errorsSyncLock)
            {
                if (this.errors == null)
                {
                    this.errors = new Dictionary<string, ErrorLogDO>();
                }

                if (errors.ContainsKey(propName) == true)
                {
                    ErrorLogDO e1 = errors[propName];
                    if (e1.Level[0] != e.Level[0] && e1.Suppress == true)
                    {
                        errors[propName] = e;
                    }
                    else if (e1.Message == e.Message && (e.Suppress == true && e1.Suppress == false))
                    {
                        if (e1.IsPersisted)
                        {
                            e1.Delete();
                        }
                        errors[propName] = e;
                    }
                    else if (e1.Level.StartsWith("W") && e.Level.StartsWith("E"))
                    {
                        errors[propName] = e;
                    }

                }
                else
                {
                    errors[propName] = e;
                }
            }
        }

        public void RemoveError(string propName, string error)
        {
            lock (this._errorsSyncLock)
            {
                if (errors == null) { return; }
                if (errors.ContainsKey(propName) == true)
                {
                    //if (errors[propName] != error) { throw new InvalidOperationException(); }
                    ErrorLogDO e = errors[propName];
                    if (e.Message == error)
                    {
                        if (e.Suppress == true) { return; }

                        this.errors.Remove(propName);
                        if (e.IsPersisted)
                        {
                            e.Delete();
                        }
                    }
                }
                if (this.errors.Count == 0)
                {
                    this.errors = null;
                }
            }
        }



        internal void ClearErrors()
        {
            lock (this._errorsSyncLock)
            {
                if (this.HasErrors() == false) { return; }
                List<string> keysToRemove = new List<string>();
                foreach (KeyValuePair<string, ErrorLogDO> kv in this.errors)
                {
                    ErrorLogDO e = kv.Value;
                    if (e != null)
                    {
                        if (e.Suppress == true) { continue; }
                        if (e.IsPersisted)
                        {
                            e.Delete();
                        }
                    }
                    keysToRemove.Add(kv.Key);

                }

                foreach (string k in keysToRemove)
                {
                    this.errors.Remove(k);
                }
            }
        }



        internal void ClearWarnings()
        {
            lock (this._errorsSyncLock)
            {
                if (this.HasErrors() == false) { return; }
                List<string> keysToRemove = new List<string>();
                foreach (KeyValuePair<string, ErrorLogDO> kv in this.errors)
                {
                    ErrorLogDO e = kv.Value;
                    if (e != null)
                    {
                        if (e.Suppress == true) { continue; }
                        if (e.Level == "E") { continue; }
                        if (e.IsPersisted)
                        {
                            e.Delete();
                        }
                    }
                    keysToRemove.Add(kv.Key);

                }

                foreach (string k in keysToRemove)
                {
                    this.errors.Remove(k);
                }
            }
        }

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

        public bool HasErrors(string propName)
        {
            if (HasErrors() == false) { return false; }
            //if (this.errors == null) { return false; }
            return this.errors.ContainsKey(propName) && this.errors[propName].Suppress == false;
        }

        public bool HasErrors()
        {
            if (this.errors == null) { return false; }
            return errors.Count > 0;
        }

        public String GetError(string fieldName)
        {
            return (this as IDataErrorInfo)[fieldName];
        }

        public void PurgeErrorList()
        {
            lock (_errorsSyncLock)
            {
                if (this.errors != null)
                {
                    this.errors.Clear();
                }
                this.IsValidated = false;
                this._errorsLoaded = false;
            }
        }

        protected void PopulateErrorList()
        {
            if (this.rowID == null) { return; }
            string tableName = DatastoreBase.GetObjectDiscription(this.GetType()).ReadSource;
            List<ErrorLogDO> errorList = this.DAL.Read<ErrorLogDO>("ErrorLog", "WHERE TableName = ? AND CN_Number = ?", tableName, this.rowID);
            foreach (ErrorLogDO e in errorList)
            {
                this.AddError(e.ColumnName, e);
            }
            this._errorsLoaded = true;
        }

        protected abstract bool DoValidate();

        public virtual bool Validate(IEnumerable<String> fields)
        {
            bool isValid = true;
            foreach (String f in fields)
            {
                isValid = this.ValidateProperty(f) && isValid;
            }
            return isValid;
        }

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
                return !HasErrors();
            }
        }

        //depreciated
        //public virtual bool ValidateProperty(string name)
        //{
        //    if (PropertyChangedEventsDisabled) { return true; }
        //    object value = null;
        //    try
        //    {
        //        value = DatastoreBase.GetObjectDiscription(this.GetType()).Fields[name]._getter.Invoke(this, null);
        //        //PropertyInfo property = this.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
        //        ////if (property == null) { return true; }
        //        //value = property.GetValue(this, null);
        //    }
        //    catch
        //    {
        //        return true;
        //    }
        //    return this.ValidateProperty(name, value);
        //}




        protected virtual bool ValidateProperty(string name, object value)
        {
            if (PropertyChangedEventsDisabled) { return true; }
            if (Validator != null) { return Validator.Validate(this, name, value); }
            else { return true; }
        }

        #endregion 

        protected override void NotifyPropertyChanged(string name)
        {
            base.NotifyPropertyChanged(name);
            if (!PropertyChangedEventsDisabled)
            {
                IsValidated = false;
            }
        }

    }
}
