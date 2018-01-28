# Sample C# Azure Function App to upload two binary files and compare them byte-by-byte

## Implementation Tech Stack

- C# 7
- .NET Standard 2.0, .NET Core 2.0
- Azure Blob Storage
- Azure Functions v2.0 Beta

## Usage Workflow

1. Encode the two binaries that you need to compare as Base64 string.
2. Put these Base64 strings to JSON into `data` field.
3. Pick an `ID` for the comparison.
3. Send the binaries with `PUT` request to `/v1/diff/:id/left` and `/v1/diff/:id/right` URLs.
4. Receive the comparison result with `GET` request to `/v1/diff/:id` URL.

A test version of this application is deployed to `https://binarycompare.azurewebsites.net`
base URL.

## API

### **Upload Binary**

  Upload binary as `left` or `right` object for comparison.

* **URL**

  `/v1/diff/:id/left` and `/v1/diff/:id/right`

* **Method:**

  `PUT`
  
*  **URL Params**

   **Required:**
 
   `id=[integer]` - difference ID that you pick, to be used when requesting the difference

* **Content**

  `{ "data": "base64 encoded binary" }` 

* **Success Response:**

  * **Code:** 200 <br />
    Binary was uploaded successfully
 
* **Error Response:**

  * **Code:** 404 NOT FOUND <br />
    URL is not correct

  OR

  * **Code:** 400 BAD REQUEST <br />
    **Content:** `"Reason explaining why the request was not valid"`

* **Sample HTTP Request:**

  ```
  PUT /v1/diff/sampleid/left HTTP/1.1
  Content-Type:application/json
  Accept:application/json

  {"data":"AQIDBAUGBwgJAA=="}
  ```

### **Get Comparison Result**

  Get the comparison result of 2 previously uploaded binaries (left and right).

* **URL**

  `/v1/diff/:id`

* **Method:**

  `GET`
  
*  **URL Params**

   **Required:**
 
   `id=[integer]` - difference ID, which was used to upload the two binaries to compare

* **Success Response:**

  * **Code:** 200 <br />
    Comparison is successful <br />
    **Content when binaries are exactly the same:** <br /> `{"result":"same"}` <br />
    **Content when binaries have different length:** <br /> `{"result":"size-differs"}` <br />
    **Content when binaries have the same length but some differences exist:** 
    ```
    { 
      "result":"different", 
      "differences": [
         { "startIndex": 5, "length": 2 },
         { "startIndex": 11, "length": 1 }
      ]
    }`
    ```
    Both `startIndex` and `length` refer to each byte separately.
 
* **Error Response:**

  * **Code:** 404 NOT FOUND <br />
    One or both binaries with requested ID were not uploaded yet

* **Sample HTTP Request:**

  ```
  GET /v1/diff/sampleid HTTP/1.1
  Content-Type:application/json
  Accept:application/json
  ```
                 
## Running Locally

- Make sure that you have a proper Azure Storage connection string in `local.settings.json`
- `dotnet restore` to restore NuGet packages
- `dotnet build` to build the solution
- `dotnet publish` to publish the binaries
- `cd BinaryDiff` then `func start --script-root bin\\debug\\netstandard2.0\\publish` to start Function App locally
- `dotnet test 'BinaryDiff.Tests\\BinaryDiff.Tests.csproj'` to run tests (Integration Test required Function
App to be running)