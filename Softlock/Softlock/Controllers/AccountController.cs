using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Softlock.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Spectrawin.DataAccess;
using Softlock.App_Code;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Softlock.Controllers
{
    public class AccountController : Controller
    {
        private AppDBContext appDBContext;
        private bool _isEmailSendUsingGmail = false;
        private readonly string _appURL;

        public AccountController(AppDBContext dbContext, IConfiguration configuration)
        {
            appDBContext = dbContext;
            _isEmailSendUsingGmail = Convert.ToBoolean(configuration["AppSettings:SendEmailThorughGmail"]);
            _appURL = configuration["AppSettings:AppUrl"].ToString();
        }

        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.Message = "";
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {

            ViewBag.Message = "";
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

                if (isAuthenticated)
                {
                    var principal = new ClaimsPrincipal(identity);
                    var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Message = "UserName/Password is incorrect.";
                    return View("Login");
                }
            }
            else
            {
                ViewBag.Message = "Please enter valid UserName/Password.";
                return View("Login");
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            ViewBag.Message = "";
            return View();
        }

        [HttpGet]
        public IActionResult PasswordReset(string userName)
        {
            return View();
        }

        [HttpPost]
        public IActionResult PasswordReset(PasswordResetModel model)
        {
            if (model.Password != model.ConfirmPassword)
            {
                ViewBag.Massge = "Password and Confirm Password must be matched.";
                return View("Login");
            }
            var user = appDBContext.Users.Where(u => u.EmailId == model.UserName).FirstOrDefault();
            if (user != null)
            {
                user.Password = model.Password;
                Task<int> res = appDBContext.SaveChanges();                
                ViewBag.Massge = "Your password has been updated.";
                return View("Login");
            }
            else {
                ViewBag.Massge = "User not found. Please insert correct User name.";
                return View("Login");
            }
        }

        [HttpPost]
        public IActionResult ForgotPassword(LoginModel model)
        {
                        
            SmtpClient smtpServer = new SmtpClient("mail.gsig.com", 25);

            MailMessage mail = new MailMessage();

            mail.From = new MailAddress("donotreply@novanta.com");
            mail.To.Add(model.UserName);
            mail.IsBodyHtml = true;
            mail.Subject = "Reset Password Link";
            mail.Body = "Hello , <br/><br/> We received a request to reset your password for your Softlock account. We are here to help. <br/><br/>  <b>" +
                                                    "<a href='" + _appURL + "/account/passwordreset?username=" + model.UserName + "'>  Click Here </a> to reset password. </b>" +
                                                    "<br/><br/> If you didn't ask to change your password, don't worry! Your password is still safe and you can ignore this email. <br/><br/> Regards, <br/> Team Novanta Inc.";

            try
            {

                if (_isEmailSendUsingGmail)
                {
                    smtpServer = new SmtpClient("smtp.gmail.com", 587);
                    smtpServer.UseDefaultCredentials = false;
                    smtpServer.EnableSsl = true;
                    smtpServer.Credentials = new NetworkCredential("softlock.novanta@gmail.com", "hmixnsorxgreqdsi");
                }
                else
                {

                    smtpServer.Credentials = new System.Net.NetworkCredential("donotreply@novanta.com", "WelcomeDR1");
                    smtpServer.EnableSsl = false;
                    smtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                }
                
                smtpServer.Send(mail);
                //smtpServer.Dispose();
            }
            catch (Exception ex)
            {
                LogWriter.LogWrite("Send Email: -" + ex.Message);
                smtpServer.Dispose();
            }

            ViewBag.Message = "Your password reset link has been sent successfully over an email.";
            return View();

        }
    }
}
