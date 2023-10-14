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

using Carcass.Core;
using Qrfy.Sdk.DotNet.Core.Contracts.Requests.Qrs;

namespace Qrfy.Sdk.DotNet.Core.Builders;

public sealed class QrRequestBuilder
{
    private QrRequestBuilder(QrRequest qrRequest)
    {
        ArgumentVerifier.NotNull(qrRequest, nameof(qrRequest));

        QrRequest = qrRequest;
    }

    public QrRequest QrRequest { get; }

    public QrRequestBuilder WithFolder(int? folder)
    {
        QrRequest.Folder = folder;

        return this;
    }

    public QrRequestBuilder WithAccessPassword(string accessPassword)
    {
        ArgumentVerifier.NotNull(accessPassword, nameof(accessPassword), true);
        ArgumentVerifier.Requires(accessPassword.Length >= 3, "AccessPassword length must be greater or equal 3 characters.");

        QrRequest.AccessPassword = accessPassword;

        return this;
    }

    public QrRequestBuilder WithGoogleAnalyticsId(string googleAnalyticsId)
    {
        ArgumentVerifier.NotNull(googleAnalyticsId, nameof(googleAnalyticsId));

        QrRequest.GoogleAnalyticsId = googleAnalyticsId;

        return this;
    }

    public QrRequestBuilder WithFacebookPixelId(string facebookPixelId)
    {
        ArgumentVerifier.NotNull(facebookPixelId, nameof(facebookPixelId));

        QrRequest.FacebookPixelId = facebookPixelId;

        return this;
    }

    public QrRequestBuilder WithGoogleTagManagerId(string googleTagManagerId)
    {
        ArgumentVerifier.NotNull(googleTagManagerId, nameof(googleTagManagerId));

        QrRequest.GoogleTagManagerId = googleTagManagerId;

        return this;
    }

    public QrRequestBuilder WithHostname(string hostname)
    {
        ArgumentVerifier.NotNull(hostname, nameof(hostname));

        QrRequest.Hostname = hostname;

        return this;
    }

    public QrRequestBuilder WithScanLimit(int? scanLimit)
    {
        if (scanLimit.HasValue)
            ArgumentVerifier.Requires(
                scanLimit.Value is >= 1 and <= 10000000,
                "ScanLimit must be between [1..10000000]"
            );

        QrRequest.ScanLimit = scanLimit;

        return this;
    }

    public QrRequestBuilder WithData(Action<QrDataBuilder> dataBuilder)
    {
        dataBuilder.Invoke(QrDataBuilder.New(this));

        return this;
    }

    public static QrRequestBuilder New(string name)
    {
        ArgumentVerifier.NotNull(name, nameof(name));
        ArgumentVerifier.Requires(name.Length <= 100, "Name length must be less or equal 100 characters.");

        return new QrRequestBuilder(new QrRequest
        {
            Name = name
        });
    }
}