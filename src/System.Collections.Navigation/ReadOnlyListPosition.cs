﻿using System.Collections.Generic;

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

        public T Peek(int delta)
        {
            var newIndex = Index + delta;
            if (newIndex < 0 || newIndex >= Buffer.Count)
            {
                return default!;
            }

            return Buffer[newIndex];
        }

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
        public static ReadOnlyListPosition<T> GetReadOnlyPositionOf<T>(this IReadOnlyList<T> list, int index) => ReadOnlyListPosition<T>.Create(list, index);
        public static ReadOnlyListPosition<T> GetReadOnlyPositionOf<T>(this IReadOnlyList<T> list, Index index) => ReadOnlyListPosition<T>.Create(list, index);

        public static ReadOnlyListPosition<T> GetReadOnlyPositionOfStart<T>(this IReadOnlyList<T> list) => ReadOnlyListPosition<T>.Create(list, 0);

        public static ReadOnlyListPosition<T> GetReadOnlyPositionOfEnd<T>(this IReadOnlyList<T> list) => ReadOnlyListPosition<T>.Create(list, list.Count - 1);
    }
}