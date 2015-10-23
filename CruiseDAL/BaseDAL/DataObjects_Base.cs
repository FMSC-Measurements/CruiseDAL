using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Reflection;
using CruiseDAL.DataObjects;
using System.Text;


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
    public abstract class DataObject : INotifyPropertyChanged, IDataErrorInfo, IFormattable
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

        #endregion

        /// <summary>
        /// Tag allows any suplemental object to 
        /// be atatched to a dataobject. 
        /// </summary>
        [XmlIgnore]
        public Object Tag { get; set; }

        [XmlIgnore]
        public abstract RowValidator Validator
        {
            get;
        }

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

        #region validation
        protected bool _errorsLoaded = false;

        #region IDataErrorInfo Members

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
            lock(this._errorsSyncLock)
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
            foreach(ErrorLogDO e in errorList)
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

        

        //depreciated
        public virtual bool ValidateProperty(string name)
        {
            if (PropertyChangedEventsDisabled) { return true; }
            object value = null;
            try
            {
                value = DatastoreBase.GetObjectDiscription(this.GetType()).Properties[name]._getter.Invoke(this, null);
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

        protected void NotifyPropertyChanged(string name, object value)
        {
            NotifyPropertyChanged(name);
        }

        protected virtual void NotifyPropertyChanged(string name)
        {
            if (!PropertyChangedEventsDisabled)
            {
                HasChanges = true;
                IsValidated = false;
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

        //protected abstract PropertyInfo[] GetPropertieInfo();

        #region IFormattable Members
        /// <summary>
        /// replaces placehoders with property values in format string. 
        /// [propertyName], [propertyName:nullValue], [propertyName:nullValue:pad], [propertyName::pad] 
        /// pad option can include prefix U | L | C . for Uppercase, lowercase, capitalize
        /// </summary>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (String.IsNullOrEmpty(format))
            {
                return this.ToString();
            }



            //get a list of all propertie place holders in format
            System.Text.RegularExpressions.Regex rx = new System.Text.RegularExpressions.Regex(@"\[(?<prop>[a-zA-Z]\w+)(?:(?:\|)(?<ifnull>\w+))?(?:(?::)(?<pad>(?:[-]?\d+)?[ULC]?))?\]", RegexOptions.Compiled);

            return rx.Replace(format, this.ProcessFormatElementMatch);
        }

        /// <summary>
        /// helper method for ToString(String format, IFormatProvider fomatProvider)
        /// </summary>
        private string ProcessFormatElementMatch(Match m)
        {
            string sValue = string.Empty;
            string propName = m.Groups["prop"].Captures[0].Value;
            string ifnull = string.Empty;
            if (m.Groups["ifnull"].Captures.Count == 1)
            {
                ifnull = m.Groups["ifnull"].Captures[0].Value;
            }

            try
            {
                EntityDescription des = DatastoreBase.GetObjectDiscription(this.GetType());
                Object value = des.Properties[propName]._getter.Invoke(this, null);
                if (value != null && value is IFormattable)
                {
                    sValue = ((IFormattable)value).ToString(null, null);
                }
                else
                {
                    sValue = (value == null) ? ifnull : value.ToString();
                }
            }
            catch
            {
                throw new FormatException("unable to resolve value for " + propName);
            }


            short pad;
            char padOpt;
            if (m.Groups["pad"].Captures.Count == 1)
            {
                try
                {

                    String sPad = m.Groups["pad"].Captures[0].Value;
                    try
                    {
                        pad = short.Parse(sPad.TrimEnd('U', 'L', 'C', 'u', 'l', 'c'));
                    }
                    catch
                    {
                        pad = 0;
                    }
                    char last = char.ToUpper(sPad[sPad.Length - 1]);
                    padOpt = (last == 'U' || last == 'L' || last == 'C') ? last : char.MinValue;

                    switch (padOpt)
                    {
                        case 'U':
                            {
                                sValue = sValue.ToUpper();
                                break;
                            }
                        case 'L':
                            {
                                sValue = sValue.ToLower();
                                break;
                            }
                        case 'C':
                            {
                                sValue = CapitalizeString(sValue);
                                break;
                            }
                    }

                    if (pad < 0)
                    {
                        sValue = sValue.PadRight(Math.Abs(pad));
                    }
                    else if (pad > 0)
                    {
                        sValue = sValue.PadLeft(pad);
                    }
                }
                catch (Exception ex)
                {
                    throw new FormatException("Format element " + propName + " pad argument invalid", ex);
                }
            }
            return sValue;
        }

        private static String CapitalizeString(String s)
        {
            char[] cArray = s.ToCharArray();
            for (int i = 0; i < cArray.Length; i++)
            {
                if (Char.IsLetter(cArray[i]))
                {
                    cArray[i] = char.ToUpper(cArray[i]);
                    break;
                }
            }
            return new String(cArray);
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
