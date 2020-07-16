using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using xknote.Filters;
using xknote.Helpers;
using xknote.Models;

namespace xknote.Controllers
{
    [Auth]
    public class UserController : Controller
    {
        [Route("api/user"), HttpGet]
        public ActionResult Get()
        {
            return new JObjectResult("user", JObject.FromObject(Req.GetUser(HttpContext)));
        }

        [Route("api/user/conf"), HttpGet]
        public ActionResult GetConfig()
        {
            return new JObjectResult("config", UserModel.GetConfig(Req.GetUser(HttpContext).id));
        }

        [Route("api/user/conf"), HttpPut]
        public ActionResult SetConfig()
        {
            JObject body = Req.ParseBody(Request);
            UserModel.SetConfig(Req.GetUser(HttpContext).id, body);
            return new JObjectResult();
        }
    }
}