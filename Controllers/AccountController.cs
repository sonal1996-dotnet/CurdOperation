using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApp.Models;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(UserManager<ApplicationUser> userManager,
                             SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // Step 1: User Info
    [HttpGet]
    public IActionResult RegisterStep1() => View();

    [HttpPost]
    public IActionResult RegisterStep1(RegisterStep1ViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        TempData["Step1Data"] = Newtonsoft.Json.JsonConvert.SerializeObject(model);
        return RedirectToAction(nameof(RegisterStep2));
    }

    // Step 2: Company + Password
    [HttpGet]
    public IActionResult RegisterStep2() => View();

    [HttpPost]
    public IActionResult RegisterStep2(RegisterStep2ViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        TempData["Step2Data"] = Newtonsoft.Json.JsonConvert.SerializeObject(model);
        return RedirectToAction(nameof(RegisterStep3));
    }

    // Step 3: Address
    [HttpGet]
    public IActionResult RegisterStep3()
    {
        // You can prepare cascading dropdown data here (countries, states, cities)
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RegisterStep3(RegisterStep3ViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        // Retrieve previous step data from TempData
        var step1Json = TempData["Step1Data"] as string;
        var step2Json = TempData["Step2Data"] as string;
        if (string.IsNullOrEmpty(step1Json) || string.IsNullOrEmpty(step2Json))
        {
            ModelState.AddModelError("All", "Session expired. Please start registration again.");
            return RedirectToAction(nameof(RegisterStep1));
        }

        var step1Data = Newtonsoft.Json.JsonConvert.DeserializeObject<RegisterStep1ViewModel>(step1Json);
        var step2Data = Newtonsoft.Json.JsonConvert.DeserializeObject<RegisterStep2ViewModel>(step2Json);

        var user = new ApplicationUser
        {
            UserName = step1Data.Email,
            Email = step1Data.Email,
            FirstName = step1Data.FirstName,
            LastName = step1Data.LastName,
            DateOfBirth = step1Data.DateOfBirth,
            Mobile = step1Data.Mobile,
            CompanyName = step2Data.CompanyName,
            Country = model.Country,
            State = model.State,
            City = model.City,
            EmailConfirmed = true // optional: email confirmation logic here
        };

        var result = await _userManager.CreateAsync(user, step2Data.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Dashboard", "User");
        }
        foreach (var error in result.Errors)
            ModelState.AddModelError("All", error.Description);

        return View(model);
    }

    // Login Get
    [HttpGet]
    public IActionResult Login() => View();

    // Login Post
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (await _userManager.IsInRoleAsync(user, "SuperAdmin"))
            {
                return RedirectToAction("Index", "SuperAdmin");
            }
            else if (await _userManager.IsInRoleAsync(user, "User"))
            {
                return RedirectToAction("Dashboard", "User");
            }
            else
            {
                // User logged in but has no role assigned - redirect or show error
                await _signInManager.SignOutAsync();
                ModelState.AddModelError("All", "Your account does not have a role assigned. Contact admin.");
                return View(model);
            }
        }

        ModelState.AddModelError("All", "Invalid login attempt.");
        return View(model);
    }

    // Logout
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }
}
