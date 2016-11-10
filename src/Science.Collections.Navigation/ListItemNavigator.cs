using System;
using System.Collections.Generic;

namespace Science.Collections.Navigation
{
    public static class ListExtensions
    {
        public static IEditableListItemNavigator<T> GetNavigator<T>(this T[] source, int index = 0)
        {
            return new ListItemNavigator<T>(source, index);
        }

        public static IWriteableListItemNavigator<T> GetNavigator<T>(this IList<T> source, int index = 0)
        {
            return new ListItemNavigator<T>(source, index);
        }
    }

    public class ListItemNavigator<T> :
        IListItemNavigator<T>,
        IEditableListItemNavigator<T>,
        IWriteableListItemNavigator<T>
    {
        public ListItemNavigator(IList<T> collection, int index = 0)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (index < 0 || index >= collection.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            _list = collection;
            _index = index;
        }

        private readonly IList<T> _list;
        private int _index;
        

        /// <summary>
        /// Gets or sets the position of the navigator.
        /// </summary>
        public int CurrentIndex
        {
            get { return _index; }
            set
            {
                if (value < 0 || value >= _list.Count)
                    throw new ArgumentOutOfRangeException(nameof(value));

                _index = value;
            }
        }

        /// <summary>
        /// Gets the current value pointed at.
        /// </summary>
        public T Current
        {
            get { return _list[this.CurrentIndex]; }
            set { _list[this.CurrentIndex] = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        /// <returns></returns>
        public IWriteableListItemNavigator<T> Offset(int delta)
        {
            if (this.CurrentIndex + delta < 0 ||
                this.CurrentIndex + delta >= _list.Count)
                throw new ArgumentOutOfRangeException(nameof(delta));

            return new ListItemNavigator<T>(_list, this.CurrentIndex + delta);
        }
        IEditableListItemNavigator<T> IEditableListItemNavigator<T>.Offset(int delta)
        {
            return this.Offset(delta);
        }
        IListItemNavigator<T> IListItemNavigator<T>.Offset(int delta)
        {
            return this.Offset(delta);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        /// <returns>The element at the peeked position.</returns>
        public T Peek(int delta)
        {
            if (this.CurrentIndex + delta < 0 ||
                this.CurrentIndex + delta >= _list.Count)
                return default(T);

            return _list[this.CurrentIndex + delta];
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
                yield return _list[i];
        }

        /// <summary>
        /// Gets all elements after the navigator.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> After()
        {
            for (int i = this.CurrentIndex + 1; i < _list.Count; i++)
                yield return _list[i];
        }

        /// <summary>
        /// Gets all elements before the navigator.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IWriteableListItemNavigator<T>> NodesBefore()
        {
            for (int i = this.CurrentIndex - 1; i >= 0; i--)
                yield return new ListItemNavigator<T>(_list, i);
        }
        IEnumerable<IEditableListItemNavigator<T>> IEditableListItemNavigator<T>.NodesBefore()
        {
            return this.NodesBefore();
        }
        IEnumerable<IListItemNavigator<T>> IListItemNavigator<T>.NodesBefore()
        {
            return this.NodesBefore();
        }

        /// <summary>
        /// Gets all elements after the navigator.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IWriteableListItemNavigator<T>> NodesAfter()
        {
            for (int i = this.CurrentIndex + 1; i < _list.Count; i++)
                yield return new ListItemNavigator<T>(_list, i);
        }
        IEnumerable<IEditableListItemNavigator<T>> IEditableListItemNavigator<T>.NodesAfter()
        {
            return this.NodesAfter();
        }
        IEnumerable<IListItemNavigator<T>> IListItemNavigator<T>.NodesAfter()
        {
            return this.NodesAfter();
        }


        /// <summary>
        /// Inserts an item at the current position.
        /// </summary>
        /// <param name="item"></param>
        public void InsertBefore(T item)
        {
            _list.Insert(this.CurrentIndex, item);
        }

        /// <summary>
        /// Inserts an item at the next position.
        /// </summary>
        /// <param name="item"></param>
        public void InsertAfter(T item)
        {
            _list.Insert(this.CurrentIndex + 1, item);
        }

        /// <summary>
        /// Removes the pointed item.
        /// </summary>
        public void Remove()
        {
            if (_list.Count == 1)
                throw new InvalidOperationException("At least one item must remain in the collection the navigator can point to.");

            _list.RemoveAt(this.CurrentIndex);

            // adjust pointer
            if (this.CurrentIndex == _list.Count)
                this.CurrentIndex--;
        }
    }
}
