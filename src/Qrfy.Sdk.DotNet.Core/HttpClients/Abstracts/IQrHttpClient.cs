// MIT License
//
// Copyright (c) 2023 Serhii Kokhan
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Text.Json;
using Qrfy.Sdk.DotNet.Core.Contracts.Requests.Qrs;
using Qrfy.Sdk.DotNet.Core.Contracts.Requests.Qrs.Common;
using Qrfy.Sdk.DotNet.Core.Contracts.Responses.Qrs;
using Qrfy.Sdk.DotNet.Core.Models.Qrs;
using Refit;

namespace Qrfy.Sdk.DotNet.Core.HttpClients.Abstracts;

public interface IQrHttpClient
{
    [Post("/api/public/qrs")]
    Task<ApiResponse<CreateBulkQrsResponse>> CreateBulkQrsAsync([Body] CreateBulkQrsBody body);

    [Get("/api/public/qrs")]
    Task<ApiResponse<ListQrsResponse?>> ListQrsAsync(
        int? page,
        SortBy? sortBy,
        int? folder,
        QrType[]? types,
        QrParsedStatus? status,
        string? searchTerm
    );

    [Get("/api/public/qrs/{id}")]
    Task<ApiResponse<QrResponse?>> GetQrAsync(int id);

    [Get("/api/public/qrs/{query.Id}/{query.Format}")]
    Task<ApiResponse<Stream?>> RetrieveQrImageAsync(RetrieveQrImageQuery query);

    [Post("/api/public/qrs/batch-delete")]
    Task<IApiResponse> BatchDeleteQrsAsync([Body] BatchDeleteQrsBody body);

    [Get("/api/public/qrs/report")]
    Task<ApiResponse<JsonDocument?>> GetAnalysisReportAsync(
        int from,
        int to,
        int[] ids,
        AnalysisReportFormat format
    );
}