using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using OpenTelemetry.Trace;
using System;
using System.Diagnostics;
using System.Net.Http;

namespace OtelTestHarness
{
    public class Program
    {
        public static ActivitySource ActivitySource = new ActivitySource("OtelTestHarness");
        public static void Main()
        {
            using var tracerProvider = Sdk.CreateTracerProviderBuilder()
                .SetSampler(new AlwaysOnSampler())
                .AddSource(ActivitySource.Name)
                .AddConsoleExporter()
                .AddHttpClientInstrumentation(x =>
                {
                    x.Filter = (request) =>
                    {
                        Console.WriteLine($"HttpFilter called for {request.RequestUri}");
                        return true;
                    };
                    x.Enrich = (activity, name, obj) =>
                    {
                        if(obj is HttpRequestMessage request)
                        {
                            Console.WriteLine($"HttpEnrich called for {request.RequestUri}");
                        }
                    };
                })
                .Build();

            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .Build();

            host.Run();
        }
    }
}