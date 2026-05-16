using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Agreely.Controllers
{
    public class BaseController : Controller
    {
        protected int GetSessionUserId() => HttpContext.Session.GetInt32("UserId") ?? 0;
        protected string GetSessionFullName() => HttpContext.Session.GetString("FullName") ?? "";

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controllerName = context.RouteData.Values["controller"]?.ToString();
            if (controllerName != "Auth" && GetSessionUserId() == 0)
            {
                context.Result = RedirectToAction("Login", "Auth");
                return;
            }
            base.OnActionExecuting(context);
        }
    }
}