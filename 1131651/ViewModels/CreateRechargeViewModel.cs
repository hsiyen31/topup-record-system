using Microsoft.AspNetCore.Mvc.Rendering;

namespace _1131651.ViewModels
{
    public class CreateRechargeViewModel
    {
        public int RecordId { get; set; }
        public string PoolName { get; set; } = "";

        public int PoolTypeId { get; set; }

        public List<SelectListItem> PoolTypes { get; set; } = new List<SelectListItem>();

        public List<PackageItemViewModel> PackageItems { get; set; } = new List<PackageItemViewModel>();
    }

    public class PackageItemViewModel
    {
        public int PackageId { get; set; }

        public string PackageName { get; set; } = "";

        public int Price { get; set; }

        public int LimitQuantity { get; set; }

        public int Quantity { get; set; }
    }
}