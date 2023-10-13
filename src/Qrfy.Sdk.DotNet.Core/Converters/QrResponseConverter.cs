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
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Qrfy.Sdk.DotNet.Core.Contracts.Responses.Qrs;
using Qrfy.Sdk.DotNet.Core.Extensions;
using Qrfy.Sdk.DotNet.Core.Models.Qrs;
using Qrfy.Sdk.DotNet.Core.Models.Qrs.Data.Body;
using Qrfy.Sdk.DotNet.Core.Models.Qrs.Data.Body.Abstracts;
using Qrfy.Sdk.DotNet.Core.Models.Qrs.Folders;
using Qrfy.Sdk.DotNet.Core.Models.Qrs.Styles;

namespace Qrfy.Sdk.DotNet.Core.Converters;

public sealed class QrResponseConverter : JsonConverter<QrResponse>
{
    public override QrResponse Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using JsonDocument jsonDocument = JsonDocument.ParseValue(ref reader);
        JsonElement rootElement = jsonDocument.RootElement;

        QrType type = Enum.Parse<QrType>(rootElement.GetProperty("type").GetString()!, true);
        string data = rootElement.GetProperty("data").GetRawText();
        IDataBody dataBody = type switch
        {
            QrType.Url => JsonSerializer.Deserialize<UrlBody>(data, options)!,
            QrType.UrlStatic => JsonSerializer.Deserialize<UrlStaticBody>(rootElement.GetProperty("data").GetRawText(), options)!,
            _ => new NotSupportedBody()
        };

        int id = rootElement.GetProperty("id").GetInt32();
        bool accessPassword = rootElement.GetProperty("accessPassword").GetBoolean();
        QrFolder? folder = JsonSerializer.Deserialize<QrFolder>(rootElement.GetProperty("folder").GetRawText(), options);
        bool stopped = rootElement.GetProperty("stopped").GetBoolean();
        Style style = JsonSerializer.Deserialize<Style>(rootElement.GetProperty("style").GetRawText(), options)!;
        int createdAt = rootElement.GetProperty("createdAt").GetInt32();
        int updatedAt = rootElement.GetProperty("updatedAt").GetInt32();
        bool status = rootElement.GetProperty("status").GetBoolean();
        bool blocked = rootElement.GetProperty("blocked").GetBoolean();
        int visits = rootElement.GetProperty("visits").GetInt32();
        int scans = rootElement.GetProperty("scans").GetInt32();
        bool favorite = rootElement.GetProperty("favorite").GetBoolean();
        QrParsedStatus parsedStatus = Enum.Parse<QrParsedStatus>(rootElement.GetProperty("parsedStatus").GetString()!, true);

        JsonObject jsonObject = JsonNode.Parse(rootElement.ToString())!.AsObject();
        jsonObject.Remove("data");
        Qr qr = JsonSerializer.Deserialize<Qr>(jsonObject.ToJsonString(), options)!;

        QrResponse qrResponse = qr.Map<Qr, QrResponse>() with
        {
            Id = id,
            Folder = folder,
            AccessPassword = accessPassword,
            Stopped = stopped,
            Data = dataBody,
            Style = style,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            Status = status,
            Blocked = blocked,
            Visits = visits,
            Scans = scans,
            Favorite = favorite,
            ParsedStatus = parsedStatus
        };

        return qrResponse;
    }

    public override void Write(Utf8JsonWriter writer, QrResponse value, JsonSerializerOptions options) => throw new NotSupportedException();
}