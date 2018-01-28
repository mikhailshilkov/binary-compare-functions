namespace BinaryDiff
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class BinaryComparer
    {
        public static BinaryComparisonResult Compare(byte[] leftBinary, byte[] rightBinary)
        {
            if (leftBinary == null) throw new ArgumentNullException(nameof(leftBinary));
            if (rightBinary == null) throw new ArgumentNullException(nameof(rightBinary));

            if (leftBinary.Length != rightBinary.Length)
            {
                return BinaryComparisonResult.DifferentSize;
            }

            var differences = FindDifferences(leftBinary, rightBinary).ToArray();
            return differences.Any() 
                ? BinaryComparisonResult.Different(differences) 
                : BinaryComparisonResult.Equal;
        }

        private static IEnumerable<Difference> FindDifferences(byte[] leftBinary, byte[] rightBinary)
        {
            int? differenceStart = null;
            for (int i = 0; i < leftBinary.Length; i++)
            {
                bool bytesAreSame = leftBinary[i] == rightBinary[i];
                if (!bytesAreSame && !differenceStart.HasValue)
                {
                    differenceStart = i;
                }
                else if (bytesAreSame && differenceStart.HasValue)
                {
                    yield return new Difference(
                        startIndex: differenceStart.Value, 
                        length: i - differenceStart.Value); 
                    differenceStart = null;
                }
            }

            if (differenceStart.HasValue)
            {
                yield return new Difference(
                    startIndex: differenceStart.Value, 
                    length: leftBinary.Length - differenceStart.Value);
            }
        }
    }
}
