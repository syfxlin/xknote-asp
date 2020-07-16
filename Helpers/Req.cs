using System.IO;
using System.Web;
using Newtonsoft.Json.Linq;

namespace xknote.Helpers
{
    public class Req
    {
        public static JObject ParseBody(HttpRequestBase request)
        {
            Stream stream = request.InputStream;
            stream.Seek(0, SeekOrigin.Begin);
            return JObject.Parse(new StreamReader(stream).ReadToEnd());
        }

        public static users GetUser(HttpContextBase ctx)
        { 
            return (users) ctx.Items["user"];
        }
    }
}