using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGUtilities
{


    public class PriorityQueue<T>
    {
        private List<T> data;
        private readonly Comparison<T> comparison;

        public PriorityQueue(Comparison<T> comparison)
        {
            this.data = new List<T>();
            this.comparison = comparison;
        }

        public int Count => data.Count;

        public void Enqueue(T item)
        {
            data.Add(item);
            data.Sort(comparison);
        }

        public T Dequeue()
        {
            if (data.Count == 0)
                throw new InvalidOperationException("Queue is empty");

            T item = data[0];
            data.RemoveAt(0);
            return item;
        }
    }

}
