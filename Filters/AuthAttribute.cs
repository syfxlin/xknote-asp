using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace xknote.Filters
{
    public class AuthAttribute: AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var id = httpContext.Session["login_id"];
            if (id == null)
            {
                return false;
            }

            xknoteEntities entities = new xknoteEntities();
            users user = entities.users.Where(item => item.id == (long) id).FirstOrDefault();
            if (user == null)
            {
                return false;
            }

            httpContext.Items["user"] = user;
            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var url = new UrlHelper(filterContext.RequestContext).Action("Index", "Login");
            filterContext.Result = new RedirectResult(url);
        }
    }
}