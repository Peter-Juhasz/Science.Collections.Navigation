using System;
using System.Collections.Generic;

namespace Science.Collections.Navigation
{
    public interface IEditableListItemNavigator<T> : IListItemNavigator<T>
    {
        /// <summary>
        /// Gets or sets the value pointed by the <see cref="IEditableListItemNavigator{T}"/>.
        /// </summary>
        new T Current { get; set; }

        new IEditableListItemNavigator<T> Offset(int delta);


        new IEnumerable<IEditableListItemNavigator<T>> NodesBefore();

        new IEnumerable<IEditableListItemNavigator<T>> NodesAfter();
    }

    public static class EditableListItemNavigatorExtensions
    {
        public static IEditableListItemNavigator<T> Previous<T>(this IEditableListItemNavigator<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));


            return source.Offset(-1);
        }

        public static IEditableListItemNavigator<T> Next<T>(this IEditableListItemNavigator<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));


            return source.Offset(1);
        }


        public static void Replace<T>(this IEditableListItemNavigator<T> source, T newValue)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));


            source.Current = newValue;
        }


        public static IEnumerable<IEditableListItemNavigator<T>> NodesBeforeAndSelf<T>(this IEditableListItemNavigator<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));


            yield return source;

            foreach (IEditableListItemNavigator<T> next in source.NodesBefore())
                yield return next;
        }

        public static IEnumerable<IEditableListItemNavigator<T>> NodesAfterAndSelf<T>(this IEditableListItemNavigator<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));


            yield return source;

            foreach (IEditableListItemNavigator<T> next in source.NodesAfter())
                yield return next;
        }
    }
}