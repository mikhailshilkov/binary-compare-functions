namespace BinaryDiff
{
    using System;

    public class Difference
    {
        public Difference(int startIndex, int length)
        {
            if (startIndex < 0) throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));

            this.StartIndex = startIndex;
            this.Length = length;
        }

        public int StartIndex { get; }
        public int Length { get; }
    }
}
