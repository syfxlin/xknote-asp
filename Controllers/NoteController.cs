using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using xknote.Filters;
using xknote.Helpers;
using xknote.Models;

namespace xknote.Controllers
{
    [Auth]
    public class NoteController : Controller
    {
        [Route("api/notes"), HttpGet]
        public ActionResult Get()
        {
            if (Request["path"] == null)
            {
                return new ErrorResult("Parameter not found. (path)", 400);
            }
            string[] path = this.GetPath(Request["path"]);
            dynamic res = new NoteModel(path[0]).Get(path[1]);
            if (res is JObject)
            {
                return new JObjectResult("note", res);
            }
            return new ErrorResult("Folder not found.", 404);
        }
        
        [Route("api/notes/all"), HttpGet]
        public ActionResult GetAll()
        {
            if (Request["path"] == null)
            {
                return new ErrorResult("Parameter not found. (path)", 400);
            }
            string[] path = this.GetPath(Request["path"]);
            List<string> res = new NoteModel(path[0]).GetAll(path[1]);
            return new JObjectResult("note", res);
        }

        [Route("api/notes"), HttpPost]
        public ActionResult Create()
        {
            JObject body = Req.ParseBody(Request);
            if (body["path"] == null)
            {
                return new ErrorResult("Parameter not found. (path)", 400);
            }
            string[] path = this.GetPath(body["path"].ToString());
            xknoteEntities entities = new xknoteEntities();
            Dictionary<string, config> config = entities.config.Select(i => i).ToDictionary(item => item.config_name);
            string documentExt = config["document_ext"].config_value;
            string documentExtPreg = ".+\\." + documentExt + "$";
            if (!new Regex(documentExtPreg, RegexOptions.IgnoreCase).IsMatch(path[1]))
            {
                return new ErrorResult("Parameter error. (path)", 404);
            }
            JObject info = new JObject();
            info["title"] = body["title"].ToString();
            info["author"] = body["author"].ToString();
            info["created_at"] = body["created_at"].ToString();
            info["updated_at"] = body["updated_at"].ToString();
            string content = body["content"].ToString();
            int code = new NoteModel(path[0]).Create(path[1], info, content);
            if (code == 409)
            {
                return new ErrorResult("The note already exists.", 409);
            }
            return new JObjectResult();
        }

        [Route("api/notes"), HttpDelete]
        public ActionResult Delete()
        {
            if (Request["path"] == null)
            {
                return new ErrorResult("Parameter not found. (path)", 400);
            }
            string[] path = this.GetPath(Request["path"]);
            int code = new NoteModel(path[0]).Delete(path[1]);
            return new JObjectResult();
        }

        [Route("api/notes"), HttpPut]
        public ActionResult Edit()
        {
            JObject body = Req.ParseBody(Request);
            if (body["path"] == null)
            {
                return new ErrorResult("Parameter not found. (path)", 400);
            }
            string[] path = this.GetPath(body["path"].ToString());
            xknoteEntities entities = new xknoteEntities();
            Dictionary<string, config> config = entities.config.Select(i => i).ToDictionary(item => item.config_name);
            string documentExt = config["document_ext"].config_value;
            string documentExtPreg = ".+\\." + documentExt + "$";
            if (!new Regex(documentExtPreg, RegexOptions.IgnoreCase).IsMatch(path[1]))
            {
                return new ErrorResult("Parameter error. (path)", 404);
            }
            JObject info = new JObject();
            info["title"] = body["title"].ToString();
            info["author"] = body["author"].ToString();
            info["created_at"] = body["created_at"].ToString();
            info["updated_at"] = body["updated_at"].ToString();
            string content = body["content"].ToString();
            int code = new NoteModel(path[0]).Edit(path[1], info, content);
            return new JObjectResult();
        }

        [Route("api/notes/move"), HttpPut]
        public ActionResult Move()
        {
            JObject body = Req.ParseBody(Request);
            if (body["old_path"] == null || body["new_path"] == null)
            {
                return new ErrorResult("Parameter not found. (old_path, new_path)", 400);
            }
            string[] oldPath = this.GetPath(body["old_path"].ToString());
            string[] newPath = this.GetPath(body["new_path"].ToString());
            if (new Regex(@"\.\.\/").IsMatch(newPath[1] + oldPath[1]))
            {
                return new ErrorResult("You submitted a restricted character. (../)", 400);
            }
            xknoteEntities entities = new xknoteEntities();
            Dictionary<string, config> config = entities.config.Select(i => i).ToDictionary(item => item.config_name);
            string documentExt = config["document_ext"].config_value;
            string documentExtPreg = ".+\\." + documentExt + "$";
            if (!new Regex(documentExtPreg, RegexOptions.IgnoreCase).IsMatch(newPath[1]))
            {
                return new ErrorResult("Parameter error. (path)", 404);
            }
            int code = new NoteModel(newPath[0]).Move(newPath[1], oldPath[1]);
            if (code == 404)
            {
                return new ErrorResult("Folder not found.", 404);
            }
            return new JObjectResult();
        }

        [Route("api/notes/rename"), HttpPut]
        public ActionResult Rename()
        {
            return this.Move();
        }

        [Route("api/notes/check"), HttpPost]
        public ActionResult CheckStatus()
        {
            JObject body = Req.ParseBody(Request);
            if (body["check_list"] == null)
            {
                return new ErrorResult("Parameter not found. (check_list)", 400);
            }
            string[] pathList = body["check_list"].ToObject<string[]>();
            string[] checkList = new string[pathList.Length];
            string basePath = this.GetPath("")[0];
            for (int i = 0; i < pathList.Length; i++)
            {
                checkList[i] = this.GetPath(pathList[i])[1];
            }
            JObject res = new NoteModel(basePath).CheckStatus(checkList, pathList);
            return new JObjectResult("check_list", res);
        }

        [Route("api/notes/exist"), HttpGet]
        public ActionResult Exist()
        {
            if (Request["path"] == null)
            {
                return new ErrorResult("Parameter not found. (path)", 400);
            }
            string[] path = this.GetPath(Request["path"]);
            bool res = new NoteModel(path[0]).Exist(path[1]);
            return new JObjectResult("exist", res);
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