using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using xknote.Filters;
using xknote.Helpers;
using xknote.Models;

namespace xknote.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [Route("home"), Auth]
        public ActionResult Home()
        {
            xknoteEntities entities = new xknoteEntities();
            Dictionary<string, config> config = entities.config.Select(i => i).ToDictionary(item => item.config_name);
            JObject xknoteData = JObject.FromObject(new
            {
                user_id = Req.GetUser(HttpContext).id,
                nickname = Req.GetUser(HttpContext).nickname,
                xknote_name = config["xknote_name"].config_value,
                document_ext = config["document_ext"].config_value
            });
            ViewData["xknoteData"] = xknoteData.ToString();
            return View();
        }
    }
}
