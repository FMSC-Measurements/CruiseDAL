using System;

using System.Collections.Generic;
using System.Text;
using Logger;
using System.Collections; 

#if ANDROID
using Mono.Data.Sqlite;
#else
using System.Data.SQLite;
#endif

namespace CruiseDAL
{
    public abstract class MappingCollection<MapType, ParentType, ChildType> : IEnumerable<ChildType>, IList<ChildType>, IList
        where MapType : DataObject
        where ParentType : DataObject
        where ChildType : DataObject
    {

        #region fields
        private List<ChildWraper> _children;
        private List<ChildWraper> _toBeDeleted; 
        #endregion 

        #region indexer & properties 
        public bool IsPopulated { get; protected set; }

        //read only
        public ChildType this[int index]
        {
            get { return _children[index].Value; }
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

        private DatastoreBase _DAL;
        internal DatastoreBase DAL
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
            _children = new List<ChildWraper>();
            _toBeDeleted = new List<ChildWraper>();
            IsPopulated = false;
        }
        #endregion


        #region Members
        protected void ResetChildStates()
        {
            _toBeDeleted.Clear();
            foreach (ChildWraper child in _children)
            {
                child.State = ChildState.NotPersisted;
            }
        }

        public void Populate()
        {
            if (this.Parent.IsPersisted == false) { return; }
            //_children.Clear();
            //_toBeDeleted.Clear();

            foreach (ChildType c in retrieveChildList())
            {
                if(!this.Contains(c))
                {
                    _children.Add(new ChildWraper(c, ChildState.Persisted));
                }
            }
            IsPopulated = true;
        }

        public void Save()
        {
            if (this.DAL == null)
            {
                throw new InvalidOperationException("DAL is null");
            }

            foreach (ChildWraper c in _children)
            {
                saveChild(c);
            }

            foreach (ChildWraper c in _toBeDeleted)
            {
                saveChild(c);
            }
            _toBeDeleted.Clear();

        }

        protected void saveChild(ChildWraper child)
        {
            if (child.State == ChildState.Persisted)
            {
                return;
            }
            else if (child.State == ChildState.NotPersisted)
            {
                addMap(child.Value);
                child.State = ChildState.Persisted;
            }
            else if (child.State == ChildState.Deleted)
            {
                deleteMap(child.Value);
            }
        }

        protected void deleteMap(ChildType child)
        {
            var map = retrieveMapObject(child);
            if (map != null)
            {
                map.Delete();
            }
        }

        protected abstract void addMap(ChildType child);

        protected abstract MapType retrieveMapObject(ChildType child);

        protected abstract List<ChildType> retrieveChildList();

        //protected abstract SQLiteCommand createJoinChildCommand();

        public void Add(ChildType child)
        {
            if (child == null)
            {
                throw new ArgumentNullException("child");
            }
            var item = findItem(_toBeDeleted, child);
            //check to see if child is in the to be deleted list
            if (item != null)
            {
                //if so remove from the to be delected list 
                _toBeDeleted.Remove(item);
                item.State = ChildState.Persisted;
                //and add back to the child list
                _children.Add(item);
            }
            //if the child is not in the tobe deleted list 
            //and is not in the child list
            else if (!containsItem(_children, child))
            {
                //then add it to the child list
                item = new ChildWraper(child);
                _children.Add(item);
            }
            //otherwise do nothing
        }

        public bool Remove(ChildType child)
        {
            var item = findItem(_children, child);
            //first check to see if it is in the child list
            if (item != null)
            {
                //if so check its state 
                //if it is persisted add it to the tobe deleted list 
                if (item.State == ChildState.Persisted)
                {
                    item.State = ChildState.Deleted;
                    _toBeDeleted.Add(item);
                }
                //if not persisted then just remove it from the child list
                _children.Remove(item);
                return true; 
            }
            return false; 
        }


        public bool Contains(ChildType child)
        {
            return containsItem(_children, child);
        }


        public int Count
        {
            get { return _children.Count; }
        }

        protected static ChildWraper findItem(List<ChildWraper> list, ChildType obj)
        {
            foreach (ChildWraper c in list)
            {
                if (Object.ReferenceEquals(c.Value, obj))
                {
                    return c;
                }
            }
            return null;
        }

        protected static bool containsItem(List<ChildWraper> list, ChildType obj)
        {
            foreach ( ChildWraper c in list) 
            {
                if( Object.ReferenceEquals(c.Value, obj))
                {
                    return true; 
                }
            }
            return false;
        }

        protected int indexOfItem(List<ChildWraper> list, ChildType obj)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (Object.ReferenceEquals(list[i].Value, obj))
                {
                    return i;
                }
            }
            return -1;
        }


        #endregion

        #region IEnumerable<ChildType> Members

        public IEnumerator<ChildType> GetEnumerator()
        {
            return new ChildEnum(_children);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        protected internal class ChildEnum : IEnumerator<ChildType>
        {
            private List<ChildWraper> list;
            private int currentIndex;
            private ChildType current;

            public ChildEnum(List<ChildWraper> list)
            {
                this.list = list;
                currentIndex = -1;
                current = null;
            }

            #region IEnumerator<ChildType> Members

            public ChildType Current
            {
                get { return current; }
            }

            #endregion

            #region IDisposable Members

            public void Dispose() { }


            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get { return current; }
            }

            public bool MoveNext()
            {
                if (++currentIndex < list.Count)
                {
                    current = list[currentIndex].Value;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public void Reset()
            {
                currentIndex = -1;
                current = null;
            }

            #endregion
        }

        protected internal enum ChildState {NotPersisted = 0, Persisted, Deleted};

        protected internal class ChildWraper
        {
            public ChildType Value { get; set; } 
            public ChildState State { get; set; }

            public ChildWraper(ChildType value)
            {
                this.Value = value;
                this.State = ChildState.NotPersisted;
            }

            public ChildWraper(ChildType value, ChildState state)
            {
                this.Value = value;
                this.State = state;
            }
        }

        #region IList<ChildType> Members

        public int IndexOf(ChildType item)
        {
            return indexOfItem(_children, item);
        }

        public void Insert(int index, ChildType item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            Remove(this[index]);
        }

        

        #endregion

        #region ICollection<ChildType> Members


        public void Clear()
        {
            foreach (ChildWraper child in _children)
            {
                if (child.State == MappingCollection<MapType,ParentType,ChildType>.ChildState.Persisted)
                {
                    child.State = MappingCollection<MapType, ParentType, ChildType>.ChildState.Deleted;
                    _toBeDeleted.Add(child);
                }
            }
            _children.Clear();
        }

        public void CopyTo(ChildType[] array, int arrayIndex)
        {
            //array = new ChildType[_children.Count - arrayIndex];
            for (int i = arrayIndex, j = 0; i < _children.Count; j++, i++)
            {
                array.SetValue(_children[i].Value, j);
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region IList Members

        int IList.Add(object value)
        {
            if(value is ChildType)
            {
                Add(value as ChildType);
                return IndexOf(value as ChildType);
            }
            return -1;
        }

        void IList.Clear()
        {
            this.Clear();
        }

        bool IList.Contains(object value)
        {
            if (value is ChildType)
            {
                return this.Contains(value as ChildType);
            }
            return false;
        }

        int IList.IndexOf(object value)
        {
            if (value is ChildType)
            {
                return this.IndexOf(value as ChildType);
            }
            return -1;
        }

        void IList.Insert(int index, object value)
        {
            if(value is ChildType)
            {
                Insert(index, value as ChildType);
            }
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        bool IList.IsReadOnly
        {
            get { return this.IsReadOnly; }
        }

        void IList.Remove(object value)
        {
            if (value is ChildType)
            {
                this.Remove(value as ChildType);
            }
        }

        void IList.RemoveAt(int index)
        {
            this.RemoveAt(index);
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            for (int i = index, j = 0; j < _children.Count && j < array.Length; j++, i++)
            {
                array.SetValue(_children[i].Value, j);
            }
        }

        int ICollection.Count
        {
            get { return this.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { return ((ICollection)this._children).SyncRoot; }
        }

        #endregion
    };
}
