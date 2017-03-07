using System;
using System.Linq;

using System.Collections.Generic;
using System.Collections;
using FMSC.ORM.Core;

#if Mono
using Mono.Data.Sqlite;
#else

using System.Data.SQLite;

#endif

namespace FMSC.ORM.EntityModel
{
    public abstract class MappingCollection<MapType, ParentType, ChildType>
        : IEnumerable<ChildType>
        , ICollection<ChildType>
        , ICollection
        , IList
        where MapType : class, IDataObject
        where ParentType : class, IDataObject
        where ChildType : class, IDataObject
    {
        #region fields

        private List<ChildType> _unpersistedChildren = new List<ChildType>();
        private List<ChildType> _persistedChildren = new List<ChildType>(0);
        private List<ChildType> _toBeDeleted = new List<ChildType>();

        #endregion fields

        #region indexer & properties

        public bool IsPopulated { get; protected set; }

        public ParentType Parent
        {
            get;
            protected set;
        }

        private DatastoreRedux _DAL;

        public DatastoreRedux DAL
        {
            get { return _DAL; }
            set
            {
                _DAL = value;
                ResetChildStates();
            }
        }

        #endregion indexer & properties

        #region ctor

        protected MappingCollection(ParentType parent)
        {
            Parent = parent;
            _DAL = parent.DAL;

            IsPopulated = false;
        }

        #endregion ctor

        #region abstract methods

        protected abstract void addMap(ChildType child);

        protected abstract MapType retrieveMapObject(ChildType child);

        protected abstract List<ChildType> retrieveChildList();

        //protected abstract SQLiteCommand createJoinChildCommand();

        #endregion abstract methods

        #region Members

        protected void DeleteMap(ChildType child)
        {
            var map = retrieveMapObject(child);
            System.Diagnostics.Debug.Assert(map != null);
            if (map != null)
            {
                map.Delete();
            }
        }

        protected void ResetChildStates()
        {
            _toBeDeleted.Clear();
            foreach (ChildType child in _persistedChildren)
            {
                _unpersistedChildren.Add(child);
            }
        }

        public bool HasChanges
        {
            get
            {
                return this.DAL == null
                    || this._unpersistedChildren.Count > 0
                    || this._toBeDeleted.Count > 0;
            }
        }

        public void Populate()
        {
            if (this.Parent.IsPersisted == false) { return; }
            this._persistedChildren = retrieveChildList();
            foreach (ChildType c in _toBeDeleted)
            {
                _persistedChildren.Remove(c);
            }

            IsPopulated = true;
        }

        public void Save()
        {
            if (this.DAL == null)
            {
                throw new InvalidOperationException("DAL is null");
            }

            foreach (ChildType c in _unpersistedChildren)
            {
                addMap(c);
                _persistedChildren.Add(c);
            }
            _unpersistedChildren.Clear();

            foreach (ChildType c in _toBeDeleted)
            {
                DeleteMap(c);
            }
            _toBeDeleted.Clear();
        }

        #region ICollection<ChildType> Members

        public int Count
        {
            get { return _persistedChildren.Count + _unpersistedChildren.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add(ChildType child)
        {
            if (child == null) { throw new ArgumentNullException("child"); }

            if (_unpersistedChildren.Contains(child)) { return; }
            if (_persistedChildren.Contains(child)) { return; }

            if (_toBeDeleted.Remove(child))
            {
                _persistedChildren.Add(child);
            }
            else
            {
                _unpersistedChildren.Add(child);
            }
        }

        public bool Remove(ChildType child)
        {
            if (_persistedChildren.Remove(child))
            {
                _toBeDeleted.Add(child);
                return true;
            }
            else
            {
                return _unpersistedChildren.Remove(child);
            }
        }

        public bool Contains(ChildType child)
        {
            return _persistedChildren.Contains(child) || _unpersistedChildren.Contains(child);
        }

        public void Clear()
        {
            foreach (ChildType child in _persistedChildren)
            {
                _toBeDeleted.Add(child);
            }
            _persistedChildren.Clear();
            _unpersistedChildren.Clear();
        }

        public void CopyTo(ChildType[] array, int arrayIndex)
        {
            ((ICollection)this).CopyTo(array, arrayIndex);
        }

        #endregion ICollection<ChildType> Members

        #region ICollection Members

        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            if (array == null) { throw new ArgumentNullException("array"); }
            if (arrayIndex < 0) { throw new ArgumentOutOfRangeException("arrayIndex"); }
            if (this.Count > array.Length - arrayIndex) { throw new ArithmeticException("The number of elements in the source ICollection<T> is greater than the available space from arrayIndex to the end of the destination array."); }

            foreach (var c in this)
            {
                array.SetValue(c, arrayIndex++);
            }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { return this; }
        }

        #endregion ICollection Members

        #endregion Members

        #region IEnumerable<ChildType> Members

        public IEnumerator<ChildType> GetEnumerator()
        {
            foreach (var c in _persistedChildren)
            {
                yield return c;
            }

            foreach (var c in _unpersistedChildren)
            {
                yield return c;
            }
        }

        #endregion IEnumerable<ChildType> Members

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion IEnumerable Members

        #region IList members

        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        object IList.this[int index]
        {
            get
            {
                return this.ElementAt(index);
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        void IList.Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        int IList.Add(object obj)
        {
            var value = obj as ChildType;
            if (value == null) { return -1; }
            Add(value);
            return ((IList)this).IndexOf(obj);
        }

        bool IList.Contains(object value)
        {
            return Contains(value as ChildType);
        }

        void IList.Remove(object value)
        {
            Remove(value as ChildType);
        }

        int IList.IndexOf(object value)
        {
            var i = -1;
            foreach (var item in this)
            {
                i++;
                if (item == value)
                { break; }
            }
            return i;
        }

        void IList.RemoveAt(int index)
        {
            var itemAt = this.ElementAt(index);
            Remove(itemAt);
        }

        #endregion IList members
    }
}