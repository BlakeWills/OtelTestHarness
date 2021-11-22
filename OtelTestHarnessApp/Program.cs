using Grpc.Net.Client;
using OpenTelemetry;
using OpenTelemetry.Trace;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using OtelTestHarnessAppGrpc;

namespace OtelTestHarnessApp
{
    internal class Program
    {
        public static ActivitySource ActivitySource = new ActivitySource("OtelTestHarness");
        private static HttpClient _httpClient = new HttpClient();

        static async Task Main(string[] args)
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
                        if (obj is HttpRequestMessage request)
                        {
                            Console.WriteLine($"HttpEnrich called for {request.RequestUri}");
                        }
                    };
                })
                .Build();

            // Comment out below line to fix http filter.
            await MakeGrpcRequest();

            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(10));

                using var activity = ActivitySource.StartActivity("test");
                await _httpClient.GetAsync("https://google.com");
            }
        }

        private static async Task MakeGrpcRequest()
        {
            Console.WriteLine("Making gRPC request");
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            _ = await client.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });
        }
    }
}
