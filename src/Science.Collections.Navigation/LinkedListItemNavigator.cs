using System;
using System.Collections.Generic;

namespace Science.Collections.Navigation
{
    public static class LinkedListExtensions
    {
        public static IWriteableListItemNavigator<T> GetNavigator<T>(this LinkedList<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            
            return new LinkedListItemNavigator<T>(source.First);
        }

        public static IWriteableListItemNavigator<T> GetNavigator<T>(this LinkedListNode<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));


            return new LinkedListItemNavigator<T>(source);
        }
    }

    public class LinkedListItemNavigator<T> :
        IListItemNavigator<T>,
        IEditableListItemNavigator<T>,
        IWriteableListItemNavigator<T>
    {
        public LinkedListItemNavigator(LinkedListNode<T> node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            _node = node;
        }

        private LinkedListNode<T> _node;
        
        protected LinkedList<T> List => _node.List;


        /// <summary>
        /// Gets or sets the position of the navigator.
        /// </summary>
        /// <remarks>Performance critical.</remarks>
        public int CurrentIndex
        {
            get
            {
                int countOfNodesBefore = 0;
                LinkedListNode<T> current = _node;

                while (current.Previous != null)
                {
                    countOfNodesBefore++;
                    current = current.Previous;
                }
                
                return countOfNodesBefore;
            }
            set
            {
                if (value < 0 || value >= this.List.Count)
                    throw new ArgumentOutOfRangeException(nameof(value));
                
                int delta = value - this.CurrentIndex;

                this.MoveBy(delta);
            }
        }

        /// <summary>
        /// Gets the current value pointed at.
        /// </summary>
        public T Current
        {
            get { return _node.Value; }
            set { _node.Value = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        /// <returns></returns>
        public IWriteableListItemNavigator<T> Offset(int delta)
        {
            if (delta < 0)
            {
                LinkedListNode<T> current = _node;

                for (int i = 0; delta < i; i--)
                {
                    if (current.Previous == null)
                        throw new ArgumentOutOfRangeException(nameof(delta));

                    current = current.Previous;
                }

                return new LinkedListItemNavigator<T>(current);
            }
            else if (delta > 0)
            {
                LinkedListNode<T> current = _node;

                for (int i = 0; i < delta; i++)
                {
                    if (current.Next == null)
                        throw new ArgumentOutOfRangeException(nameof(delta));

                    current = current.Next;
                }

                return new LinkedListItemNavigator<T>(current);
            }
            else
                return this;
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
            if (delta < 0)
            {
                LinkedListNode<T> current = _node;

                for (int i = 0; delta < i; i--)
                {
                    if (current.Previous == null)
                        return default(T);

                    current = current.Previous;
                }

                return current.Value;
            }
            else if (delta > 0)
            {
                LinkedListNode<T> current = _node;

                for (int i = 0; i < delta; i++)
                {
                    if (current.Next == null)
                        return default(T);

                    current = current.Next;
                }

                return current.Value;
            }
            else
                return _node.Value;
        }
        
        public void MoveBy(int delta)
        {
            if (delta < 0)
            {
                LinkedListNode<T> current = _node;

                for (int i = 0; delta < i; i--)
                {
                    if (current.Previous == null)
                        throw new ArgumentOutOfRangeException(nameof(delta));

                    current = current.Previous;
                }

                _node = current;
            }
            else if (delta > 0)
            {
                LinkedListNode<T> current = _node;

                for (int i = 0; i < delta; i++)
                {
                    if (current.Next == null)
                        throw new ArgumentOutOfRangeException(nameof(delta));

                    current = current.Next;
                }

                _node = current;
            }
        }


        /// <summary>
        /// Gets all elements before the navigator.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> Before()
        {
            LinkedListNode<T> current = _node.Previous;

            while (current != null)
            {
                yield return current.Value;

                current = current.Previous;
            }
        }

        /// <summary>
        /// Gets all elements after the navigator.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> After()
        {
            LinkedListNode<T> current = _node.Next;

            while (current != null)
            {
                yield return current.Value;

                current = current.Next;
            }
        }

        public IEnumerable<IWriteableListItemNavigator<T>> NodesBefore()
        {
            LinkedListNode<T> current = _node.Previous;

            while (current != null)
            {
                yield return new LinkedListItemNavigator<T>(current);

                current = current.Previous;
            }
        }
        IEnumerable<IEditableListItemNavigator<T>> IEditableListItemNavigator<T>.NodesBefore()
        {
            return this.NodesBefore();
        }
        IEnumerable<IListItemNavigator<T>> IListItemNavigator<T>.NodesBefore()
        {
            return this.NodesBefore();
        }

        public IEnumerable<IWriteableListItemNavigator<T>> NodesAfter()
        {
            LinkedListNode<T> current = _node.Next;

            while (current != null)
            {
                yield return new LinkedListItemNavigator<T>(current);

                current = current.Next;
            }
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
            _node.List.AddBefore(_node, item);
        }

        /// <summary>
        /// Inserts an item at the next position.
        /// </summary>
        /// <param name="item"></param>
        public void InsertAfter(T item)
        {
            _node.List.AddAfter(_node, item);
        }

        /// <summary>
        /// Removes the pointed item.
        /// </summary>
        public void Remove()
        {
            if (this.List.Count == 1)
                throw new InvalidOperationException("At least one item must remain in the collection the navigator can point to.");

            // snap to the remaining nodes
            LinkedListNode<T> newCurrent = _node.Next ?? _node.Previous;

            // remove
            this.List.Remove(_node);

            // set new node as current
            _node = newCurrent;
        }
    }
}
