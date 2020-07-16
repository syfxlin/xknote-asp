using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace xknote.Models
{
    public class LoginModel
    {
        [DisplayName("用户名")]
        [Required(ErrorMessage = "用户名不能为空")]
        public string username { get; set; }

        [DisplayName("密码")]
        [Required(ErrorMessage = "密码不能为空")]
        [StringLength(20, ErrorMessage = "确认新密码长度最大为20位,最小为6位", MinimumLength = 6)]
        public string password { get; set; }
        
        [DisplayName("记住我")]
        public string remember { get; set; }
    }
}