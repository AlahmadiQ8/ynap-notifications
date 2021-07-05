using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace YnabTransactionsNotifier
{
    public class AvailableTransactionsNotifier
    {
        private readonly int _numberOfTransactionsThreshold;
        private readonly IHostEnvironment _environment;
        private readonly YnabService _ynabService;
        private readonly IConfiguration _configuration;

        public AvailableTransactionsNotifier(YnabService ynabService, IHostEnvironment environment, IConfiguration configuration)
        {
            _ynabService = ynabService;
            _environment = environment;
            _configuration = configuration;
            _numberOfTransactionsThreshold = int.Parse(configuration["NumberOfTransactionsThreshold"]);
        }

        [Function("AvailableTransactionsNotifier")]
        public async Task Run([TimerTrigger("0 0 * * * *")] MyInfo myTimer, FunctionContext context)
        {
            var accountSid = _configuration["TwilioAccountSid"];
            var authToken = _configuration["TwilioAuthToken"];
            TwilioClient.Init(accountSid, authToken);

            var logger = context.GetLogger("AvailableTransactionsNotifier");
            logger.LogInformation($"Current running environment: {_environment.EnvironmentName}");
            logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            logger.LogInformation($"Next timer schedule at: {myTimer?.ScheduleStatus?.Next}");
            logger.LogInformation("Importing transactions");
            var transactions = await _ynabService.ImportTransactions();
            logger.LogInformation($"Imported {transactions.Count} transactions");
            logger.LogTrace($"Transactions ids imported: {string.Join(',', transactions)}");

            if (transactions.Count <= _numberOfTransactionsThreshold)
                return;

            var message = await MessageResource.CreateAsync(
                body: $"Imported {transactions.Count} transactions",
                @from: new PhoneNumber("+18139061616"),
                to: new PhoneNumber("+15419086876")
            );

            logger.LogInformation($"SMS sent with status {message.Status}");
            if (message.ErrorCode != null)
            {
                logger.LogError($"SMS sending message failed: {message.ErrorMessage}");
                throw new ApplicationException(message.ErrorMessage);
            }
        }
    }

    public class MyInfo
    {
        public MyScheduleStatus? ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
