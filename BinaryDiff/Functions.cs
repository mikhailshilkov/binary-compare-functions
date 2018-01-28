namespace BinaryDiff
{
    using System;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;

    public static class Functions
    {
        [FunctionName("Upload")]
        public static IActionResult Upload(
            [HttpTrigger(AuthorizationLevel.Anonymous, "PUT", Route = "v1/diff/{id}/{name}")] UploadFileRequest request,
            string name,
            [Blob("{name}/{id}")] out byte[] binary)
        {
            if (name != "left" && name != "right")
            {
                binary = null;
                return new NotFoundResult();
            }

            if (request?.Data == null)
            {
                binary = null;
                return new BadRequestObjectResult("Expecting JSON body with 'data' field");
            }

            try
            {
                binary = Convert.FromBase64String(request?.Data);
            }
            catch (FormatException)
            {
                binary = null;
                return new BadRequestObjectResult("Data is a not valid Base64 string");
            }

            return new OkResult();
        }

        [FunctionName("Compare")]
        public static IActionResult Compare(
            [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "v1/diff/{id}")] HttpRequest request,
            [Blob("left/{id}")] byte[] leftBinary,
            [Blob("right/{id}")] byte[] rightBinary)
        {
            if (leftBinary == null || rightBinary == null)
            {
                return new NotFoundResult();
            }

            var result = Comparer(leftBinary, rightBinary);
            return new OkObjectResult(result);
        }

        // Azure Functions don't support traditional Dependency Injection yet,
        // so I made Comparer set-able for unit test via this static property,
        // while the actual Function App won't ever change it
        public static Func<byte[], byte[], BinaryComparisonResult> Comparer { get; set; } = BinaryComparer.Compare;
    }
}
