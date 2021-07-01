using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace YnabTransactionsNotifier
{
    public class DataResponse<T>
    {
        [JsonPropertyName("data")] public T Data { get; set; }
    }

    public class TransactionIdsResponse
    {
        [JsonPropertyName("transaction_ids")] public IList<string> TransactionIds { get; set; }
    }
}
