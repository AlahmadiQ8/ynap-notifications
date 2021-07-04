using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;


namespace YnabTransactionsNotifier
{
    public class YnabService
    {
        private const string AccessToken = "36463d79993fa5a235f04850c3ce862bdf0adb4743894f8089792d554e85fbf1";
        private const string BudgetId = "0e14082f-3e6c-48a8-b644-e31f6cda8785";
        private readonly HttpClient _httpClient;
        private readonly ILogger<YnabService> _logger;

        public YnabService(HttpClient httpClient, ILogger<YnabService> logger)
        {
            httpClient.BaseAddress = new Uri("https://api.youneedabudget.com");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string> GetBudgets()
        {
            var endpoint = "/v1/budgets";
            var result = await _httpClient.GetAsync(endpoint);
            var content = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
            {
                _logger.LogError($"{result.RequestMessage?.RequestUri?.OriginalString} failed with status code {result.StatusCode}");
                _logger.LogError($"Error body: {content}");
                throw new HttpRequestException(null, null, result.StatusCode);
            }

            return content;
        }

        public async Task<IList<string>> ImportTransactions()
        {
            var endpoint = $"/v1/budgets/{BudgetId}/transactions/import";
            var result = await _httpClient.PostAsync(endpoint, new StringContent(string.Empty));

            switch (result.StatusCode)
            {
                case HttpStatusCode.OK:
                    return new List<string>();
                case HttpStatusCode.Created:
                    var transactions = await result.Content.ReadFromJsonAsync<DataResponse<TransactionIdsResponse>>();
                    return transactions?.Data.TransactionIds ?? new List<string>();
                default:
                    _logger.LogError($"{result.RequestMessage?.RequestUri?.OriginalString} failed with status code {result.StatusCode}");
                    throw new HttpRequestException(null, null, result.StatusCode);
            }
        }
    }
}
