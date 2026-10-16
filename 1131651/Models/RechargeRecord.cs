using System.ComponentModel.DataAnnotations;

namespace _1131651.Models
{
    public class RechargeRecord
    {
        [Key]
        public int RecordId { get; set; }

        public int UserId { get; set; }

        public int PoolTypeId { get; set; }

        [Required(ErrorMessage = "請輸入卡池名稱")]
        public string PoolName { get; set; } = "";

        public int TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}