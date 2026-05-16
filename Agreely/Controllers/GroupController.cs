using Agreely.Services.DTO.Requests;
using Agreely.Services.Interfaces;
using Agreely.ViewModels;
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
            return View(new CreateGroupViewModel());
        }

        [HttpPost]
        public IActionResult Create(CreateGroupViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);
            try
            {
                var request = new CreateGroupRequest
                {
                    Name = vm.Name,
                    Description = vm.Description,
                    CreatedByUserId = 1 // hardcoded for now
                };
                _groupService.CreateGroup(request);
                TempData["Success"] = "Group created successfully!";
                return RedirectToAction("MyGroups");
            }
            catch (Exception ex)
            {
                ViewData["Error"] = "Something went wrong. Please try again.";
                return View(vm);
            }
        }

        [HttpGet]
        public IActionResult Join()
        {
            return View(new JoinGroupViewModel());
        }

        [HttpPost]
        public IActionResult Join(JoinGroupViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);
            try
            {
                var request = new JoinGroupRequest
                {
                    GroupId = vm.GroupId,
                    UserId = 1 // hardcoded for now
                };
                _groupService.JoinGroup(request);
                TempData["Success"] = "Successfully joined the group!";
                return RedirectToAction("MyGroups");
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                return View(vm);
            }
        }

        [HttpGet]
        public IActionResult Details(int groupId)
        {
            try
            {
                var response = _groupService.GetGroupDetails(groupId);
                var vm = new GroupDetailsViewModel
                {
                    GroupId = response.GroupId,
                    Name = response.Name,
                    Description = response.Description,
                    MemberCount = response.MemberCount,
                    Commitments = response.Commitments
                };
                return View(vm);
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
                var vm = new MyGroupsViewModel { Groups = groups };
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }
    }
}