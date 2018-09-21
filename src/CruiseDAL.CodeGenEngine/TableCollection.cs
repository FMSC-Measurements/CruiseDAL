using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace FMSCORM.CodeGenEngine
{
    //TODO Log and logical unimplemented
    public enum DeleteMethodType { Unknown = 0, None, Tombstone, Log, Logical, TombstoneGUID }

    public class TableCollection : ICollection<Table>
    {
        public DeleteMethodType DeleteMethod { get; set; }
        public bool TrackCreated { get; set; }
        public bool TrackModified { get; set; }
        public bool TrackMergeState { get; set; }
        public bool TrackRowVersion { get; set; } 
        public String Name { get; set; }

        protected Dictionary<string, Table> _tables = new Dictionary<string, Table>();
        
        public Table this[string key]
        {
            get
            {
                return _tables[key];
            }
            set
            {
                this._tables[key] = value;
            }
        }




        #region ICollection<Table> Members

        public void Add(Table item)
        {
            this._tables.Add(item.Name, item);
        }

        public void Clear()
        {
            this._tables.Clear();
        }

        public bool Contains(Table item)
        {
            return this._tables.ContainsValue(item);
        }

        public void CopyTo(Table[] array, int arrayIndex)
        {
            ((ICollection)this._tables).CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this._tables.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(Table item)
        {
            return this._tables.Remove(item.Name);
        }

        #endregion

        #region IEnumerable<Table> Members

        public IEnumerator<Table> GetEnumerator()
        {
            return this._tables.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}
