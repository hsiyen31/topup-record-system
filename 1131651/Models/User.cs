using System.ComponentModel.DataAnnotations;

namespace _1131651.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "請輸入帳號")]
        public string Account { get; set; }

        [Required(ErrorMessage = "請輸入密碼")]
        public string Password { get; set; }

        [Required(ErrorMessage = "請輸入使用者名稱")]
        public string UserName { get; set; }
    }
}