using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectTV;

namespace KinectTV.Kinect
{
    public class CircularBuffer<T> : IList<T>, ICollection<T>
    {
        protected T[] data;
        protected int cap;
        protected int end;
        protected int size;
        protected bool isFull;

        protected int getArrayIndex(int index)
        {
            if (isFull)
            {
               // Program.Notify("" + ((isFull ? end + 1 : 0) + index % size));
            }
            return ((isFull ? end + 1 : 0) + index) % size;
        }

        public CircularBuffer(int cap)
        {
            this.cap = cap;
            Clear();
        }

        public void Add(T item)
        {
            data[end] = item;
            end++;
            size++;

            if (end >= cap)
            {
                isFull = true;
                end = 0;
            }
            if (size > cap) size = cap;
        }

        public T this[int index]
        {
           
            get
            {
                if (index >= size || index < 0) throw new IndexOutOfRangeException();
                return data[getArrayIndex(index)];
            }
            set
            {
                if (index >= size || index < 0) throw new IndexOutOfRangeException();
                data[getArrayIndex(index)] = value;
            }
        }

        public int IndexOf(T item)
        {
            for (int index = 0; index < size; index++)
            {
                if (data[index].Equals(item))
                {
                    return getArrayIndex(index);
                }
            }
            return -1;
        }
        
        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            data = new T[cap];
            size = end = 0;
            isFull = false;
        }

        public bool Contains(T item)
        {
            return IndexOf(item) != -1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return size; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
