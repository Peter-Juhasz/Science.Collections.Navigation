﻿using System.Collections.Generic;
using System.Linq;

namespace System.Collections.Navigation
{
    public ref struct ReadOnlySpanPosition<T>
    {
        public ReadOnlySpanPosition(ReadOnlySpan<T> buffer, int index)
        {
            Buffer = buffer;
            _index = index;
            ValidateIndex(index);
        }

        public static ReadOnlySpanPosition<T> Create(ReadOnlySpan<T> list, int index) => new ReadOnlySpanPosition<T>(list, index);

        public static ReadOnlySpanPosition<T> Create(ReadOnlySpan<T> list, Index index) => new ReadOnlySpanPosition<T>(list, index.GetOffset(list.Length));

        public readonly ReadOnlySpan<T> Buffer { get; }

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

        public bool CanMoveNext => Index < Buffer.Length - 1;
        public bool CanMovePrevious => Index > 0;

        public void MoveNext() => Index++;
        public void MovePrevious() => Index--;

        public void MoveToStart() => Index = 0;
        public void MoveToEnd() => Index = Buffer.Length - 1;

        public bool IsStart => Index == 0;
        public bool IsEnd => Index == Buffer.Length - 1;

        public void Offset(int delta) => Index += delta;

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

        public ReadOnlySpan<T> AllBefore() => Buffer[..Index];

        public ReadOnlySpan<T> AllAfter() => Buffer[(Index + 1)..];

        public ReadOnlySpan<T> Slice(int length) => Buffer.Slice(Index, length);

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
            if (index < 0 || index >= Buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        public ReadOnlySpanPosition<T> Clone() => new(Buffer, Index);


        public static bool operator ==(ReadOnlySpanPosition<T> left, ReadOnlySpanPosition<T> right) => left.Buffer == right.Buffer && left.Index == right.Index;

        public static bool operator !=(ReadOnlySpanPosition<T> left, ReadOnlySpanPosition<T> right) => !(left == right);
    }

    public static partial class Extensions
    {
        public static ReadOnlySpanPosition<T> GetReadOnlyPositionOf<T>(this ReadOnlySpan<T> list, int index) => ReadOnlySpanPosition<T>.Create(list, index);
        public static ReadOnlySpanPosition<T> GetReadOnlyPositionOf<T>(this ReadOnlySpan<T> list, Index index) => ReadOnlySpanPosition<T>.Create(list, index);

        public static ReadOnlySpanPosition<T> GetReadOnlyPositionOfStart<T>(this ReadOnlySpan<T> list) => ReadOnlySpanPosition<T>.Create(list, 0);

        public static ReadOnlySpanPosition<T> GetReadOnlyPositionOfEnd<T>(this ReadOnlySpan<T> list) => ReadOnlySpanPosition<T>.Create(list, list.Length - 1);
    }
}
