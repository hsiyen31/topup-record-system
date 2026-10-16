namespace _1131651.ViewModels
{
    public class RechargeRecordViewModel
    {
        public int RecordId { get; set; }

        public string PoolName { get; set; } = "";

        public int PoolTypeId { get; set; }
        public string PoolTypeName { get; set; } = "";

        public int TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<RechargeDetailViewModel> Details { get; set; } = new List<RechargeDetailViewModel>();
    }

    public class RechargeDetailViewModel
    {
        public string PackageName { get; set; } = "";

        public int Quantity { get; set; }

        public int UnitPrice { get; set; }

        public int SubTotal { get; set; }
    }
}