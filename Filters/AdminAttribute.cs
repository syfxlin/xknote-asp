using System.Web;
using System.Web.Mvc;
using xknote.Models;

namespace xknote.Filters
{
    public class AdminAttribute: AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var id = httpContext.Session["login_id"];
            if (id == null || (long) id != 1)
            {
                return false;
            }
            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new ErrorResult("用户没有管理员权限，无法访问。", 401);
        }
    }
}