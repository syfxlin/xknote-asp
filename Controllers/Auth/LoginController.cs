using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using xknote.Filters;
using xknote.Models;

namespace xknote.Controllers.Auth
{
    public class LoginController : Controller
    {
        [Guest]
        [Route("login"), HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [Guest]
        [Route("login"), HttpPost]
        public ActionResult Post(LoginModel data)
        {
            xknoteEntities entities = new xknoteEntities();
            users user = entities.users.Where(users => users.username == data.username).FirstOrDefault();
            if (user == null || !Crypto.VerifyHashedPassword(user.password, data.password))
            {
                return new ErrorResult("用户名或密码错误", 401);
            }

            Session["login_id"] = user.id;
            return Redirect(Url.Action("Home", "Home"));
        }
        
        [Auth]
        [Route("logout")]
        public ActionResult Logout()
        {
            Session.Remove("login_id");
            HttpContext.Items.Remove("user");
            return Redirect(Url.Action("Index", "Login"));
        }
    }
}