using System.Web;
using System.Web.Mvc;

namespace xknote.Filters
{
    public class GuestAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var id = httpContext.Session["login_id"];
            if (id != null)
            {
                return false;
            }

            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var url = new UrlHelper(filterContext.RequestContext).Action("Index", "Home");
            filterContext.Result = new RedirectResult(url);
        }
    }
}