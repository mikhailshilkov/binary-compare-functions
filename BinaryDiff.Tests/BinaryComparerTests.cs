namespace BinaryDiff.Tests
{
    using System;
    using System.Linq;

    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class BinaryComparerTests
    {
        [Test]
        public void Compare_WithNullArgument_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => BinaryComparer.Compare(null, new byte[1]));
            Assert.Throws<ArgumentNullException>(() => BinaryComparer.Compare(new byte[2], null));
        }

        [Test]
        public void Compare_TwoEmptyArrays_AreSame()
        {
            var binary1 = new byte[0];
            var binary2 = new byte[0];
            var result = BinaryComparer.Compare(binary1, binary2);
            result.ShouldBeEquivalentTo(BinaryComparisonResult.Equal);
        }

        [Test]
        public void Compare_ArrayAndItsCopy_AreSame()
        {
            var binary1 = new byte[] { 1, 5, 9, 0 };
            var binary2 = binary1.ToArray();
            var result = BinaryComparer.Compare(binary1, binary2);
            result.ShouldBeEquivalentTo(BinaryComparisonResult.Equal);
        }

        [TestCaseSource(nameof(DifferentLengths))]
        public void Compare_ArraysOfDifferentLength_ReportedAsOfDifferentSize(byte[] binary1, byte[] binary2)
        {
            var result = BinaryComparer.Compare(binary1, binary2);
            result.ShouldBeEquivalentTo(BinaryComparisonResult.DifferentSize);
        }

        private static object[] DifferentLengths =
        {
            new[] { new byte[0], new byte[] { 1 } },
            new[] { new byte[] { 1, 2, 4 }, new byte[0] },
            new[] { new byte[] { 1, 2, 4 }, new byte[] { 1, 2, 3, 4 } }
        };

        [TestCaseSource(nameof(DifferentArraysOfSameLength))]
        public void Compare_DifferentArraysOfSameLength_DifferencesAreReportedCorrectly
            (byte[] binary1, byte[] binary2, Difference[] differences)
        {
            var result = BinaryComparer.Compare(binary1, binary2);
            result.ShouldBeEquivalentTo(BinaryComparisonResult.Different(differences));
        }

        private static object[] DifferentArraysOfSameLength =
        {
            new object[] 
            { 
                new byte[] { 1 }, 
                new byte[] { 2 }, 
                new[] { new Difference(0, 1) } 
            },
            new object[] 
            { 
                new byte[] { 1, 1, 1, 1 },
                new byte[] { 2, 2, 2, 2 }, 
                new[] { new Difference(0, 4) } 
            },
            new object[] 
            { 
                new byte[] { 1, 2, 3, 4, 5 }, 
                new byte[] { 6, 7, 3, 8, 9 }, 
                new[] { new Difference(0, 2), new Difference(3, 2) } 
            },
            new object[] 
            { 
                new byte[] { 1, 2, 3,   4, 5 }, 
                new byte[] { 1, 2, 255, 4, 5 }, 
                new[] { new Difference(2, 1) } 
            }
        };
    }
}
