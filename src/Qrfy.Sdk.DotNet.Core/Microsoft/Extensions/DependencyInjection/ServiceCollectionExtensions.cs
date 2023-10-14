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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Qrfy.Sdk.DotNet.Core.DelegatingHandlers;
using Qrfy.Sdk.DotNet.Core.HttpClients.Abstracts;
using Qrfy.Sdk.DotNet.Core.Options;
using Qrfy.Sdk.DotNet.Core.Settings;
using Refit;

// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddQrfy(this IServiceCollection services, IConfiguration configuration)
    {
        const string qrfy = "Qrfy";

        services.Configure<QrfyOptions>(configuration.GetSection(qrfy));

        services.AddHttpClient();

        IHttpClientBuilder httpClientBuilder = services.AddHttpClient(qrfy, (sp, hc) =>
            {
                IOptions<QrfyOptions> optionsAccessor = sp.GetRequiredService<IOptions<QrfyOptions>>();

                hc.BaseAddress = new Uri("https://qrfy.com");
                hc.DefaultRequestHeaders.Add("API-KEY", optionsAccessor.Value.ApiKey);
            }
        );

        ServiceProvider serviceProvider = services.BuildServiceProvider(false);
        IOptions<QrfyOptions> optionsAccessor = serviceProvider.GetRequiredService<IOptions<QrfyOptions>>();

        if (optionsAccessor.Value.IsDebugMode)
        {
            services.AddTransient<QrfyConsoleLoggingDelegatingHandler>();
            httpClientBuilder.AddHttpMessageHandler<QrfyConsoleLoggingDelegatingHandler>();
        }

        services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(qrfy));

        RefitSettings refitSettings = new(new SystemTextJsonContentSerializer(QrfySettings.JsonSerializerOptions))
        {
            ExceptionFactory = _ => Task.FromResult<Exception?>(null)
        };

        serviceProvider = services.BuildServiceProvider(false);
        IHttpClientFactory httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        HttpClient httpClient = httpClientFactory.CreateClient(qrfy);

        services
            .AddSingleton(RestService.For<IFolderHttpClient>(httpClient, refitSettings))
            .AddSingleton(RestService.For<IQrHttpClient>(httpClient, refitSettings));

        return services;
    }
}