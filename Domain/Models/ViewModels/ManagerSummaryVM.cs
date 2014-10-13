namespace Domain.Models.ViewModels
{
    public class ManagerSummaryVM
    {
        public string TellerName { get; set; }
        public decimal TellerBalance { get; set; }
        public decimal ReceiptCount { get; set; }
        public decimal  TotalSales { get; set; }
        public decimal CashOut{ get; set; }
    }
}
