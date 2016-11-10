using System;
using System.Collections.Generic;

namespace Science.Collections.Navigation
{
    public static class ReadOnlyListExtensions
    {
        public static IListItemNavigator<T> GetNavigator<T>(this IReadOnlyList<T> source, int index = 0)
        {
            return new ReadOnlyListItemNavigator<T>(source, index);
        }
    }

    public class ReadOnlyListItemNavigator<T> : IListItemNavigator<T>
    {
        public ReadOnlyListItemNavigator(IReadOnlyList<T> collection, int index = 0)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (index < 0 || index >= collection.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            _collection = collection;
            _index = index;
        }

        private readonly IReadOnlyList<T> _collection;
        private int _index;

        
        /// <summary>
        /// Gets or sets the position of the navigator.
        /// </summary>
        public int CurrentIndex
        {
            get { return _index; }
            set
            {
                if (value < 0 || value >= _collection.Count)
                    throw new ArgumentOutOfRangeException(nameof(value));

                _index = value;
            }
        }

        /// <summary>
        /// Gets the current value pointed at.
        /// </summary>
        public T Current
        {
            get { return _collection[this.CurrentIndex]; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        /// <returns></returns>
        public IListItemNavigator<T> Offset(int delta)
        {
            if (this.CurrentIndex + delta < 0 ||
                this.CurrentIndex + delta <= _collection.Count)
                throw new ArgumentOutOfRangeException(nameof(delta));

            return new ReadOnlyListItemNavigator<T>(_collection, this.CurrentIndex + delta);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        /// <returns>The element at the peeked position.</returns>
        public T Peek(int delta)
        {
            if (this.CurrentIndex + delta < 0 ||
                this.CurrentIndex + delta >= _collection.Count)
                return default(T);

            return _collection[this.CurrentIndex + delta];
        }

        public void MoveBy(int delta)
        {
            this.CurrentIndex += delta;
        }


        /// <summary>
        /// Gets all elements before the navigator.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> Before()
        {
            for (int i = this.CurrentIndex - 1; i >= 0; i--)
                yield return _collection[i];
        }

        /// <summary>
        /// Gets all elements after the navigator.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> After()
        {
            for (int i = this.CurrentIndex + 1; i < _collection.Count; i++)
                yield return _collection[i];
        }
        
        /// <summary>
        /// Gets all elements before the navigator.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IListItemNavigator<T>> NodesBefore()
        {
            for (int i = this.CurrentIndex - 1; i >= 0; i--)
                yield return new ReadOnlyListItemNavigator<T>(_collection, i);
        }

        /// <summary>
        /// Gets all elements before the navigator.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IListItemNavigator<T>> NodesAfter()
        {
            for (int i = this.CurrentIndex + 1; i < _collection.Count; i++)
                yield return new ReadOnlyListItemNavigator<T>(_collection, i);
        }
    }
}
