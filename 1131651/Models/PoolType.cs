using System.ComponentModel.DataAnnotations;

namespace _1131651.Models
{
    public class PoolType
    {
        [Key]
        public int PoolTypeId { get; set; }

        public string PoolTypeName { get; set; } = "";
    }
}