using System;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using xknote.Filters;
using xknote.Models;

namespace xknote.Controllers.Auth
{
    [Guest]
    public class RegisterController : Controller
    {
        [Route("register"), HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [Route("register"), HttpPost]
        public ActionResult Post(RegisterModel data)
        {
            xknoteEntities entities = new xknoteEntities();
            config con = entities.config.Where(item => item.config_name == "enable_register")
                .FirstOrDefault();
            if (con == null || con.config_value == "0")
            {
                return new ErrorResult("系统未开启注册", 403);
            }
            
            if (entities.users.Where(users => users.username == data.username).Count() != 0)
            {
                return new ErrorResult("用户名已经被注册", 409);
            }

            if (data.password != data.password_confirmation)
            {
                return new ErrorResult("两次密码不匹配", 400);
            }

            users user = new users();
            user.username = data.username;
            user.nickname = data.nickname;
            user.email = data.email;
            user.email_verif = DateTime.Now;
            user.password = Crypto.HashPassword(data.password);
            user.remember_to = "";
            user.created_at = DateTime.Now;
            user.updated_at = DateTime.Now;
            entities.users.Add(user);
            entities.SaveChanges();
            user_config userConfig = new user_config();
            userConfig.uid = user.id;
            JObject config = UserModel.GetDefaultConfig();
            userConfig.tinymce_setting = config["tinymceSetting"].ToString();
            userConfig.ace_setting = config["aceSetting"].ToString();
            userConfig.xk_setting = config["xkSetting"].ToString();
            entities.user_config.Add(userConfig);
            entities.SaveChanges();
            return Redirect(Url.Action("Index", "Login"));
        }

        public ActionResult CheckUser()
        {
            xknoteEntities entities = new xknoteEntities();
            string username = Request["username"];
            return Json(entities.users.Where(users => users.username == username).Count() == 0,
                JsonRequestBehavior.AllowGet);
        }
    }
}