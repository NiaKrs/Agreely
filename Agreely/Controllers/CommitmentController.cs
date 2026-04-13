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
    }
}