using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Softlock.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Spectrawin.DataAccess;
using Softlock.App_Code;

namespace Softlock.Controllers
{
    public class AccountController : Controller
    {
        private AppDBContext appDBContext;
        

        public AccountController(AppDBContext dbContext)
        {
            appDBContext = dbContext;            
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {

            ClaimsIdentity identity = null;
            bool isAuthenticated = false;

            if (ModelState.IsValid)
            {
                var user = appDBContext.Users.Where(u => u.EmailId == model.UserName && u.Password == model.Password && u.IsActive == true).FirstOrDefault();
                if (user != null)
                {
                    identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, user. FirstName + " " + user.LastName),
                    new Claim(ClaimTypes.Role, user.UserRole)
                    }, CookieAuthenticationDefaults.AuthenticationScheme);
                    isAuthenticated = true;      
                }
                //if (model.UserName == "admin@spectrawin.com" && model.Password == "Test@123")
                //{
                //    //Create the identity for the user  
                //    identity = new ClaimsIdentity(new[] {
                //    new Claim(ClaimTypes.Name, model.UserName),
                //    new Claim(ClaimTypes.Role, "Admin")
                //    }, CookieAuthenticationDefaults.AuthenticationScheme);
                //    isAuthenticated = true;                    
                //}
                //if (model.UserName == "superuser@spectrawin.com" && model.Password == "Test@123")
                //{
                //    //Create the identity for the user  
                //    identity = new ClaimsIdentity(new[] {
                //    new Claim(ClaimTypes.Name, model.UserName),
                //    new Claim(ClaimTypes.Role, "SuperUser")
                //    }, CookieAuthenticationDefaults.AuthenticationScheme);
                //    isAuthenticated = true;
                //}
                //else if (model.UserName == "user@spectrawin.com" && model.Password == "Test@123")
                //{
                //    //Create the identity for the user  
                //    identity = new ClaimsIdentity(new[] {
                //    new Claim(ClaimTypes.Name, model.UserName),
                //    new Claim(ClaimTypes.Role, "User")
                //    }, CookieAuthenticationDefaults.AuthenticationScheme);
                //    isAuthenticated = true;
                //}

                if (isAuthenticated)
                {
                    var principal = new ClaimsPrincipal(identity);
                    var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    return RedirectToAction("Index", "Home");
                }

            }
            return RedirectToAction("Login");
            
        }

        [HttpGet]
        public IActionResult Logout()
        {
            var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
