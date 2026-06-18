using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Agreely.Controllers
{
    public class BaseController : Controller
    {
        protected int GetSessionUserId()
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(value, out int id) ? id : 0;
        }

        protected string GetSessionFullName()
        {
            return User.FindFirstValue(ClaimTypes.Name) ?? "";
        }


    }
}