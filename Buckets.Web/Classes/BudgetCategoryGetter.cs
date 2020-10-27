using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Buckets.Web.Classes
{
    public interface IBudgetCategoryGetter
    {
        Task<BudgetCategory> Get(string categoryId);
    }

    public class BudgetCategoryGetter : IBudgetCategoryGetter
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private readonly string _apiKey;
        private readonly string _budgetId;

        public BudgetCategoryGetter(string apiKey, string budgetId)
        {
            _apiKey = apiKey;
            _budgetId = budgetId;
        }

        public async Task<BudgetCategory> Get(string categoryId)
        {
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            var content = await HttpClient.GetStringAsync($"https://api.youneedabudget.com/v1/budgets/{_budgetId}/categories/{categoryId}");
            var jObject = JObject.Parse(content);
            var data = jObject["data"];
            var category = data["category"];
            var name = category.Value<string>("name");
            var balanceMilliUnits = category.Value<decimal>("balance");
            var balance = Math.Round(balanceMilliUnits / 1000, 2);

            content = await HttpClient.GetStringAsync($"https://api.youneedabudget.com/v1/budgets/{_budgetId}/categories/{categoryId}/transactions?since_date={DateTime.Now.AddDays(-30):yyyy-MM-dd}");
            jObject = JObject.Parse(content);
            data = jObject["data"];
            var transactionsData = (JArray)data["transactions"];
            var transactions = transactionsData.Select(x =>
            {
                var amountMilliUnits = (decimal) x["amount"];
                var amount = Math.Round(amountMilliUnits / 1000, 2);
                var txResult = new BudgetTransaction
                {
                    Id = (string) x["id"],
                    When = (DateTime) x["date"],
                    Amount = amount,
                    PayeeName = (string) x["payee_name"],
                    Memo = (string) x["memo"]
                };
                return txResult;
            });

            var result = new BudgetCategory
            {
                Id = categoryId,
                Name = name,
                Balance = balance,
                Transactions = transactions
            };
            return result;
        }
    }
}