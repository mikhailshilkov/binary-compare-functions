namespace BinaryDiff.Tests
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    
    using FluentAssertions;
    using Newtonsoft.Json;
    using NUnit.Framework;

    [TestFixture]
    public class IntegrationTests
    {
        [Test]
        public async Task UploadLeftRightAndThenCompare_WithKnownDifference_ExpectedDifferenceIsReported()
        {
            // Prepare data
            var binaryLeft  = new byte[] {  1, 2, 3,  4,  5, 6, 7, 8, 9, 0 };
            var binaryRight = new byte[] { 11, 2, 3, 14, 15, 6, 7, 8, 9, 0 };

            // Upload 
            var requestLeft  = new UploadFileRequest { Data = Convert.ToBase64String(binaryLeft) };
            var requestRight = new UploadFileRequest { Data = Convert.ToBase64String(binaryRight) };

            await PutAsync($"{baseUrl}/diff/{id}/left", requestLeft);
            await PutAsync($"{baseUrl}/diff/{id}/right", requestRight);

            // Query for difference
            var diff = await GetAsync($"{baseUrl}/diff/{id}");

            // Assert
            diff.result.Should().Be("different");
            diff.differences.ShouldBeEquivalentTo(new[]
            {
                new { startIndex = 0, length = 1 },
                new { startIndex = 3, length = 2 }
            });
        }

        private class ExpectedResult
        {
            public string result { get; set; }

            public ExpectedResultDiff[] differences { get; set; }

            public class ExpectedResultDiff 
            {
                public int startIndex { get; set; }
                public int length { get; set; }
            }
        }

        private static async Task PutAsync(string url, object body)
        {
            string jsonString = JsonConvert.SerializeObject(body);
            var httpContent = new StringContent(jsonString);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var uploadLeftResponse = await client.PutAsync(url, httpContent);
            uploadLeftResponse.EnsureSuccessStatusCode();
        }

        private static async Task<ExpectedResult> GetAsync(string url)
        {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var contentString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ExpectedResult>(contentString);
        }

        private const string id = "INTEGRATION_TEST";
        private const string baseUrl = "http://localhost:7071/v1";
        private static readonly HttpClient client = new HttpClient();
    }
}
