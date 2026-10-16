using System.ComponentModel.DataAnnotations;

namespace _1131651.Models
{
    public class Package
    {
        [Key]
        public int PackageId { get; set; }

        public int PoolTypeId { get; set; }

        public string PackageName { get; set; } = "";

        public int Price { get; set; }

        public int LimitQuantity { get; set; }
    }
}