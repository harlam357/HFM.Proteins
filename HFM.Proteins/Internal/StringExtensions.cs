using System;
using System.Buffers;

namespace HFM.Proteins.Internal
{
    internal static class StringExtensions
    {
        // https://www.meziantou.net/split-a-string-into-lines-without-allocation.htm
        internal static LineSplitEnumerator SplitLines(this Span<char> str) => new LineSplitEnumerator(str);

        internal ref struct LineSplitEnumerator
        {
            private ReadOnlySpan<char> _str;

            public LineSplitEnumerator(ReadOnlySpan<char> str)
            {
                _str = str;
                Current = default;
            }

            public LineSplitEnumerator GetEnumerator() => this;

            public bool MoveNext()
            {
                var span = _str;
                if (span.Length == 0)
                {
                    return false;
                }

                var index = span.IndexOfAny('\r', '\n');
                if (index == -1)
                {
                    _str = ReadOnlySpan<char>.Empty;
                    Current = new LineSplitEntry(span, ReadOnlySpan<char>.Empty);
                    return true;
                }

                if (index < span.Length - 1 && span[index] == '\r')
                {
                    // Try to consume the '\n' associated to the '\r'
                    var next = span[index + 1];
                    if (next == '\n')
                    {
                        Current = new LineSplitEntry(span[..index], span.Slice(index, 2));
                        _str = span[(index + 2)..];
                        return true;
                    }
                }

                Current = new LineSplitEntry(span[..index], span.Slice(index, 1));
                _str = span[(index + 1)..];
                return true;
            }

            public LineSplitEntry Current { get; private set; }
        }

        internal readonly ref struct LineSplitEntry
        {
            public LineSplitEntry(ReadOnlySpan<char> line, ReadOnlySpan<char> separator)
            {
                Line = line;
                Separator = separator;
            }

            public ReadOnlySpan<char> Line { get; }
            public ReadOnlySpan<char> Separator { get; }
            public bool HasSeparator => Separator.Length > 0;

            public static implicit operator ReadOnlySpan<char>(LineSplitEntry entry) => entry.Line;
        }

        internal static TabSplitEnumerator SplitTabs(this ReadOnlySpan<char> str) => new TabSplitEnumerator(str);

        internal ref struct TabSplitEnumerator
        {
            private ReadOnlySpan<char> _str;

            public TabSplitEnumerator(ReadOnlySpan<char> str)
            {
                _str = str;
                Current = default;
            }

            public TabSplitEnumerator GetEnumerator() => this;

            public bool MoveNext()
            {
                var span = _str;
                if (span.Length == 0)
                {
                    return false;
                }

                var index = span.IndexOf('\t');
                if (index == -1)
                {
                    _str = ReadOnlySpan<char>.Empty;
                    Current = span;
                    return true;
                }

                Current = span[..index];
                _str = span[(index + 1)..];
                return true;
            }

            public ReadOnlySpan<char> Current { get; private set; }
        }

        internal static ReadOnlySpan<char> Concat(this char[] first, ReadOnlySpan<char> second)
        {
            int length = first.Length + second.Length;
            char[] temp = ArrayPool<char>.Shared.Rent(length);
            for (int i = 0; i < first.Length; i++)
            {
                temp[i] = first[i];
            }
            for (int i = 0; i < second.Length; i++)
            {
                temp[i + first.Length] = second[i];
            }

            var result = temp.AsSpan()[..length];
            ArrayPool<char>.Shared.Return(temp);
            return result;
        }
    }
}
