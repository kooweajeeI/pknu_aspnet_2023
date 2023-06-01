using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace aspnet02_boardapp.Models
{
    // 회원가입할 때 데이터 받는 부분
    public class RegisterModel
    {
        [Required(ErrorMessage = "이메일주소는 필수입니다.")]
        [EmailAddress]
        [DisplayName("이메일주소")]
        public string Email { get; set; }

        [DisplayName("핸드폰번호")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "패스워드는 필수입니다.")]
        [DataType(DataType.Password)]
        [DisplayName("패스워드")]
        public string Password { get; set; }

        [Required(ErrorMessage = "패스워드 확인은 필수입니다.")]
        [DataType(DataType.Password)]
        [DisplayName("패스워드 확인")]
        [Compare("Password", ErrorMessage = "")]
        public string ConfirmPassword { get; set; }
    }
}
