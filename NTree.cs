using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace NTLib
{
    public class NTree<T> : IEnumerator<T>, IEnumerable<T> where T : NTree<T>
    {
        private T parent;
        private string fullStr = string.Empty;
        internal protected NTreeColection<T> children;
        private T curObject;
        //internal T nextObject = null;
        private bool enumFirstNode = true;
        private int count;

        internal NTreeColection<T> inArray { get; set; }

        public T Parent
        {
            get
            {
                return parent;
            }
            internal set
            {
                parent = value;
                if (parent != null)
                    Level = parent.Level + 1;
                else Level = 0;
            }
        }
        public int Level { get; internal set; }
        internal void setReNewLevel()
        {
            Level = parent.Level + 1;
        }
        public string Name { get; protected set; }
        public T Next
        {
            get
            {
                return nextItem();
            }
        }
        public T Previous
        {
            get
            {
                return previousItem();
            }
        }

        public string FullPath
        {
            get
            {
                return fullpath(this);

            }
        }
        public int Count
        {
            get
            {
                countRecursive((T)this);
                return count;
            }
        }
        public int Index
        {
            get
            {
                if (inArray == null)
                    return 0;
                return inArray.IndexOf((T)this);
            }
        }
        private int countRecursive(T tag)
        {
            count = 1;
            if (tag.children != null)
                foreach (T item in tag.children)
                {
                    count += countRecursive(item);
                }
            return count;
        }
        public bool MoveNext()
        {
            //if (curObject == null) return false;
            if (enumFirstNode)
            {
                enumFirstNode = false;
                curObject = (T)this;
                return true;
            }
            curObject = curObject.Next;
            
            if (curObject == null)
            {
                enumFirstNode = true;
                return false;
            }
            if (curObject.Level <= Level)
            {
                enumFirstNode = true;
                return false;
            }
            return (curObject != null);
        }
        public T Current { get { return curObject; } }
        object IEnumerator.Current { get { return curObject; } }
        public void Reset() { enumFirstNode = true; }
        void IDisposable.Dispose() { enumFirstNode = true; }
        public IEnumerator<T> GetEnumerator() { return (T)this; }
        IEnumerator IEnumerable.GetEnumerator() { return (T)this; }
        public T this[int index]
        {
            get
            {
                if (index == 0) return (T)this;
                if (children == null) throw new ArgumentOutOfRangeException();
                if (index < 1 || index > children.Count) throw new ArgumentOutOfRangeException();
                return children[index - 1];
            }
        }

        private T nextItem()
        {
            //next child
            if (!children.IsNullOrEmptyCollection())
                return children.First();

            //same array next item
            if (!inArray.IsNullOrEmptyCollection())
            {
                int index = inArray.IndexOf((T)this);
                if (index < inArray.Count - 1)
                    return inArray[index + 1];
            }
            T result = (T)this;
            //parent after
            while ((result = result.Parent) != null)
            {
                if (!result.inArray.IsNullOrEmptyCollection())
                {
                    int index = result.inArray.IndexOf(result);
                    if (index < result.inArray.Count - 1)
                        return result.inArray[index + 1];
                }
            }
            return null;
        }
        private T previousItem()
        {
            T result = (T)this;
            //same array previous item
            if (!result.inArray.IsNullOrEmptyCollection())
            {
                int index = result.inArray.IndexOf(result);
                if (index > 0)
                    result = result.inArray[index - 1];
                else
                    return result.Parent;
            }
            //the last deepest
            while (!result.children.IsNullOrEmptyCollection())
            {
                result = result.children.Last;
            }

            return result;
        }
        private string fullpath(NTree<T> item)
        {
            List<string> temp = new List<string>();
            string result = string.Empty;
            for (NTree<T> obj = this; obj != null; obj = obj.parent)
                temp.Add(obj.Name);
            temp.Reverse();
            foreach (string nameitem in temp)
            {
                result += nameitem + "\\";
            }
            if (result.Length > 1) result = result.Remove(result.Length - 1, 1);
            return result;
        }
    }

}
