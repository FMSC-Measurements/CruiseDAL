using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using CruiseDAL.DataObjects;
using FMSC.ORM.Core.SQL;
using Backpack.SqlBuilder;

namespace CruiseDAL
{
    public class CruiseDALErrorCollection
    {
        Dictionary<String, ErrorLogDO> _errors;
        readonly object _errorsSyncLock = new object();

        String _tableName;

        //EntityDescription _entityDescription;
        DataObject _dataObject;

        public CruiseDALErrorCollection(DataObject dataObject, String tableName)
        {
            _dataObject = dataObject;
            _tableName = tableName;
        }

        public bool ErrorsLoaded { get; set; }

        public bool HasErrors
        {
            get
            {
                if (this._errors == null) { return false; }
                return _errors.Count > 0;
            }
        }

        public String GetErrors()
        {
            lock (this._errorsSyncLock)
            {
                if (this.HasErrors == false) { return String.Empty; }
                var b = new StringBuilder();
                foreach (ErrorLogDO e in _errors.Values)
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
                if (!_errors.ContainsKey(fieldName)) { return new string[0]; }
                ErrorLogDO e = _errors[fieldName];
                if (e.Suppress == false)
                {
                    return new string[] { _errors[fieldName].Message };
                }
                else
                {
                    return new string[0];
                }
            }
        }

        public bool HasError(string propName)
        {
            if (HasErrors == false) { return false; }
            return this._errors.ContainsKey(propName) && this._errors[propName].Suppress == false;
        }

        public void AddError(string propName, string error)
        {
            var e = new ErrorLogDO();
            if (error.StartsWith("Warning::"))
            {
                e.Level = "W";
            }
            else
            {
                e.Level = "E";
            }

            e.TableName = _tableName;
            e.ColumnName = propName;
            e.Message = error;
            e.Program = AppDomain.CurrentDomain.FriendlyName;
            AddError(propName, e);
        }

        private void AddError(string propName, ErrorLogDO e)
        {
            lock (this._errorsSyncLock)
            {
                if (this._errors == null)
                {
                    this._errors = new Dictionary<string, ErrorLogDO>();
                }

                if (_errors.ContainsKey(propName) == true)
                {
                    ErrorLogDO e1 = _errors[propName];
                    if (e1.Level[0] != e.Level[0] && e1.Suppress == true)
                    {
                        _errors[propName] = e;
                        //OnErrorsChanged(new DataErrorsChangedEventArgs(propName));
                    }
                    else if (e1.Message == e.Message && (e.Suppress == true && e1.Suppress == false))
                    {
                        if (e1.IsPersisted)
                        {
                            e1.Delete();
                        }
                        _errors[propName] = e;
                        //OnErrorsChanged(new DataErrorsChangedEventArgs(propName));
                    }
                    else if (e1.Level.StartsWith("W") && e.Level.StartsWith("E"))
                    {
                        _errors[propName] = e;
                        //OnErrorsChanged(new DataErrorsChangedEventArgs(propName));
                    }
                }
                else
                {
                    _errors[propName] = e;
                    //OnErrorsChanged(new DataErrorsChangedEventArgs(propName));
                }
            }
        }

        public void RemoveError(string propName, string error)
        {
            lock (this._errorsSyncLock)
            {
                if (_errors == null) { return; }
                if (_errors.ContainsKey(propName) == true)
                {
                    //if (errors[propName] != error) { throw new InvalidOperationException(); }
                    ErrorLogDO e = _errors[propName];
                    if (e.Message == error)
                    {
                        if (e.Suppress == true) { return; }

                        this._errors.Remove(propName);
                        if (e.IsPersisted)
                        {
                            e.Delete();
                        }
                        //OnErrorsChanged(new DataErrorsChangedEventArgs(propName));
                    }
                }

                if (this._errors.Count == 0)
                {
                    this._errors = null;
                }
            }
        }

        internal void ClearErrors()
        {
            lock (this._errorsSyncLock)
            {
                if (this.HasErrors == false) { return; }
                var keysToRemove = new List<string>();
                foreach (KeyValuePair<string, ErrorLogDO> kv in this._errors)
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
                    this._errors.Remove(k);
                }
            }
        }

        internal void ClearWarnings()
        {
            lock (this._errorsSyncLock)
            {
                if (this.HasErrors == false) { return; }
                var keysToRemove = new List<string>();
                foreach (KeyValuePair<string, ErrorLogDO> kv in this._errors)
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
                    this._errors.Remove(k);
                }
            }
        }

        //protected void OnErrorsChanged(DataErrorsChangedEventArgs e)
        //{
        //    if(ErrorsChanged != null)
        //    {
        //        ErrorsChanged(this._dataObject, e);
        //    }
        //}

        public void PurgeErrorList()
        {
            lock (_errorsSyncLock)
            {
                if (this._errors != null)
                {
                    this._errors.Clear();
                }
                _dataObject.IsValidated = false;
                this.ErrorsLoaded = false;
            }
        }

        public void PopulateErrorList()
        {
            if (_dataObject.rowID == null) { return; }
            string tableName = _tableName;

            var errors = _dataObject.DAL.From<ErrorLogDO>()
                .Where("TableName = @p1 AND CN_Number = @p2 ")
                .Read(tableName, _dataObject.rowID);

            foreach (ErrorLogDO e in errors)
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
                if (this._errors == null) { return; } //no errors to save
                foreach (ErrorLogDO e in _errors.Values)
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
    }
}