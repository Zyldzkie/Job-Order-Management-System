using Microsoft.AspNetCore.Mvc;
using TaskManagement.Data;
using TaskManagement.Models;
using System;
using System.Linq;

namespace TaskManagement.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly TaskManagementContext _context;

        public FeedbackController(TaskManagementContext context)
        {
            _context = context;
        }

        // GET: Feedback/Create
        public IActionResult Create(int projectId)
        {
            // Hanapin ang proyekto gamit ang projectId
            var project = _context.Project.SingleOrDefault(p => p.ProjectID == projectId);
            if (project == null)
            {
                return NotFound();
            }

            // I-set ang project name sa ViewBag para magamit sa view
            ViewBag.ProjectName = project.ProjectName;
            ViewBag.ProjectID = projectId;

            return View();
        }

        // POST: Feedback/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Username, Comments, Rating, ProjectID")] Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                // Itakda ang SubmittedOn field
                feedback.SubmittedOn = DateTime.Now;

                // I-save ang feedback sa database
                _context.Add(feedback);
                _context.SaveChanges();

                // Mag-redirect pabalik sa project details page o kung saan man nais
                TempData["SuccessMessage"] = "Feedback submitted successfully!";
                return RedirectToAction("Details", "Project", new { id = feedback.ProjectID });
            }

            // Kung may error sa form, ipasa ang feedback pabalik sa view
            return View(feedback);
        }

        //// GET: Project/Feedback/5
        //public IActionResult Feedback(int id)
        //{
        //    var project = _context.Project.Find(id);
        //    if (project == null)
        //    {
        //        return NotFound();
        //    }

        //    ViewBag.ProjectName = project.ProjectName;
        //    ViewBag.ProjectID = id;
        //    return View(new Feedback { ProjectID = id });
        //}
    }
}
