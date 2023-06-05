using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace aspnet02_boardapp.Models
{
    public class EditRoleModel
    {
        [DisplayName("권한 아이디")]
        public string Id { get; set; }

        [Required(ErrorMessage = "권한이름을 작성해주세요.")]
        [DisplayName("권한 이름")]
        public string RoleName { get; set; }


        public List<string> Users { get; set; }

        public EditRoleModel() 
        {
            Users = new List<string>();
        }

    }
}
