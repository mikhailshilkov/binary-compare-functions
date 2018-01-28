namespace BinaryDiff
{
    using System;
    using System.Linq;
    
    public class BinariesHaveDifferences : IBinaryComparisonResult
    {
        public BinariesHaveDifferences(Difference[] differences)
        {
            if (!differences.Any()) throw new ArgumentException("List of differences can't be empty", nameof(differences));

            this.Differences = differences;
        }

        public string Result => "different";

        public Difference[] Differences { get; }
    }
}
