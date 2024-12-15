using TaskManagement.Data;
using TaskManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace TaskManagement.Controllers
{
    public class LoginController : Controller
    {
        private readonly TaskManagementContext _db;

        public LoginController(TaskManagementContext db)
        {
            _db = db;
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            var userFromDb = _db.User.Where(x => x.EmployeeNo == model.EmployeeNo).FirstOrDefault();
            if (userFromDb != null)
            {
                if (userFromDb.Password == model.Password)
                {
                    HttpContext.Session.SetInt32("EmployeeNo", model.EmployeeNo);


                    if (userFromDb.Active == true)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return View("Forbidden");
                    }
                }
            }

            return View();
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("EmployeNo");
            return View("Login");
        }
    }
}
