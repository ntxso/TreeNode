using System;
using System.Collections.Generic;
using System.Collections;

namespace NTLib
{
    public class NTreeColection<T> : ICollection<T>, IEnumerable<T> where T : NTree<T>
    {
        protected List<T> collection;
        
        public NTreeColection()
        {
            Level = 0;
            Parent = null;
            collection = new List<T>();
            Name = "root";
        }
        public NTreeColection(T owner)
        {
            collection = new List<T>();
            Parent = owner;
            Level = owner.Level + 1;
            Name = owner.Name;
        }
        public void Clear() { collection.Clear(); }
        public bool Contains(T item) { return collection.Contains(item); }
        public void CopyTo(T[] array, int index = 0) { collection.CopyTo(array, index); }
        public bool Remove(T item) { return collection.Remove(item); }
        public int Count { get { return collection.Count; } }
        public int CountAll
        {
            get
            {
                int count = 0;
                for (int i = 0; i < Count; i++)
                {
                    count += this[i].Count;
                }
                return count;
            }
        }
        public bool IsReadOnly { get { return false; } }
        IEnumerator IEnumerable.GetEnumerator()
        {
                return collection.GetEnumerator();
        }
        public IEnumerator<T> GetEnumerator()
        {
                return collection.GetEnumerator();
        }
        public T this[int index] { get { return collection[index]; } }
        public T Last { get { return collection[Count - 1]; } }
        public int IndexOf(T item)
        {
            return collection.IndexOf(item);
        }
        internal T Parent { get; private set; }
        internal int Level { get; private set; }
        public string Name { get; private set; }
        public void Add(T treeItem)
        {
            treeItem.Parent = Parent;
            treeItem.inArray = this;

            if (treeItem.children != null)
                if (treeItem.children.Count > 0)
                    setAllLevel(treeItem.children);

            collection.Add(treeItem);

        }
        public void Insert(int index, T item)
        {
            item.Parent = Parent;
            item.inArray = this;

            if (item.children != null)
                if (item.children.Count > 0)
                    setAllLevel(item.children);

            collection.Insert(index, item);
        }
        public void RemoveAt(int index)
        {
            collection.RemoveAt(index);
        }
        public List<T> Tags { get { return collection; } }

        private void setAllLevel(NTreeColection<T> col)
        {
            //item.Level+
            foreach (var item in col)
            {
                item.setReNewLevel();
                if (item.children != null)
                    if (item.children.Count > 0)
                        setAllLevel(item.children);
            }
        }
    }
}
