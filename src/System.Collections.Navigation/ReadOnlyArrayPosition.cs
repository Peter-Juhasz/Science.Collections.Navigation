namespace System.Collections.Navigation
{
    public ref struct ReadOnlyArrayPosition<T>
    {
        public ReadOnlyArrayPosition(T[] buffer, int index)
        {
            Buffer = buffer;
            _index = index;
            ValidateIndex(index);
        }

        public static ReadOnlyArrayPosition<T> Create(T[] array, int index) => new ReadOnlyArrayPosition<T>(array, index);

        public static ReadOnlyArrayPosition<T> Create(T[] array, Index index) => new ReadOnlyArrayPosition<T>(array, index.GetOffset(array.Length));

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
        public static ReadOnlyArrayPosition<T> GetReadOnlyPositionOf<T>(this T[] array, int index) => ReadOnlyArrayPosition<T>.Create(array, index);
        public static ReadOnlyArrayPosition<T> GetReadOnlyPositionOf<T>(this T[] array, Index index) => ReadOnlyArrayPosition<T>.Create(array, index);

        public static ReadOnlyArrayPosition<T> GetReadOnlyPositionOfStart<T>(this T[] array) => ReadOnlyArrayPosition<T>.Create(array, 0);

        public static ReadOnlyArrayPosition<T> GetReadOnlyPositionOfEnd<T>(this T[] array) => ReadOnlyArrayPosition<T>.Create(array, array.Length - 1);
    }
}
