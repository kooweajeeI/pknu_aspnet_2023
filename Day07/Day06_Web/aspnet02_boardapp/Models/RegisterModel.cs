using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace aspnet02_boardapp.Models
{
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        [DisplayName("이메일 주소")]
        public string Email { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [DisplayName("패스워드")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("패스워드 확인")]
        public string ConfirmPassword { get; set; }

    }
}
