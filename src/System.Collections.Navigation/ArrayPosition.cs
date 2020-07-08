using System.Collections.Generic;
using System.Linq;

namespace System.Collections.Navigation
{
    public ref struct ArrayPosition<T>
    {
        public ArrayPosition(T[] buffer, int index)
        {
            Buffer = buffer;
            _index = index;
            ValidateIndex(index);
        }

        public static ArrayPosition<T> Create(T[] array, int index) => new ArrayPosition<T>(array, index);

        public static ArrayPosition<T> Create(T[] array, Index index) => new ArrayPosition<T>(array, index.GetOffset(array.Length));

        public readonly T[] Buffer { get; }

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

        public ref T Value => ref Buffer[Index];

        public bool CanMoveNext => Index < Buffer.Length - 1;
        public bool CanMovePrevious => Index > 0;

        public void MoveNext() => Index++;
        public void MovePrevious() => Index--;

        public void MoveToStart() => Index = 0;
        public void MoveToEnd() => Index = Buffer.Length - 1;

        public bool IsStart => Index == 0;
        public bool IsEnd => Index == Buffer.Length - 1;

        public void Offset(int delta) => Index += delta;

        public T Read() => Value;

        public void Write(T value) => Buffer[Index] = value;

        public void WriteAndAdvance(T value)
        {
            Write(value);
            if (!IsEnd) MoveNext();
        }

        public T Peek(int delta) => Buffer[Index + delta];

        public bool TryPeek(int delta, out T value)
        {
            var newIndex = Index + delta;
            if (newIndex < 0 || newIndex >= Buffer.Length)
            {
                value = default!;
                return false;
            }

            value = Buffer[newIndex];
            return true;
        }

        public Span<T> AllBefore() => Buffer[..Index];

        public Span<T> AllAfter() => Buffer[(Index + 1)..];

        public Span<T> Slice(int length) => Buffer[Index..(Index + length)];

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
            if (index < 0 || index >= Buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        public ArrayPosition<T> Clone() => new(Buffer, Index);


        public static bool operator ==(ArrayPosition<T> left, ArrayPosition<T> right) => left.Buffer == right.Buffer && left.Index == right.Index;

        public static bool operator !=(ArrayPosition<T> left, ArrayPosition<T> right) => !(left == right);

        public override int GetHashCode() => HashCode.Combine(Buffer, Index);
    }

    public static partial class Extensions
    {
        public static ArrayPosition<T> GetPositionOf<T>(this T[] array, int index) => ArrayPosition<T>.Create(array, index);
        public static ArrayPosition<T> GetPositionOf<T>(this T[] array, Index index) => ArrayPosition<T>.Create(array, index);

        public static ArrayPosition<T> GetPositionOfStart<T>(this T[] array) => ArrayPosition<T>.Create(array, 0);

        public static ArrayPosition<T> GetPositionOfEnd<T>(this T[] array) => ArrayPosition<T>.Create(array, array.Length - 1);
    }
}
