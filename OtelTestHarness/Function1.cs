using Microsoft.Azure.Functions.Worker;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace OtelTestHarness
{
    public static class Function1
    {
        private static HttpClient _httpClient = new HttpClient();

        [Function(nameof(HttpFunction))]
        public static async Task HttpFunction([TimerTrigger("*/15 * * * * *")] MyInfo myTimer, FunctionContext context)
        {
            using var activity = Program.ActivitySource.StartActivity(nameof(HttpFunction));
            await _httpClient.GetAsync("https://google.com");
        }
    }

    public class MyInfo
    {
        public MyScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
