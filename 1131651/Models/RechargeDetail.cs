using System.ComponentModel.DataAnnotations;

namespace _1131651.Models
{
    public class RechargeDetail
    {
        [Key]
        public int DetailId { get; set; }

        public int RecordId { get; set; }

        public int PackageId { get; set; }

        public int Quantity { get; set; }

        public int UnitPrice { get; set; }

        public int SubTotal { get; set; }
    }
}