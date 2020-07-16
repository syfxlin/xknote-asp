using System.Text.RegularExpressions;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using xknote.Filters;
using xknote.Helpers;
using xknote.Models;

namespace xknote.Controllers
{
    [Auth]
    public class FolderController : Controller
    {
        [Route("api/folders"), HttpGet]
        public ActionResult Get()
        {
            string[] path = this.GetPath("");
            return new JObjectResult("folders", new FolderModel(path[0]).Get(path[1]));
        }

        [Route("api/folders/only"), HttpGet]
        public ActionResult GetOnly()
        {
            string[] path = this.GetPath("");
            return new JObjectResult("folders", new FolderModel(path[0]).Get(path[1], "only"));
        }

        [Route("api/folders/flat"), HttpGet]
        public ActionResult GetFlat()
        {
            string[] path = this.GetPath("");
            return new JObjectResult("folders", JArray.FromObject(new FolderModel(path[0]).GetFlat(path[1])));
        }

        [Route("api/folders"), HttpPost]
        public ActionResult Create()
        {
            JObject body = Req.ParseBody(Request);
            if (body["path"] == null)
            {
                return new ErrorResult("Parameter not found. (path)", 400);
            }
            string[] path = this.GetPath(body["path"].ToString());
            int code = new FolderModel(path[0]).Create(path[1]);
            if (code == 409)
            {
                return new ErrorResult("The folder already exists.", 409);
            }
            return new JObjectResult();
        }

        [Route("api/folders"), HttpDelete]
        public ActionResult Delete()
        {
            if (Request["path"] == null)
            {
                return new ErrorResult("Parameter not found. (path)", 400);
            }
            string[] path = this.GetPath(Request["path"]);
            int code = new FolderModel(path[0]).Delete(path[1]);
            return new JObjectResult();
        }

        [Route("api/folders/move"), HttpPut]
        public ActionResult Move()
        {
            JObject body = Req.ParseBody(Request);
            if (body["new_path"] == null || body["old_path"] == null)
            {
                return new ErrorResult("Parameter not found. (new_path, old_path)", 400);
            }
            string[] newPath = this.GetPath(body["new_path"].ToString());
            string[] oldPath = this.GetPath(body["old_path"].ToString());
            if (new Regex(@"\.\.\/").IsMatch(newPath[1] + oldPath[1]))
            {
                return new ErrorResult("You submitted a restricted character. (../)", 400);
            }
            int code = new FolderModel(newPath[0]).Move(newPath[1], oldPath[1]);
            if (code == 404)
            {
                return new ErrorResult("Folder not found.", 404);
            }
            return new JObjectResult();
        }

        [Route("api/folders/rename"), Route("api/folders"), HttpPut]
        public ActionResult Rename()
        {
            return this.Move();
        }

        [Route("api/folders/exist"), HttpGet]
        public ActionResult Exist()
        {
            if (Request["path"] == null)
            {
                return new ErrorResult("Parameter not found. (path)", 400);
            }
            string[] path = this.GetPath(Request["path"]);
            return new JObjectResult("exits", new FolderModel(path[0]).Exist(path[1]));
        }

        public ActionResult Export()
        {
            return Json(false);
        }

        private string[] GetPath(string path)
        {
            long id = Req.GetUser(HttpContext).id;
            string basePath = HttpContext.Server.MapPath("/Storage/uid_" + id);
            string targetPath = HttpContext.Server.MapPath("/Storage/uid_" + id + path);
            return new string[] { basePath, targetPath };
        }
    }
}