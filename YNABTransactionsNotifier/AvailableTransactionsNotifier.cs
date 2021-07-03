using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace YnabTransactionsNotifier
{
    public class AvailableTransactionsNotifier
    {
        private const int Threshold = 5;

        private readonly IHostEnvironment _environment;
        private readonly YnabService _ynabService;

        public AvailableTransactionsNotifier(YnabService ynabService, IHostEnvironment environment)
        {
            _ynabService = ynabService;
            _environment = environment;
        }

        [Function("AvailableTransactionsNotifier")]
        public async Task Run([TimerTrigger("0 0 * * * *	")] MyInfo myTimer, FunctionContext context)
        {
            var logger = context.GetLogger("AvailableTransactionsNotifier");
            logger.LogInformation($"Current running environment: {_environment.EnvironmentName}");
            logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            logger.LogInformation($"Next timer schedule at: {myTimer?.ScheduleStatus.Next}");
            logger.LogInformation("Importing transactions");
            var transactions = await _ynabService.ImportTransactions();
            logger.LogInformation($"Imported {transactions.Count} transactions");
            logger.LogTrace($"Transactions ids imported: {string.Join(',', transactions)}");

            if (transactions.Count >= Threshold)
            {
                logger.LogInformation("Send Message via SMS");
            }
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
