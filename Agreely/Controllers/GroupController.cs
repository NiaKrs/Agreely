using Microsoft.AspNetCore.Mvc;
using Agreely.Models;
using Agreely.Services;

namespace Agreely.Controllers
{
    public class GroupController : Controller
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(string name, string? description)
        {
            int userId = 1;
            int groupId = _groupService.CreateGroup(name, description, userId);
            return RedirectToAction("Index", "Home");
        }
    }
}
