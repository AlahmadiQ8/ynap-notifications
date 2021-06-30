using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace YNABTransactionsNotifier
{
    public class Function1
    {
        private readonly YNABService _ynabService;

        public Function1(YNABService ynabService)
        {
            _ynabService = ynabService;
        }

        [Function("Function1")]
        public async Task Run([TimerTrigger("0 * * * * *")] MyInfo myTimer, FunctionContext context)
        {
            var logger = context.GetLogger("Function1");
            logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            logger.LogInformation($"Next timer schedule at: {myTimer?.ScheduleStatus.Next}");
            var budgets = await _ynabService.GetBudgets();
            logger.LogInformation(budgets);

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
