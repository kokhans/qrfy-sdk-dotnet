# QRfy .NET SDK [![GitHub](https://img.shields.io/github/license/kokhans/qrfy-sdk-dotnet?style=flat-square)](LICENSE)

QRfy .NET SDK provides easily generate, manage and statistically track QR codes using [QRfy](https://qrfy.com) service.

# Getting Started

## Prerequisites

- [.NET 7 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)

## Installation

To install `Qrfy.Sdk.DotNet.Core` and its dependencies via .NET Core CLI, execute the following command.

```powershell
dotnet add package Qrfy.Sdk.DotNet.Core
```

To install `Qrfy.Sdk.DotNet.Core` and its dependencies via NuGet, execute the following command.

```powershell
Install-Package Qrfy.Sdk.DotNet.Core
```

## Congfiuration

To configure the QRfy .NET SDK you'll need to add a section named `Qrfy` to your `appsettings.json` file. This section should contain the necessary configuration values for the SDK.

```json
{
  "Qrfy": {
    "ApiKey": "YOUR_API_KEY_HERE",
    "IsDebugMode": true
  }
}
```

- `ApiKey` - API key required for authentication with the QRfy service. Replace "YOUR_API_KEY_HERE" with your actual API key.
- `IsDebugMode` - Flag that indicates whether the SDK should operate in debug mode. You can set it to true or false as needed. When `IsDebugMode` is enabled, the `QrfyConsoleLoggingDelegatingHandler` will log HTTP request and response messages to the console.

## Registration

To register `Qrfy.Sdk.DotNet.Core` and its dependencies, use the following code.

```csharp
services.AddQrfy(configuration);
```

This code extends `IServiceCollection` and configures an `HttpClient` for API requests, reads configuration settings, handles debug mode, and uses `Refit` to create typed HTTP client interfaces.

## IFolderHttpClient

`IFolderHttpClient` interface defines methods for interacting with folder-related operations through HTTP requests using `Refit`.

|Name|Description|Request|Response|
|-|-|-|-|
|`CreateFolderAsync`|Creates a folder|`CreateFolderBody`|`ApiResponse<CreateFolderResponse?>`|
|`ListFoldersAsync`|Retrieves a list of folders||`ApiResponse<ListFoldersResponse?>`|

### CreateFolderAsync

```csharp
// Resolve the IFolderHttpClient from the container
var folderHttpClient = serviceProvider.GetRequiredService<IFolderHttpClient>();

var createFolderRequest = new CreateFolderBody
{
    Name = "New Folder",
    Description = "A folder for storing QR codes"
};
ApiResponse<CreateFolderResponse?> createFolderResponse = await folderHttpClient.CreateFolderAsync(createFolderRequest);
if (createFolderResponse.IsSuccessStatusCode)
{
    var createdFolder = createFolderResponse.Content;
    Console.WriteLine($"Folder created: {createdFolder?.Name}");
}
else
    Console.WriteLine($"Failed to create folder. Status code: {createFolderResponse.StatusCode}");
```

### ListFoldersAsync

```csharp
// Resolve the IFolderHttpClient from the container
var folderHttpClient = serviceProvider.GetRequiredService<IFolderHttpClient>();

ApiResponse<ListFoldersResponse?> listFoldersResponse = await folderHttpClient.ListFoldersAsync();
if (listFoldersResponse.IsSuccessStatusCode)
{
    var folders = listFoldersResponse.Content;
    Console.WriteLine("List of folders:");
    foreach (var folder in folders)    
        Console.WriteLine($"Folder ID: {folder.Id}, Name: {folder.Name}");    
}
else
    Console.WriteLine($"Failed to list folders. Status code: {listFoldersResponse.StatusCode}");
```

## IQrHttpClient

`IQrHttpClient` interface defines methods for interacting with QR code-related operations through HTTP requests using `Refit`.

|Name|Description|Request|Response|
|-|-|-|-|
|`CreateBulkQrsAsync`|Creates multiple QR codes|`CreateBulkQrsBody`|`ApiResponse<CreateBulkQrsResponse>`|
|`ListQrsAsync`|Retrieves a list of QR codes|`int? page`, `SortBy? sortBy`, `int? folder`, `QrType[]? types`, `QrParsedStatus? status`, `string? searchTerm`|`ApiResponse<ListQrsResponse?>`|
|`GetQrAsync`|Retrieves information about a QR code|`int id`|`ApiResponse<QrResponse?>`|
|`RetrieveQrImageAsync`|Retrieves the QR code image|`RetrieveQrImageQuery`|`ApiResponse<Stream?>`|
|`BatchDeleteQrsAsync`|Deletes multiple QR codes in bulk|`BatchDeleteQrsBody`|`IApiResponse`|
|`GetAnalysisReportAsync`|Retrieves an analysis report for QR codes|`int from`, `int to`, `int[] ids`, `AnalysisReportFormat format`|`ApiResponse<JsonDocument?>`|

### CreateBulkQrsAsync

```csharp
// Resolve the IQrHttpClient from the container
var qrHttpClient = serviceProvider.GetRequiredService<IQrHttpClient>();

var createBulkQrsRequest = new CreateBulkQrsBody
{
    // Set the properties
};
ApiResponse<CreateBulkQrsResponse> createBulkQrsResponse = await qrHttpClient.CreateBulkQrsAsync(createBulkQrsRequest);
if (createBulkQrsResponse.IsSuccessStatusCode)
{
    var createdQrs = createBulkQrsResponse.Content;
    Console.WriteLine($"Batch of QR codes created. Count: {createdQrs?.Count}");
}
else
    Console.WriteLine($"Failed to create QR codes. Status code: {createBulkQrsResponse.StatusCode}");
```

### ListQrsAsync

```csharp
// Resolve the IQrHttpClient from the container
var qrHttpClient = serviceProvider.GetRequiredService<IQrHttpClient>();

ApiResponse<ListQrsResponse?> listQrsResponse = await qrHttpClient.ListQrsAsync(
    page: 1,
    sortBy: SortBy.Name,
    folder: 123,
    types: new[] { QrType.Url },
    status: QrParsedStatus.Parsed,
    searchTerm: "example"
);
if (listQrsResponse.IsSuccessStatusCode)
{
    var qrCodes = listQrsResponse.Content;
    Console.WriteLine("List of QR codes:");
    foreach (var qrCode in qrCodes)
    {
        Console.WriteLine($"QR Code ID: {qrCode.Id}, Type: {qrCode.Type}, Status: {qrCode.Status}");
    }
}
else
    Console.WriteLine($"Failed to list QR codes. Status code: {listQrsResponse.StatusCode}");
```

### GetQrAsync

```csharp
// Resolve the IQrHttpClient from the container
var qrHttpClient = serviceProvider.GetRequiredService<IQrHttpClient>();

int qrCodeId = 123;
ApiResponse<QrResponse?> getQrResponse = await qrHttpClient.GetQrAsync(qrCodeId);
if (getQrResponse.IsSuccessStatusCode)
{
    var qrCode = getQrResponse.Content;
    Console.WriteLine($"QR Code ID: {qrCode?.Id}, Type: {qrCode?.Type}, Status: {qrCode?.Status}");
}
else
    Console.WriteLine($"Failed to get QR code. Status code: {getQrResponse.StatusCode}");
```

### RetrieveQrImageAsync

```csharp
// Resolve the IQrHttpClient from the container
var qrHttpClient = serviceProvider.GetRequiredService<IQrHttpClient>();

var retrieveQrImageQuery = new RetrieveQrImageQuery
{
    Id = 123,
    Format = "png"
};
ApiResponse<Stream?> retrieveQrImageResponse = await qrHttpClient.RetrieveQrImageAsync(retrieveQrImageQuery);
if (retrieveQrImageResponse.IsSuccessStatusCode)
{
    var qrImageStream = retrieveQrImageResponse.Content;
    // Process or display the QR code image stream as needed
}
else
    Console.WriteLine($"Failed to retrieve QR code image. Status code: {retrieveQrImageResponse.StatusCode}");
```

### BatchDeleteQrsAsync

```csharp
// Resolve the IQrHttpClient from the container
var qrHttpClient = serviceProvider.GetRequiredService<IQrHttpClient>();

var batchDeleteQrsRequest = new BatchDeleteQrsBody
{
    // Set the properties
};
ApiResponse<IApiResponse> batchDeleteQrsResponse = await qrHttpClient.BatchDeleteQrsAsync(batchDeleteQrsRequest);
if (batchDeleteQrsResponse.IsSuccessStatusCode)
    Console.WriteLine("QR codes deleted successfully.");
else
    Console.WriteLine($"Failed to delete QR codes. Status code: {batchDeleteQrsResponse.StatusCode}");
```

### GetAnalysisReportAsync

```csharp
// Resolve the IQrHttpClient from the container
var qrHttpClient = serviceProvider.GetRequiredService<IQrHttpClient>();

int from = /* specify from date */;
int to = /* specify to date */;
int[] qrCodeIds = /* specify QR code IDs */;
AnalysisReportFormat reportFormat = AnalysisReportFormat.Json;
ApiResponse<JsonDocument?> analysisReportResponse = await qrHttpClient.GetAnalysisReportAsync(from, to, qrCodeIds, reportFormat);
if (analysisReportResponse.IsSuccessStatusCode)
{
    var analysisReport = analysisReportResponse.Content;
    // Process the analysis report data as needed
}
else
    Console.WriteLine($"Failed to get analysis report. Status code: {analysisReportResponse.StatusCode}");
```

# License

This project is licensed under the [MIT license](LICENSE).
