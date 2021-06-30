using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;


namespace YNABTransactionsNotifier
{
    public class YNABService
    {
        private const string AccessToken = "36463d79993fa5a235f04850c3ce862bdf0adb4743894f8089792d554e85fbf1";
        private readonly HttpClient _httpClient;
        private readonly ILogger<YNABService> _logger;

        public YNABService(HttpClient httpClient, ILogger<YNABService> logger)
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
            }

            return content;
        }
    }
}
