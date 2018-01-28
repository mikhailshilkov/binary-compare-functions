namespace BinaryDiff.Tests
{
    using System;

    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NUnit.Framework;

    [TestFixture]
    public class FunctionsTests
    {
        [TestCase(null)]
        [TestCase("middle")]
        public void Upload_WithInvalidName_ReturnsNotFound(string name)
        {
            var request = new UploadFileRequest { Data = "MQ==" };
            var result = Functions.Upload(request, name, out byte[] binary);
            result.Should().BeOfType<NotFoundResult>();
            binary.Should().BeNull();
        }

        [Test]
        public void Upload_WithoutDataField_ReturnsBadRequest()
        {
            var request = new UploadFileRequest { Data = null };
            var result = Functions.Upload(request, "left", out byte[] binary);
            result.Should().BeOfType<BadRequestObjectResult>();
            binary.Should().BeNull();
        }

        [Test]
        public void Upload_DataFieldIsNotValidBase64_ReturnsBadRequest()
        {
            var request = new UploadFileRequest { Data = "notbase64~" };
            var result = Functions.Upload(request, "right", out byte[] binary);
            result.Should().BeOfType<BadRequestObjectResult>();
            binary.Should().BeNull();
        }

        [Test]
        public void Upload_WithValidBase64_ReturnsOkAndDecodedBinary()
        {
            var value = new byte[] { 1, 5, 11 };
            var request = new UploadFileRequest { Data = Convert.ToBase64String(value) };
            var result = Functions.Upload(request, "right", out byte[] binary);
            result.Should().BeOfType<OkResult>();
            binary.ShouldBeEquivalentTo(value);
        }

        [Test]
        public void Compare_WithValidArguments_ReturnsOkWithComparisonResult()
        {
            var request = new DefaultHttpContext().Request;
            var binary1 = new byte[] { 1, 2, 3 };
            var binary2 = new byte[] { 4, 5, 6 };
            var expected = BinaryComparisonResult.Different(
                new[] { new Difference(0, 1), new Difference(5, 2) });
            Functions.Comparer = (b1, b2) => 
                b1 == binary1 && b2 == binary2 
                ? expected 
                : throw new ArgumentException("Unexpected byte array passed to comparer");

            var actual = Functions.Compare(request, binary1, binary2);

            actual.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(expected);
        }

        [TestCaseSource(nameof(NullBinaries))]
        public void Compare_WithBinaryMissing_NotFoundResultIsReturned(byte[] binary1, byte[] binary2)
        {
            var request = new DefaultHttpContext().Request;
            var result = Functions.Compare(request, binary1, binary2);
            result.Should().BeOfType<NotFoundResult>();
        }

        private static object[] NullBinaries =
        {
            new[] { null, new byte[] { 1 } },
            new[] { new byte[] { 1, 2, 4 }, null },
            new byte[][] { null, null }
        };        
    }
}
