namespace BinaryDiff
{
    using System;
    using System.Linq;
    public abstract class BinaryComparisonResult 
    {
        public abstract string Result { get; }

        public static BinaryComparisonResult Equal { get; } = new BinariesAreEqual();
        
        public static BinaryComparisonResult DifferentSize { get; } = new BinariesHaveDifferentSize();
        
        public static BinaryComparisonResult Different(Difference[] differences) 
            => new BinariesHaveDifferences(differences);

        private class BinariesAreEqual : BinaryComparisonResult
        {
            public override string Result => "same";
        }

        private class BinariesHaveDifferentSize : BinaryComparisonResult
        {
            public override string Result => "size-differs";
        }

        private class BinariesHaveDifferences : BinaryComparisonResult
        {
            public BinariesHaveDifferences(Difference[] differences)
            {
                if (!differences.Any()) throw new ArgumentException("List of differences can't be empty", nameof(differences));

                this.Differences = differences;
            }

            public override string Result => "different";

            public Difference[] Differences { get; }
        }
    }
}
