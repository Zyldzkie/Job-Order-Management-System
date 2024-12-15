//public class AccountController : Controller
//{
//    private readonly ApplicationDbContext _context;

//    public AccountController(ApplicationDbContext context)
//    {
//        _context = context;
//    }

//    [HttpGet]
//    public IActionResult Register()
//    {
//        return View();
//    }

//    [HttpPost]
//    public async Task<IActionResult> Register(RegistrationModel model)
//    {
//        if (ModelState.IsValid)
//        {
//            // Example: Save user to the database
//            var user = new ApplicationUser
//            {
//                UserName = model.Email,
//                Email = model.Email,
//                FirstName = model.FirstName,
//                LastName = model.LastName
//            };

//            var result = await _userManager.CreateAsync(user, model.Password);
//            if (result.Succeeded)
//            {
//                // Add roles or additional logic if necessary
//                return RedirectToAction("Login", "Account");
//            }

//            foreach (var error in result.Errors)
//            {
//                ModelState.AddModelError(string.Empty, error.Description);
//            }
//        }

//        return View(model);
//    }
//}