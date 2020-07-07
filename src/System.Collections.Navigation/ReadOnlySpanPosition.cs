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

        private void ValidateIndex(int index)
        {
            if (index < 0 || index >= Buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }
    }

    public static partial class Extensions
    {
        public static ReadOnlySpanPosition<T> GetReadOnlyPositionOf<T>(this ReadOnlySpan<T> list, int index) => ReadOnlySpanPosition<T>.Create(list, index);
        public static ReadOnlySpanPosition<T> GetReadOnlyPositionOf<T>(this ReadOnlySpan<T> list, Index index) => ReadOnlySpanPosition<T>.Create(list, index);

        public static ReadOnlySpanPosition<T> GetReadOnlyPositionOfStart<T>(this ReadOnlySpan<T> list) => ReadOnlySpanPosition<T>.Create(list, 0);

        public static ReadOnlySpanPosition<T> GetReadOnlyPositionOfEnd<T>(this ReadOnlySpan<T> list) => ReadOnlySpanPosition<T>.Create(list, list.Length - 1);
    }
}
