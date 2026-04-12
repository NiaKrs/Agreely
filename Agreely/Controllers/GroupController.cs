using Agreely.Services.DTO;
using Agreely.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Create(CreateGroupDto dto)
        {
            try
            {
                dto.CreatedByUserId = 1; // hardcoded for now
                int groupId = _groupService.CreateGroup(dto);
                TempData["Success"] = "Group created successfully!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewData["Error"] = "Something went wrong. Please try again.";
                return View();
            }
        }

        [HttpGet]
        public IActionResult Join()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Join(JoinGroupDto dto)
        {
            try
            {
                dto.UserId = 2; // hardcoded for now
                _groupService.JoinGroup(dto);
                TempData["Success"] = "Successfully joined the group!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                return View();
            }
        }
    }
}