using System;
using System.Collections.Generic;

namespace Science.Collections.Navigation
{
    public static partial class ListItemNavigatorExtensions
    {
        internal static IWriteableListItemNavigator<T> AsTracking<T>(this IWriteableListItemNavigator<T> source, ListItemTrackingMode trackingMode)
        {
            if (source is TrackingListItemNavigator<T>)
                return source;

            return new TrackingListItemNavigator<T>(source, trackingMode);
        }
    }

    internal class TrackingListItemNavigator<T> : IWriteableListItemNavigator<T>
    {
        public TrackingListItemNavigator(IWriteableListItemNavigator<T> navigator, ListItemTrackingMode trackingMode)
        {
            if (navigator == null)
                throw new ArgumentNullException(nameof(navigator));
            
            _navigator = navigator;
            _trackingMode = trackingMode;
        }
        
        private readonly IWriteableListItemNavigator<T> _navigator;
        private readonly ListItemTrackingMode _trackingMode;


        /// <summary>
        /// Gets or sets the position of the navigator.
        /// </summary>
        public int CurrentIndex
        {
            get { return _navigator.CurrentIndex; }
            set { _navigator.CurrentIndex = value; }
        }

        /// <summary>
        /// Gets the current value pointed at.
        /// </summary>
        public T Current
        {
            get { return _navigator.Current; }
            set { _navigator.Current = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        /// <returns></returns>
        public IWriteableListItemNavigator<T> Offset(int delta)
        {
            return _navigator.Offset(delta);
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
            return _navigator.Peek(delta);
        }
        
        public void MoveBy(int delta)
        {
            _navigator.MoveBy(delta);
        }


        /// <summary>
        /// Gets all elements before the navigator.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> Before()
        {
            return _navigator.Before();
        }

        /// <summary>
        /// Gets all elements after the navigator.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> After()
        {
            return _navigator.After();
        }

        /// <summary>
        /// Gets all elements before the navigator.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IWriteableListItemNavigator<T>> NodesBefore()
        {
            return _navigator.NodesBefore();
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
            return _navigator.NodesAfter();
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
            _navigator.InsertBefore(item);
        }

        /// <summary>
        /// Inserts an item at the next position.
        /// </summary>
        /// <param name="item"></param>
        public void InsertAfter(T item)
        {
            _navigator.InsertAfter(item);
        }

        /// <summary>
        /// Removes the pointed item.
        /// </summary>
        public void Remove()
        {
            _navigator.Remove();
        }
    }
}
