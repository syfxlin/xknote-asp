using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace xknote.Models
{
    public class RegisterModel
    {
        [DisplayName("用户名")]
        [Required(ErrorMessage = "用户名不能为空")]
        [Remote("CheckUser", "Register", ErrorMessage = "用户名已被注册")]
        public string username { get; set; }
        
        [DisplayName("昵称")]
        [Required(ErrorMessage = "昵称不能为空")]
        public string nickname { get; set; }
        
        [DisplayName("邮箱")]
        [Required(ErrorMessage = "邮箱不能为空")]
        [RegularExpression(@"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "邮箱不正确")]
        public string email { get; set; }
        
        [DisplayName("密码")]
        [Required(ErrorMessage = "密码不能为空")]
        [StringLength(20, ErrorMessage = "确认新密码长度最大为20位,最小为6位", MinimumLength = 6)]
        public string password { get; set; }
        
        [DisplayName("确认密码")]
        [Required(ErrorMessage = "确认密码不能为空")]
        [System.ComponentModel.DataAnnotations.Compare("password", ErrorMessage="两次输入的新密码不一致")]
        public string password_confirmation { get; set; }
    }
}