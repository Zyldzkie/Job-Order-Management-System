using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Data;
using TaskManagement.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManagement.Authentication;
using Microsoft.EntityFrameworkCore;

namespace TaskManagement.Controllers
{
    public class ProjectController : Controller
    {
        private readonly TaskManagementContext _context;

        public ProjectController(TaskManagementContext context)
        {
            _context = context;
        }

        // GET: Project
        public IActionResult Index(DateTime? startCreatedDate, DateTime? endCreatedDate, string statusFilter = "All")
        {
            var roleName = HttpContext.Session.GetString("RoleName");
            var userName = HttpContext.Session.GetString("UserName");

            var projects = _context.Project.ToList();

            if (roleName == "User")
            {
                projects = projects.Where(x => x.CreatedBy == userName).ToList();
            }
            else if (roleName == "Manager")
            {
                projects = projects.Where(x => x.Status == "For Approval" || x.Status == "Approved").ToList();
            }
            else if (roleName == "Staff")
            {
                projects = projects.Where(x => x.Status == "Approved" && x.EndDate < DateTime.Now).ToList();
            }

            projects = projects.Where(t => (statusFilter == "All" || t.Status.Equals(statusFilter, StringComparison.OrdinalIgnoreCase))).ToList();

            if (startCreatedDate.HasValue && endCreatedDate.HasValue)
            {
                projects = projects.Where(p => p.CreatedAt >= startCreatedDate && p.CreatedAt <= endCreatedDate).ToList();
            }

            var taskStatusOption = _context.CodeTable.Where(x => x.CodeTableType == "ProjectStatus").ToList();
            taskStatusOption.Insert(0, new CodeTable { Value = "", Name = "-- Select Status --" });
            ViewBag.StatusFilter = new SelectList(taskStatusOption, "Value", "Name");

            return View(projects);
        }

        // GET: Project/Details/5
        public IActionResult Details(int id)
        {
            var project = _context.Project.Include(p => p.Tasks).SingleOrDefault(p => p.ProjectID == id);

            if (project == null)
            {
                return NotFound();
            }

            //var feedbacks = _context.Feedback.Where(f => f.ProjectID == id).ToList();
            //ViewBag.Feedbacks = feedbacks;

            return View(project);
        }

        // GET: Project/Create
        [CustomAuthorizeAttribute(RoleType.User)]
        public IActionResult Create()
        {
            var project = new TaskManagement.Models.Project();
            var departmentOption = _context.CodeTable.Where(x => x.CodeTableType == "Department").ToList();
            departmentOption.Insert(0, new CodeTable { Value = "", Name = "-- Select Department --" });
            ViewBag.Department = new SelectList(departmentOption, "Value", "Name", project.Department);

            return View();
        }

        // POST: Project/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorizeAttribute(RoleType.User)]
        public IActionResult Create([Bind("ProjectID,ProjectName,Description,Department,StartDate,EndDate,Active,CreatedAt,UpdatedAt,CreatedBy,UpdatedBy")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.CreatedAt = DateTime.Now;
                project.Status = "Draft";
                project.Active = true;
                _context.Add(project);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(project);
        }

        // GET: Project/Edit/5
        public IActionResult Edit(int id)
        {
            var project = _context.Project.Find(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        // POST: Project/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorizeAttribute(RoleType.User)]
        public IActionResult Edit(int id, [Bind("ProjectID,ProjectName,Description,Department,StartDate,EndDate,Active,CreatedAt,UpdatedAt,CreatedBy,UpdatedBy")] Project project)
        {
            if (id != project.ProjectID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    project.UpdatedAt = DateTime.Now;
                    _context.Update(project);
                    _context.SaveChanges();
                }
                catch (Exception)
                {
                    if (!ProjectExists(project.ProjectID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // POST: Project/Feedback
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Feedback(Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                feedback.SubmittedOn = DateTime.Now;
                _context.Feedback.Add(feedback);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Feedback submitted successfully!";
                return RedirectToAction(nameof(Index));
            }

            var project = _context.Project.Find(feedback.ProjectID);
            ViewBag.ProjectName = project?.ProjectName;
            return View(feedback);
        }

        // POST: Project/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorizeAttribute(RoleType.User)]
        public IActionResult DeleteConfirmed(int id)
        {
            var project = _context.Project.Find(id);
            _context.Project.Remove(project);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Project.Any(e => e.ProjectID == id);
        }
    }
}
