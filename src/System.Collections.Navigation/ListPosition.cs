using System.Collections.Generic;
using System.Linq;

namespace System.Collections.Navigation
{
    public ref struct ListPosition<T>
    {
        public ListPosition(IList<T> buffer, int index)
        {
            Buffer = buffer;
            _index = index;
            ValidateIndex(index);
        }

        public static ListPosition<T> Create(IList<T> list, int index) => new ListPosition<T>(list, index);

        public static ListPosition<T> Create(IList<T> list, Index index) => new ListPosition<T>(list, index.GetOffset(list.Count));

        public readonly IList<T> Buffer { get; }

        private int _index;
        public int Index
        {
            get => _index;
            set
            {
                ValidateIndex(value);
                _index = value;
            }
        }

        private bool IsStalled => Buffer.Count == 0 && Index == 0;
        private bool IsOutOfRange => Index < 0 || Index >= Buffer.Count;
        private bool IsHead => Buffer.Count == Index;

        public T Value
        {
            get => Buffer[Index];
            set => Buffer[Index] = value;
        }

        public bool CanMoveNext => Index < Buffer.Count - 1;
        public bool CanMovePrevious => Index > 0;

        public void MoveNext() => Index++;
        public void MovePrevious() => Index--;

        public void MoveToStart() => Index = 0;
        public void MoveToEnd() => Index = Buffer.Count - 1;

        public bool IsStart => Index == 0;
        public bool IsEnd => Index == Buffer.Count - 1;

        public void Offset(int delta) => Index += delta;

        public T Read() => Value;

        public void Write(T value) => Buffer[Index] = value;

        public void WriteAndAdvance(T value)
        {
            Write(value);
            if (!IsHead) MoveNext();
        }

        public void Remove()
        {
            Buffer.RemoveAt(Index);
        }

        public void Insert(T value)
        {
            Buffer.Insert(Index, value);
        }

        public void InsertAndAdvance(T value)
        {
            Buffer.Insert(Index, value);
            MoveNext();
        }

        public T Peek(int delta) => Buffer[Index + delta];

        public bool TryPeek(int delta, out T value)
        {
            var newIndex = Index + delta;
            if (newIndex < 0 || newIndex >= Buffer.Count)
            {
                value = default!;
                return false;
            }

            value = Buffer[newIndex];
            return true;
        }

        public int MoveForwardWhile(Predicate<T> predicate)
        {
            var start = Index;
            while (predicate(Value) && !IsEnd)
                MoveNext();

            return Index - start;
        }

        public int MoveForwardTo(T value, IEqualityComparer<T> comparer) =>
            MoveForwardWhile(v => !comparer.Equals(v, value));

        public int MoveForwardTo(T value) =>
            MoveForwardTo(value, EqualityComparer<T>.Default);

        public int MoveForwardToAny(IEnumerable<T> values, IEqualityComparer<T> comparer) =>
            MoveForwardWhile(v => values.All(r => !comparer.Equals(v, r)));

        public int MoveForwardToAny(IEnumerable<T> values) =>
            MoveForwardToAny(values, EqualityComparer<T>.Default);

        public int MoveBackwardsWhile(Predicate<T> predicate)
        {
            var start = Index;
            while (predicate(Value) && !IsStart)
                MovePrevious();

            return start - Index;
        }

        public int MoveBackwardsTo(T value, IEqualityComparer<T> comparer) =>
            MoveBackwardsWhile(v => !comparer.Equals(v, value));

        public int MoveBackwardsTo(T value) =>
            MoveBackwardsTo(value, EqualityComparer<T>.Default);

        public int MoveBackwardsToAny(IEnumerable<T> values, IEqualityComparer<T> comparer) =>
            MoveBackwardsWhile(v => values.All(r => !comparer.Equals(v, r)));

        public int MoveBackwardsToAny(IEnumerable<T> values) =>
            MoveBackwardsToAny(values, EqualityComparer<T>.Default);

        private void ValidateIndex(int index)
        {
            if (index < 0 || index >= Buffer.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }
    }

    public static partial class Extensions
    {
        public static ListPosition<T> GetPositionOf<T>(this IList<T> list, int index) => ListPosition<T>.Create(list, index);
        public static ListPosition<T> GetPositionOf<T>(this IList<T> list, Index index) => ListPosition<T>.Create(list, index);

        public static ListPosition<T> GetPositionOfStart<T>(this IList<T> list) => ListPosition<T>.Create(list, 0);

        public static ListPosition<T> GetPositionOfEnd<T>(this IList<T> list) => ListPosition<T>.Create(list, Math.Max(list.Count - 1, 0));
        public static ListPosition<T> GetPositionOfHead<T>(this IList<T> list) => ListPosition<T>.Create(list, list.Count);
    }
}
