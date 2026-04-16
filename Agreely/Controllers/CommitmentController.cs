using Microsoft.AspNetCore.Mvc;
using Agreely.Services.DTO;
using Agreely.Services.Interfaces;

namespace Agreely.Controllers
{
    public class CommitmentController : Controller
    {
        private readonly ICommitmentService _commitmentService;

        public CommitmentController(ICommitmentService commitmentService)
        {
            _commitmentService = commitmentService;
        }

        [HttpGet]
        public IActionResult Create(int groupId)
        {
            ViewData["GroupId"] = groupId;
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateCommitmentDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewData["GroupId"] = dto.GroupId;
                return View(dto);
            }
            try
            {
                dto.CreatedByUserId = 1; // hardcoded for now
                int commitmentId = _commitmentService.CreateCommitment(dto);
                TempData["Success"] = "Commitment created successfully!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                ViewData["GroupId"] = dto.GroupId;
                return View();
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {

            try
            {
                var commitment = _commitmentService.GetCommitmentById(id);
                if (commitment == null)
                {
                    TempData["Error"] = "Commitment not found.";
                    return RedirectToAction("MyGroups", "Group");
                }

                var dto = new UpdateCommitmentDto
                {
                    CommitmentId = commitment.CommitmentId,
                    GroupId = commitment.GroupId,
                    Title = commitment.Title,
                    Description = commitment.Description
                };

                return View(dto);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("MyGroups", "Group");
            }
        }

        [HttpPost]
        public IActionResult Edit(UpdateCommitmentDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);
            try
            {
                _commitmentService.UpdateCommitment(dto);
                TempData["Success"] = "Commitment updated successfully!";
                return RedirectToAction("Details", "Group", new { groupId = dto.GroupId });
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                return View(dto);
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
    }
}