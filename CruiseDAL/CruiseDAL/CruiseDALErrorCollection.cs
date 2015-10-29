using CruiseDAL.BaseDAL;
using CruiseDAL.DataObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL
{
    public class CruiseDALErrorCollection
    {
        Dictionary<String, ErrorLogDO> errors;
        object _errorsSyncLock = new object();

        EntityDescription _entityDescription;
        CruiseDALDataObject _dataObject;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public CruiseDALErrorCollection(CruiseDALDataObject dataObject, EntityDescription entityDescription)
        {
            _dataObject = dataObject;
            _entityDescription = entityDescription;
        }

        public bool ErrorsLoaded { get; set; }


        public String GetErrors()
        {
            lock (this._errorsSyncLock)
            {
                if (this.HasErrors == false) { return String.Empty; }
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

        public IEnumerable<String> GetErrors(string fieldName)
        {
            lock (this._errorsSyncLock)
            {
                if (this.HasError(fieldName) == false) { return new string[0]; }
                if (!errors.ContainsKey(fieldName)) { return new string[0]; }
                ErrorLogDO e = errors[fieldName];
                if (e.Suppress == false)
                {
                    return new string[] { errors[fieldName].Message };
                }
                else
                {
                    return new string[0];
                }
            }
        }

        public bool HasErrors
        {
            get
            {
                if (this.errors == null) { return false; }
                return errors.Count > 0;
            }
        }

        public bool HasError(string propName)
        {
            if (HasErrors == false) { return false; }
            return this.errors.ContainsKey(propName) && this.errors[propName].Suppress == false;
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

            e.TableName = _entityDescription.SourceName;
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
                        OnErrorsChanged(new DataErrorsChangedEventArgs(propName));
                    }
                    else if (e1.Message == e.Message && (e.Suppress == true && e1.Suppress == false))
                    {
                        if (e1.IsPersisted)
                        {
                            e1.Delete();
                        }
                        errors[propName] = e;
                        OnErrorsChanged(new DataErrorsChangedEventArgs(propName));
                    }
                    else if (e1.Level.StartsWith("W") && e.Level.StartsWith("E"))
                    {
                        errors[propName] = e;
                        OnErrorsChanged(new DataErrorsChangedEventArgs(propName));
                    }

                }
                else
                {
                    errors[propName] = e;
                    OnErrorsChanged(new DataErrorsChangedEventArgs(propName));
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
                if (this.HasErrors == false) { return; }
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

        protected void OnErrorsChanged(DataErrorsChangedEventArgs e)
        {
            if(ErrorsChanged != null)
            {
                ErrorsChanged(this._dataObject, e);
            }
        }

        public void PurgeErrorList()
        {
            lock (_errorsSyncLock)
            {
                if (this.errors != null)
                {
                    this.errors.Clear();
                }
                _dataObject.IsValidated = false;
                this.ErrorsLoaded = false;
            }
        }

        protected void PopulateErrorList()
        {
            if (_dataObject.rowID == null) { return; }
            string tableName = _entityDescription.SourceName;

            List<ErrorLogDO> errorList = _dataObject.DAL.Read<ErrorLogDO>("ErrorLog"
                , "WHERE TableName = ? AND CN_Number = ?", tableName, _dataObject.rowID);

            foreach (ErrorLogDO e in errorList)
            {
                this.AddError(e.ColumnName, e);
            }
            this.ErrorsLoaded = true;
        }

        public void SaveErrors()
        {
            System.Diagnostics.Debug.Assert(_dataObject.IsPersisted == true
                , "data object must be persisted before you can save errors");

            if (_dataObject.IsPersisted == false) { return; }
            
            lock (this._errorsSyncLock)
            {
                if (this.errors == null) { return; } //no errors to save
                foreach (ErrorLogDO e in errors.Values)
                {
                    if (e.DAL == null || e.DAL != _dataObject.DAL)
                    {
                        e.DAL = (DAL)_dataObject.DAL;
                    }

                    e.CN_Number = _dataObject.rowID.Value;
                    e.Save(OnConflictOption.Replace);
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
                        OnErrorsChanged(new DataErrorsChangedEventArgs(propName));
                    }
                }
                if (this.errors.Count == 0)
                {
                    this.errors = null;
                }
            }
        }
    }
}
