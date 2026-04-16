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
            if (!ModelState.IsValid)
                return View(dto);
            try
            {
                dto.CreatedByUserId = 1; // hardcoded for now
                int groupId = _groupService.CreateGroup(dto);
                TempData["Success"] = "Group created successfully!";
                return RedirectToAction("MyGroups", "Group");
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
            if (!ModelState.IsValid)
                return View(dto);
            try
            {
                dto.UserId = 1; // hardcoded for now
                _groupService.JoinGroup(dto);
                TempData["Success"] = "Successfully joined the group!";
                return RedirectToAction("MyGroups", "Group");
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                return View();
            }
        }

        [HttpGet]
        public IActionResult Details(int groupId)
        {
            try
            {
                var details = _groupService.GetGroupDetails(groupId);
                return View(details);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult MyGroups()
        {
            try
            {
                int userId = 1; // hardcoded for now
                var groups = _groupService.GetUserGroups(userId);
                return View(groups);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }
    }
}