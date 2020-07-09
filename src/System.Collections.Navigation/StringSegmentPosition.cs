using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Linq;

namespace System.Collections.Navigation
{
    public ref struct StringSegmentPosition
    {
        public StringSegmentPosition(StringSegment buffer, int index)
        {
            Buffer = buffer;
            _index = index;
            ValidateIndex(index);
        }

        public static StringSegmentPosition Create(StringSegment str, int index) => new StringSegmentPosition(str, index);

        public static StringSegmentPosition Create(StringSegment str, Index index) => new StringSegmentPosition(str, index.GetOffset(str.Length));

        public readonly StringSegment Buffer { get; }

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

        public char Value => Buffer[Index];

        public bool CanMoveNext => Index < Buffer.Length - 1;
        public bool CanMovePrevious => Index > 0;

        public void MoveNext() => Index++;
        public void MovePrevious() => Index--;

        public void MoveToStart() => Index = 0;
        public void MoveToEnd() => Index = Buffer.Length - 1;

        public bool IsStart => Index == 0;
        public bool IsEnd => Index == Buffer.Length - 1;

        public void Offset(int delta) => Index += delta;

        public char Peek(int delta) => Buffer[Index + delta];

        public bool TryPeek(int delta, out char value)
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

        public StringSegment AllBefore() => Buffer.Subsegment(0, Index);

        public StringSegment AllAfter() => Buffer.Subsegment(Index + 1);

        public StringSegment Slice(int length) => Buffer.Subsegment(Index, length);

        public int MoveForwardWhile(Predicate<char> predicate)
        {
            var start = Index;
            while (predicate(Value))
                MoveNext();

            return Index - start;
        }

        public int MoveForwardTo(char value, IEqualityComparer<char> comparer) =>
            MoveForwardWhile(v => !comparer.Equals(v, value));

        public int MoveForwardTo(char value) =>
            MoveForwardTo(value, EqualityComparer<char>.Default);

        public int MoveForwardToAny(IEnumerable<char> values, IEqualityComparer<char> comparer) =>
            MoveForwardWhile(v => values.All(r => !comparer.Equals(v, r)));

        public int MoveForwardToAny(IEnumerable<char> values) =>
            MoveForwardToAny(values, EqualityComparer<char>.Default);

        public int MoveBackwardsWhile(Predicate<char> predicate)
        {
            var start = Index;
            while (predicate(Value))
                MovePrevious();

            return start - Index;
        }

        public int MoveBackwardsTo(char value, IEqualityComparer<char> comparer) =>
            MoveBackwardsWhile(v => !comparer.Equals(v, value));

        public int MoveBackwardsTo(char value) =>
            MoveBackwardsTo(value, EqualityComparer<char>.Default);

        public int MoveBackwardsToAny(IEnumerable<char> values, IEqualityComparer<char> comparer) =>
            MoveBackwardsWhile(v => values.All(r => !comparer.Equals(v, r)));

        public int MoveBackwardsToAny(IEnumerable<char> values) =>
            MoveBackwardsToAny(values, EqualityComparer<char>.Default);

        private void ValidateIndex(int index)
        {
            if (index < 0 || index >= Buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        public StringSegmentPosition Clone() => new(Buffer, Index);

        public static bool operator ==(StringSegmentPosition left, StringSegmentPosition right) => left.Buffer == right.Buffer && left.Index == right.Index;

        public static bool operator !=(StringSegmentPosition left, StringSegmentPosition right) => !(left == right);

        public override int GetHashCode() => HashCode.Combine(Buffer, Index);
    }

    public static partial class Extensions
    {
        public static StringSegmentPosition GetPositionOf(this StringSegment str, int index) => StringSegmentPosition.Create(str, index);
        public static StringSegmentPosition GetPositionOf(this StringSegment str, Index index) => StringSegmentPosition.Create(str, index);

        public static StringSegmentPosition GetPositionOfStart(this StringSegment str) => StringSegmentPosition.Create(str, 0);

        public static StringSegmentPosition GetPositionOfEnd(this StringSegment str) => StringSegmentPosition.Create(str, str.Length - 1);
    }
}
