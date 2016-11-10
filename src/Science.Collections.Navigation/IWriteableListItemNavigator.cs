using System;
using System.Collections.Generic;

namespace Science.Collections.Navigation
{
    public interface IWriteableListItemNavigator<T> : IEditableListItemNavigator<T>
    {
        new IWriteableListItemNavigator<T> Offset(int delta);


        new IEnumerable<IWriteableListItemNavigator<T>> NodesBefore();

        new IEnumerable<IWriteableListItemNavigator<T>> NodesAfter();


        void Remove();

        void InsertBefore(T item);

        void InsertAfter(T item);
    }

    public static class WriteableListItemNavigatorExtensions
    {
        public static IWriteableListItemNavigator<T> Previous<T>(this IWriteableListItemNavigator<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));


            return source.Offset(-1);
        }

        public static IWriteableListItemNavigator<T> Next<T>(this IWriteableListItemNavigator<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));


            return source.Offset(1);
        }


        public static IEnumerable<IWriteableListItemNavigator<T>> NodesBeforeAndSelf<T>(this IWriteableListItemNavigator<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));


            yield return source;

            foreach (IWriteableListItemNavigator<T> next in source.NodesBefore())
                yield return next;
        }

        public static IEnumerable<IWriteableListItemNavigator<T>> NodesAfterAndSelf<T>(this IWriteableListItemNavigator<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));


            yield return source;

            foreach (IWriteableListItemNavigator<T> next in source.NodesAfter())
                yield return next;
        }
    }
}