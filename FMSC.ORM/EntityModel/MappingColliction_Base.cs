using System;

using System.Collections.Generic;
using System.Collections;
using FMSC.ORM.Core;

#if ANDROID
using Mono.Data.Sqlite;
#else
using System.Data.SQLite;
#endif

namespace FMSC.ORM.EntityModel
{
    public abstract class MappingCollection<MapType, ParentType, ChildType> : IEnumerable<ChildType>, IList<ChildType>, IList
        where MapType : class, IDataObject 
        where ParentType : class, IDataObject
        where ChildType : class, IDataObject
    {

        #region fields
        private List<ChildType> _unpersistedChildren = new List<ChildType>();
        private List<ChildType> _persistedChildren = new List<ChildType>(0); 
        private List<ChildType> _toBeDeleted = new List<ChildType>(); 
        #endregion 

        #region indexer & properties 
        public bool IsPopulated { get; protected set; }

        //read only
        public ChildType this[int index]
        {
            get 
            { 
                int pCount = _persistedChildren.Count;
                if (index >= pCount)
                {
                    return _unpersistedChildren[index - pCount];
                }
                else
                {
                    return _persistedChildren[index];
                }
            }
            set
            {
                throw new NotImplementedException();
            }
            
        }

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


        #endregion

        #region ctor
        public MappingCollection(ParentType parent)
        {
            Parent = parent;
            _DAL = parent.DAL;
            //_children = new List<ChildWraper>();
            //_toBeDeleted = new List<ChildWraper>();
            IsPopulated = false;
        }
        #endregion

        #region abstract methods
        protected abstract void addMap(ChildType child);

        protected abstract MapType retrieveMapObject(ChildType child);

        protected abstract List<ChildType> retrieveChildList();

        //protected abstract SQLiteCommand createJoinChildCommand();
        #endregion

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
            if (child == null)
            {
                throw new ArgumentNullException("child");
            }
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
            ////array = new ChildType[_children.Count - arrayIndex];
            //for (int i = arrayIndex, j = 0; i < this.Count; j++, i++)
            //{
            //    array.SetValue(this[i], j);
            //}
            ((ICollection)this).CopyTo(array, arrayIndex);
        }

        #endregion


        //TODO remove support for IList?
        #region IList<ChildType> Members

        public int IndexOf(ChildType item)
        {
            int indexOf = _persistedChildren.IndexOf(item);
            if (indexOf != -1)
            {
                return indexOf;
            }
            indexOf = _unpersistedChildren.IndexOf(item);
            if (indexOf != -1)
            {
                return indexOf + _persistedChildren.Count;
            }
            return -1;
        }


        /// <summary>
        /// not supported
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, ChildType item)
        {
            throw new NotSupportedException();
            //throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            Remove(this[index]);
        }

        #endregion

        //TODO do we need support for IList?
        #region IList Members

        int IList.Add(object value)
        {
            Add((ChildType)value);
            return IndexOf((ChildType)value);            
        }

        //void IList.Clear()
        //{
        //    this.Clear();
        //}

        bool IList.Contains(object value)
        {
            return this.Contains((ChildType)value);           
        }

        int IList.IndexOf(object value)
        {
            return this.IndexOf((ChildType)value);            
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (ChildType)value);            
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        //bool IList.IsReadOnly
        //{
        //    get { return this.IsReadOnly; }
        //}

        void IList.Remove(object value)
        {
            this.Remove((ChildType)value);
        }

        //void IList.RemoveAt(int index)
        //{
        //    this.RemoveAt(index);
        //}

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            for (int i = index, j = 0; j < this.Count && j < array.Length; j++, i++)
            {
                array.SetValue(this[i], j);
            }
        }

        //int ICollection.Count
        //{
        //    get { return this.Count; }
        //}

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { return this; }
        }

        #endregion

        #endregion

        #region IEnumerable<ChildType> Members

        public IEnumerator<ChildType> GetEnumerator()
        {
            int size = this.Count;
            for(int i = 0; i < size; i++)
            {
                yield return this[i];
            }

            //return new ChildEnum(this);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        //protected internal class ChildEnum : IEnumerator<ChildType>
        //{
        //    private MappingCollection<MapType, ParentType, ChildType> _mc;
        //    private int _size = -1;
        //    private int _currentIndex = -1;
        //    private ChildType _current;

        //    public ChildEnum(MappingCollection<MapType, ParentType, ChildType> collection)
        //    {
        //        _mc = collection;
        //        _size = _mc.Count;
        //    }

        //    #region IEnumerator<ChildType> Members

        //    public ChildType Current
        //    {
        //        get { return _current; }
        //    }

        //    #endregion

        //    #region IDisposable Members
            
        //    public void Dispose() 
        //    {
        //        _mc = null;
        //        _current = null;
        //    }

        //    #endregion

        //    #region IEnumerator Members

        //    object System.Collections.IEnumerator.Current
        //    {
        //        get { return _current; }
        //    }

        //    public bool MoveNext()
        //    {
        //        if (++_currentIndex < _size)
        //        {
        //            _current = _mc[_currentIndex];
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }

        //    public void Reset()
        //    {
        //        _currentIndex = -1;
        //        _current = null;
        //    }

        //    #endregion
        //}

        //protected internal enum ChildState {NotPersisted = 0, Persisted, Deleted};

        //protected internal class ChildWraper
        //{
        //    public ChildType Value { get; set; } 
        //    public ChildState State { get; set; }

        //    public ChildWraper(ChildType value)
        //    {
        //        this.Value = value;
        //        this.State = ChildState.NotPersisted;
        //    }

        //    public ChildWraper(ChildType value, ChildState state)
        //    {
        //        this.Value = value;
        //        this.State = state;
        //    }
        //}
    }
}
