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
using System.Text.Json.Serialization;
using Qrfy.Sdk.DotNet.Core.Models.Qrs.Styles.Abstracts;
using Qrfy.Sdk.DotNet.Core.Models.Qrs.Styles.Colors;
using Qrfy.Sdk.DotNet.Core.Models.Qrs.Styles.Colors.Gradients;

namespace Qrfy.Sdk.DotNet.Core.Converters;

public sealed class ColorConverter : JsonConverter<IColor>
{
    public override IColor Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using JsonDocument jsonDocument = JsonDocument.ParseValue(ref reader);
        JsonElement rootElement = jsonDocument.RootElement;

        return rootElement.ValueKind == JsonValueKind.String
            ? new Color { Value = rootElement.GetString()! }
            : JsonSerializer.Deserialize<Gradient>(rootElement.GetRawText(), options)!;
    }

    public override void Write(Utf8JsonWriter writer, IColor value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case Color color:
                writer.WriteStringValue(color.Value);
                break;
            case Gradient gradient:
                JsonSerializer.Serialize(writer, gradient, options);
                break;
        }
    }
}