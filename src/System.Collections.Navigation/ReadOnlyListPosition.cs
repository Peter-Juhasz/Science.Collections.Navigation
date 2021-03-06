﻿using System.Collections.Generic;
using System.Linq;

namespace System.Collections.Navigation
{
    public ref struct ReadOnlyListPosition<T>
    {
        public ReadOnlyListPosition(IReadOnlyList<T> buffer, int index)
        {
            Buffer = buffer;
            _index = index;
            ValidateIndex(index);
        }

        public static ReadOnlyListPosition<T> Create(IReadOnlyList<T> list, int index) => new ReadOnlyListPosition<T>(list, index);

        public static ReadOnlyListPosition<T> Create(IReadOnlyList<T> list, Index index) => new ReadOnlyListPosition<T>(list, index.GetOffset(list.Count));

        public readonly IReadOnlyList<T> Buffer { get; }

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

        public T Value => Buffer[Index];

        public bool CanMoveNext => Index < Buffer.Count - 1;
        public bool CanMovePrevious => Index > 0;

        public void MoveNext() => Index++;
        public void MovePrevious() => Index--;

        public void MoveToStart() => Index = 0;
        public void MoveToEnd() => Index = Buffer.Count - 1;

        public bool IsStart => Index == 0;
        public bool IsEnd => Index == Buffer.Count - 1;

        public void Offset(int delta) => Index += delta;

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
            while (predicate(Value))
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
            while (predicate(Value))
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

        public bool TryMoveNext()
        {
            if (!IsEnd)
            {
                MoveNext();
                return true;
            }

            return false;
        }

        public bool TryMovePrevious()
        {
            if (!IsStart)
            {
                MovePrevious();
                return true;
            }

            return false;
        }

        private void ValidateIndex(int index)
        {
            if (index < 0 || index >= Buffer.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        public ReadOnlyListPosition<T> Clone() => new(Buffer, Index);


        public static bool operator ==(ReadOnlyListPosition<T> left, ReadOnlyListPosition<T> right) => left.Buffer == right.Buffer && left.Index == right.Index;

        public static bool operator !=(ReadOnlyListPosition<T> left, ReadOnlyListPosition<T> right) => !(left == right);

        public static bool operator <(ReadOnlyListPosition<T> left, ReadOnlyListPosition<T> right) => left.Buffer == right.Buffer && left.Index < right.Index;

        public static bool operator >(ReadOnlyListPosition<T> left, ReadOnlyListPosition<T> right) => left.Buffer == right.Buffer && left.Index > right.Index;

        public static bool operator <=(ReadOnlyListPosition<T> left, ReadOnlyListPosition<T> right) => left.Buffer == right.Buffer && left.Index <= right.Index;

        public static bool operator >=(ReadOnlyListPosition<T> left, ReadOnlyListPosition<T> right) => left.Buffer == right.Buffer && left.Index >= right.Index;

        public static ReadOnlyListPosition<T> operator +(ReadOnlyListPosition<T> left, ReadOnlyListPosition<T> right)
        {
            if (left.Buffer != right.Buffer) throw new InvalidOperationException("Buffers must be the same.");
            left.Offset(right.Index);
            return left;
        }

        public static ReadOnlyListPosition<T> operator -(ReadOnlyListPosition<T> left, ReadOnlyListPosition<T> right)
        {
            if (left.Buffer != right.Buffer) throw new InvalidOperationException("Buffers must be the same.");
            left.Offset(-right.Index);
            return left;
        }

        public static ReadOnlyListPosition<T> operator +(ReadOnlyListPosition<T> left, int right)
        {
            left.Offset(right);
            return left;
        }

        public static ReadOnlyListPosition<T> operator -(ReadOnlyListPosition<T> left, int right)
        {
            left.Offset(-right);
            return left;
        }

        public override int GetHashCode() => HashCode.Combine(Buffer, Index);
    }

    public static partial class Extensions
    {
        public static ReadOnlyListPosition<T> GetReadOnlyPositionOf<T>(this IReadOnlyList<T> list, int index) => ReadOnlyListPosition<T>.Create(list, index);
        public static ReadOnlyListPosition<T> GetReadOnlyPositionOf<T>(this IReadOnlyList<T> list, Index index) => ReadOnlyListPosition<T>.Create(list, index);

        public static ReadOnlyListPosition<T> GetReadOnlyPositionOfStart<T>(this IReadOnlyList<T> list) => ReadOnlyListPosition<T>.Create(list, 0);

        public static ReadOnlyListPosition<T> GetReadOnlyPositionOfEnd<T>(this IReadOnlyList<T> list) => ReadOnlyListPosition<T>.Create(list, list.Count - 1);
    }
}
