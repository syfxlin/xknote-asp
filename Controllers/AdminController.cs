using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using xknote.Filters;
using xknote.Helpers;
using xknote.Models;

namespace xknote.Controllers
{
    [Admin]
    public class AdminController : Controller
    {
        [Route("api/admin/users"), HttpGet]
        public ActionResult GetUsers()
        {
            return new JObjectResult("users", JArray.FromObject(UserModel.GetAllUsers()));
        }

        [Route("api/admin/users"), HttpDelete]
        public ActionResult DeleteUsers()
        {
            if (Request["id"] == null)
            {
                return new ErrorResult("Parameter not found. (id)", 400);
            }
            int code = UserModel.DeleteUser(long.Parse(Request["id"]));
            if (code == 404)
            {
                return new ErrorResult("User not found.", 404);
            }
            return new JObjectResult();
        }

        [Route("api/admin/conf"), HttpGet]
        public ActionResult GetConfig()
        {
            xknoteEntities entities = new xknoteEntities();
            IQueryable<config> configs = entities.config.Select(i => i);
            JObject jObject = new JObject();
            foreach (config item in configs)
            {
                jObject[item.config_name] = item.config_value;
            }
            jObject["enable_register"] = jObject["enable_register"].ToString() != "0";
            return new JObjectResult("config", jObject);
        }

        [Route("api/admin/conf"), HttpPut]
        public ActionResult SetConfig()
        {
            JObject body = Req.ParseBody(Request);
            xknoteEntities entities = new xknoteEntities();
            config config = null;
            config = entities.config.Where(item => item.config_name == "enable_register").FirstOrDefault();
            if (config != null)
            {
                config.config_value = body["enable_register"].ToString();
                entities.SaveChanges();
            }
            config = entities.config.Where(item => item.config_name == "xknote_name").FirstOrDefault();
            if (config != null)
            {
                config.config_value = body["xknote_name"].ToString();
                entities.SaveChanges();
            }
            config = entities.config.Where(item => item.config_name == "upload_limit").FirstOrDefault();
            if (config != null)
            {
                config.config_value = body["upload_limit"].ToString();
                entities.SaveChanges();
            }
            return new JObjectResult();
        }
    }
}