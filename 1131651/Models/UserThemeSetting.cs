using System.ComponentModel.DataAnnotations;

namespace _1131651.Models
{
    public class UserThemeSetting
    {
        [Key]
        public int SettingId { get; set; }

        public int UserId { get; set; }

        public string ThemeCode { get; set; } = "";
    }
}