using System;
using System.Collections.Generic;

namespace Science.Collections.Navigation
{
    /// <summary>
    /// Provides navigation for an item of a list.
    /// </summary>
    /// <typeparam name="T">Type of the items in the list.</typeparam>
    public interface IListItemNavigator<out T>
    {
        /// <summary>
        /// Gets the current item pointed by the <see cref="IListItemNavigator{T}" />.
        /// </summary>
        T Current { get; }

        /// <summary>
        /// Gets the position of the <see cref="IListItemNavigator{T}" />.
        /// </summary>
        int CurrentIndex { get; set; }


        /// <summary>
        /// Tries to get the value in a given <paramref name="delta"/> distance.
        /// </summary>
        /// <param name="delta">The number of positions between the current <see cref="IListItemNavigator{T}" /> and the peeked value.</param>
        /// <returns>The value at the computed location.</returns>
        T Peek(int delta);

        /// <summary>
        /// Creates a new <see cref="IListItemNavigator{T}" /> in the distance of <paramref name="delta" /> from the current position.
        /// </summary>
        /// <param name="delta">The number of positions between the current and the new <see cref="IListItemNavigator{T}" />. Can be zero or negative.</param>
        IListItemNavigator<T> Offset(int delta);

        /// <summary>
        /// Moves the <see cref="IListItemNavigator{T}" /> by <paramref name="delta" />.
        /// </summary>
        /// <param name="delta">The number of positions to move. Can be zero or negative.</param>
        void MoveBy(int delta);


        /// <summary>
        /// Gets all values before the navigator.
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> Before();

        /// <summary>
        /// Gets all values after the navigator.
        /// </summary>
        IEnumerable<T> After();


        /// <summary>
        /// Gets all values before the navigator.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IListItemNavigator<T>> NodesBefore();

        /// <summary>
        /// Gets all values after the navigator.
        /// </summary>
        IEnumerable<IListItemNavigator<T>> NodesAfter();
    }

    public static partial class ListItemNavigatorExtensions
    {
        public static IListItemNavigator<T> Previous<T>(this IListItemNavigator<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));


            return source.Offset(-1);
        }

        public static IListItemNavigator<T> Next<T>(this IListItemNavigator<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));


            return source.Offset(1);
        }


        public static void MoveTo<T>(this IListItemNavigator<T> source, int index)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            
            source.CurrentIndex = index;
        }

        public static void MoveBackwards<T>(this IListItemNavigator<T> source, int amount = 1)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));


            source.MoveBy(-amount);
        }

        public static void MoveForward<T>(this IListItemNavigator<T> source, int amount = 1)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));


            source.MoveBy(amount);
        }


        public static IEnumerable<T> BeforeAndSelf<T>(this IListItemNavigator<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));


            yield return source.Current;

            foreach (T next in source.Before())
                yield return next;
        }

        public static IEnumerable<T> AfterAndSelf<T>(this IListItemNavigator<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            
            yield return source.Current;

            foreach (T next in source.After())
                yield return next;
        }

        public static IEnumerable<IListItemNavigator<T>> NodesBeforeAndSelf<T>(this IListItemNavigator<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));


            yield return source;

            foreach (IListItemNavigator<T> next in source.NodesBefore())
                yield return next;
        }

        public static IEnumerable<IListItemNavigator<T>> NodesAfterAndSelf<T>(this IListItemNavigator<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));


            yield return source;

            foreach (IListItemNavigator<T> next in source.NodesAfter())
                yield return next;
        }
    }
}