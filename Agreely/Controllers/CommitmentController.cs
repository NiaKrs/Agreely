using Agreely.Services.DTO.Requests;
using Agreely.Services.Interfaces;
using Agreely.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Agreely.Controllers
{
    public class CommitmentController : BaseController
    {
        private readonly ICommitmentService _commitmentService;
        private readonly IVoteService _voteService;

        public CommitmentController(ICommitmentService commitmentService, IVoteService voteService)
        {
            _commitmentService = commitmentService;
            _voteService = voteService;
        }

        [HttpGet]
        public IActionResult Create(int groupId)
        {
            return View(new CreateCommitmentViewModel { GroupId = groupId });
        }

        [HttpPost]
        public IActionResult Create(CreateCommitmentViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);
            try
            {
                var request = new CreateCommitmentRequest
                {
                    Title = vm.Title,
                    Description = vm.Description,
                    GroupId = vm.GroupId,
                    CreatedByUserId = GetSessionUserId()
                };
                _commitmentService.CreateCommitment(request);
                TempData["Success"] = "Commitment created successfully!";
                return RedirectToAction("Details", "Group", new { groupId = vm.GroupId });
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                return View(vm);
            }
        }

        [HttpGet]
        public IActionResult Edit(int id, int groupId)
        {
            try
            {
                var response = _commitmentService.GetCommitmentById(id);
                if (response == null)
                {
                    TempData["Error"] = "Commitment not found.";
                    return RedirectToAction("MyGroups", "Group");
                }
                var vm = new EditCommitmentViewModel
                {
                    CommitmentId = response.CommitmentId,
                    GroupId = groupId,
                    Title = response.Title,
                    Description = response.Description
                };
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("MyGroups", "Group");
            }
        }

        [HttpPost]
        public IActionResult Edit(EditCommitmentViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);
            try
            {
                var request = new CreateCommitmentVersionRequest
                {
                    CommitmentId = vm.CommitmentId,
                    Title = vm.Title,
                    Description = vm.Description,
                    CreatedByUserId = GetSessionUserId()
                };
                _commitmentService.CreateCommitmentVersion(request);
                TempData["Success"] = "Commitment updated successfully!";
                return RedirectToAction("Details", "Group", new { groupId = vm.GroupId });
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                return View(vm);
            }
        }

        [HttpPost]
        public IActionResult Delete(int id, int groupId)
        {
            try
            {
                _commitmentService.DeleteCommitment(id);
                TempData["Success"] = "Commitment deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Details", "Group", new { groupId });
        }

        [HttpPost]
        public IActionResult Vote(CastVoteRequest request, int groupId)
        {
            try 
            { 
                request.UserId = GetSessionUserId();
                _voteService.CastOrUpdateVote(request);
                TempData["Success"] = "Vote cast successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Details", "Group", new { groupId });
        }
    }
}