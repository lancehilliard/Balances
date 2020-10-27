using System.Collections.Generic;

namespace Balances.Web.Classes
{
    public class BudgetCategory {
        public string Name { get; set; }
        public string Id { get; set; }
        public decimal Balance { get; set; }
        public IEnumerable<BudgetTransaction> Transactions { get; set; }
    }
}