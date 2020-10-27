using System;

namespace Balances.Web.Classes
{
    public class BudgetTransaction
    {
        public string Id { get; set; }
        public DateTime When { get; set; }
        public string PayeeName { get; set; }
        public decimal Amount { get; set; }
        public string Memo { get; set; }
    }
}