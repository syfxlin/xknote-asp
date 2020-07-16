using System.IO;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;

namespace xknote.Models
{
    public class ErrorResult: ActionResult
    {
        private readonly string _message;
        private readonly int _code;
        
        public ErrorResult(string message, int code)
        {
            _message = message;
            _code = code;
        }
        
        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.StatusCode = _code;
            if (context.HttpContext.Request.Headers["X-Requested-With"] != null)
            {
                JObject jObject = new JObject();
                jObject["error"] = _message;
                context.HttpContext.Response.Write(jObject.ToString());
            }
            else
            {
                ViewResult result = new ViewResult();
                result.ViewName = "~/Views/Error/Error.cshtml";
                result.ViewData["code"] = this._code;
                result.ViewData["message"] = this._message;
                context.HttpContext.Response.Write(RenderRazorViewToString(context, result));
            }
        }
        
        public string RenderRazorViewToString(ControllerContext context, ViewResult result)
        {
            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(context,
                    result.ViewName);
                ViewContext viewContext = new ViewContext(context, viewResult.View,
                    result.ViewData, result.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(context, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}