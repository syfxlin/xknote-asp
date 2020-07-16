using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;

namespace xknote.Models
{
    public class JObjectResult: ActionResult
    {
        private readonly JContainer _data;

        public JObjectResult()
        {
            JContainer jObject = new JObject();
            jObject["error"] = false;
            this._data = jObject;
        }
        
        public JObjectResult(string key, JContainer data)
        {
            JObject jObject = new JObject();
            jObject["error"] = false;
            jObject[key] = data;
            this._data = jObject;
        }

        public JObjectResult(string key, object data)
        {
            JObject jObject = new JObject();
            jObject["error"] = false;
            jObject.Add(new JProperty(key, data));
            this._data = jObject;
        }

        public JObjectResult(JProperty property)
        {
            JObject jObject = new JObject();
            jObject["error"] = false;
            jObject.Add(property);
            this._data = jObject;
        }
        
        public override void ExecuteResult(ControllerContext context)
        {
            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = "application/json";
            response.Write(this._data.ToString());
        }
    }
}