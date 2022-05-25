using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Softlock.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Softlock.App_Code;
using Microsoft.AspNetCore.Authorization;
using Spectrawin.DataAccess;
//using MailKit.Security;
//using Microsoft.Extensions.Options;
//using MimeKit;
//using MimeKit.Text;
using System.IO;
using ClosedXML.Excel;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Net;
using MailKit.Security;

namespace Softlock.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private AppDBContext appDBContext;
        private DataHelper helper;
        private bool _isEmailSendUsingGmail = false;

        public HomeController(ILogger<HomeController> logger, AppDBContext dbContext, IConfiguration configuration)
        {
            appDBContext = dbContext;
            helper = new DataHelper();
            _isEmailSendUsingGmail = Convert.ToBoolean(configuration["AppSettings:SendEmailThorughGmail"]);
            //_isEmailSendUsingGmail = settings.options.

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LicenseDetails()
        {
            var model = new LicenseDetailModel();
            List<Applications> Swlist = this.appDBContext.Applications.Where(a => a.IsActive == true).OrderBy(a => a.ApplicationName).ToList();

            if (Swlist.Count > 0)
            {
                model.ApplicationOptions = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("Select Application", "0")
                };

                foreach (Applications info in Swlist)
                {
                    model.ApplicationOptions.Add(new KeyValuePair<string, string>(info.ApplicationName, info.AppValues));
                }
                model.LicenseType = "User";
            }

            ViewBag.Message = "";
            return View(model);

        }

        public IActionResult RepoLicenseDetails()
        {
            var model = new LicenseDetailModel();
            List<Applications> Swlist = this.appDBContext.Applications.Where(a => a.IsActive == true).OrderBy(a => a.ApplicationName).ToList();

            if (Swlist.Count > 0)
            {
                model.ApplicationOptions = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("Select Application", "0")
                };

                foreach (Applications info in Swlist)
                {
                    model.ApplicationOptions.Add(new KeyValuePair<string, string>(info.ApplicationName, info.AppValues));
                }
                model.ExpirationDays = 365;
                model.LicenseType = "Repo";
            }
            ViewBag.Message = "";
            return View(model);
        }

        public IActionResult MasterLicenseDetails()
        {
            var model = new LicenseDetailModel();
            List<Applications> Swlist = this.appDBContext.Applications.Where(a => a.IsActive == true).OrderBy(a => a.ApplicationName).ToList();

            if (Swlist.Count > 0)
            {
                model.ApplicationOptions = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("Select Application", "0")
                };

                foreach (Applications info in Swlist)
                {
                    model.ApplicationOptions.Add(new KeyValuePair<string, string>(info.ApplicationName, info.AppValues));
                }
                model.ModelNumber = 999;
                model.SerialNumber = "99999999";
                model.LicenseType = "Master";
            }
            ViewBag.Message = "";
            return View(model);
        }

        [HttpPost]
        public IActionResult LicenseDetails(LicenseDetailModel model)
        {
            try
            {
                ViewBag.Message = "";
                //throw new Exception();
                //call Encode to create a License
                var encodeObj = new EncodeLicense();
                string key = encodeObj.EncodeKey(model);

                //Insert into DB - Generated Key details
                this.AddLicenseDetailsToDb(model, key);

                //Send Key to Customer
                if (!string.IsNullOrEmpty(model.CustomerEmail))
                    this.SendEmail(model, key);

                ViewBag.Message = String.Format("License has been generated. Detail file will be automatcially downloaded on your default Download folder.");

                //Generate Excel
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = this.GetWorksheet(model, workbook, key);
                    string fileName = model.CustomerName.Replace(" ", "").Replace(".", "").Trim() + model.SerialNumber + ".xlsx";

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();


                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                           fileName);

                        if (model.LicenseType == "Master")
                            return View("MasterLicensedetails");
                        else if (model.LicenseType == "Master")
                            return View("RepoLicensedetails");
                        else
                            return View("Licensedetails");
                    }
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex.Message);
                LogWriter.LogWrite(ex.Message);
                ViewBag.Message = String.Format("An error occured. Please try again later.");
                return View();
            }
        }

        public ActionResult GetSpectrawinModels(string ApplicationId)
        {
            var output = helper.GetApplicationModels(appDBContext, ApplicationId);
            return Json(output);
        }

        public ActionResult GetOptions(string ApplicationId)
        {
            var output = helper.GetApplicationOptions(appDBContext, ApplicationId);
            return Json(output);
        }

        public ActionResult GetLabels(string ApplicationId)
        {
            var output = helper.GetApplicationLabels(appDBContext, ApplicationId);
            return Json(output);
        }

        [HttpGet]
        public IActionResult Decode()
        {
            ViewBag.Message = "";
            return View();
        }

        [HttpPost]
        public IActionResult Decode(LicenseDecodeModel model)
        {
            ViewBag.Message = "";
            if (ModelState.IsValid)
            {
                var decodeLicense = new DecodeLicense();
                model = decodeLicense.DecodeKey(model);
                if (model.ApplicationName != null || model.SerialNumber != null || model.ModelNumber != null)
                {
                    return View(model);
                }
                else
                {
                    ViewBag.Message = String.Format("Please enter a valid key.");
                    return View("Decode");
                }
            }
            return View("Decode");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private IXLWorksheet GetWorksheet(LicenseDetailModel model, XLWorkbook workbook, string key)
        {
            var worksheet = workbook.Worksheets.Add("License");

            worksheet.Column(1).Width = 1;
            worksheet.Column(2).Width = 20;
            worksheet.Column(3).Width = 20;
            worksheet.Column(4).Width = 20;
            worksheet.Column(5).Width = 20;
            worksheet.Column(6).Width = 20;

            worksheet.Range(1, 1, 36, 25).Style.Border.SetTopBorder(XLBorderStyleValues.Thin).Border.SetTopBorderColor(XLColor.White);
            worksheet.Range(1, 1, 36, 25).Style.Border.SetLeftBorder(XLBorderStyleValues.Thin).Border.SetLeftBorderColor(XLColor.White);
            worksheet.Range(1, 1, 36, 25).Style.Border.SetBottomBorder(XLBorderStyleValues.Thin).Border.SetBottomBorderColor(XLColor.White);
            worksheet.Range(1, 1, 36, 25).Style.Border.SetRightBorder(XLBorderStyleValues.Thin).Border.SetRightBorderColor(XLColor.White);
            worksheet.Range(1, 1, 36, 25).Style.Font.FontSize = 12;

            worksheet.Range(1, 1, 2, 2).Merge();
            worksheet.AddPicture(new MemoryStream(System.IO.File.ReadAllBytes("wwwroot/images/Novanta.png"))).MoveTo(0, 0).Placement = ClosedXML.Excel.Drawings.XLPicturePlacement.FreeFloating;

            worksheet.Range(6, 2, 7, 3).Merge().Value = " PR-" + model.ModelNumber + "  S/N: " + model.SerialNumber;

            worksheet.Cell(6, 3).Style.Border.SetTopBorder(XLBorderStyleValues.Thick).Border.SetTopBorderColor(XLColor.BlueGray);
            worksheet.Cell(6, 2).Style.Border.SetTopBorder(XLBorderStyleValues.Thick).Border.SetTopBorderColor(XLColor.BlueGray);

            worksheet.Cell(7, 2).Style.Border.SetBottomBorder(XLBorderStyleValues.Thick).Border.SetBottomBorderColor(XLColor.BlueGray);
            worksheet.Cell(7, 3).Style.Border.SetBottomBorder(XLBorderStyleValues.Thick).Border.SetBottomBorderColor(XLColor.BlueGray);

            worksheet.Cell(6, 2).Style.Border.SetLeftBorder(XLBorderStyleValues.Thick).Border.SetLeftBorderColor(XLColor.BlueGray);
            worksheet.Cell(7, 2).Style.Border.SetLeftBorder(XLBorderStyleValues.Thick).Border.SetLeftBorderColor(XLColor.BlueGray);

            worksheet.Cell(6, 3).Style.Border.SetRightBorder(XLBorderStyleValues.Thick).Border.SetRightBorderColor(XLColor.BlueGray);
            worksheet.Cell(7, 3).Style.Border.SetRightBorder(XLBorderStyleValues.Thick).Border.SetRightBorderColor(XLColor.BlueGray);

            worksheet.Cell(6, 2).Style.Font.Bold = true;
            worksheet.Cell(6, 2).Style.Font.FontSize = 14;
            worksheet.Cell(6, 2).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Cell(6, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(9, 2).Value = " Customer Name : ";
            worksheet.Cell(9, 3).Style.Font.Bold = true;
            worksheet.Cell(9, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            worksheet.Cell(9, 3).Value = model.CustomerName;

            worksheet.Cell(11, 2).Value = " Order #: ";
            worksheet.Cell(11, 3).Value = model.OrderNumber;
            worksheet.Cell(11, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

            worksheet.Range(13, 2, 13, 6).Merge();
            worksheet.Cell(13, 2).Style.Border.SetTopBorder(XLBorderStyleValues.Thick).Border.SetTopBorderColor(XLColor.AshGrey);
            worksheet.Cell(13, 3).Style.Border.SetTopBorder(XLBorderStyleValues.Thick).Border.SetTopBorderColor(XLColor.AshGrey);
            worksheet.Cell(13, 4).Style.Border.SetTopBorder(XLBorderStyleValues.Thick).Border.SetTopBorderColor(XLColor.AshGrey);
            worksheet.Cell(13, 5).Style.Border.SetTopBorder(XLBorderStyleValues.Thick).Border.SetTopBorderColor(XLColor.AshGrey);
            worksheet.Cell(13, 6).Style.Border.SetTopBorder(XLBorderStyleValues.Thick).Border.SetTopBorderColor(XLColor.AshGrey);

            worksheet.Range(14, 2, 14, 6).Merge();
            worksheet.Row(14).Height = 30;
            if (model.LicenseOptions != null && model.LicenseOptions.Length > 0)
                worksheet.Cell(14, 2).Value = " " + helper.GetApplicationName(model.Application) + "  " + GetOptionsString(model.LicenseOptions);
            else
                worksheet.Cell(14, 2).Value = " " + helper.GetApplicationName(model.Application);
            worksheet.Cell(14, 2).Style.Font.FontSize = 14;
            worksheet.Cell(14, 2).Style.Font.FontColor = XLColor.Black;
            worksheet.Cell(14, 2).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Cell(14, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

            worksheet.Range(15, 2, 15, 6).Merge();

            worksheet.Range(16, 2, 17, 6).Merge().Value = key;
            worksheet.Cell(16, 2).Style.Font.Bold = true;
            worksheet.Cell(16, 2).Style.Font.FontSize = 16;
            worksheet.Cell(16, 2).Style.Font.FontColor = XLColor.AirForceBlue;
            worksheet.Cell(16, 2).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Cell(16, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Range(18, 2, 18, 6).Merge();
            worksheet.Cell(18, 2).Style.Border.SetBottomBorder(XLBorderStyleValues.Thick).Border.SetBottomBorderColor(XLColor.AshGrey);
            worksheet.Cell(18, 3).Style.Border.SetBottomBorder(XLBorderStyleValues.Thick).Border.SetBottomBorderColor(XLColor.AshGrey);
            worksheet.Cell(18, 4).Style.Border.SetBottomBorder(XLBorderStyleValues.Thick).Border.SetBottomBorderColor(XLColor.AshGrey);
            worksheet.Cell(18, 5).Style.Border.SetBottomBorder(XLBorderStyleValues.Thick).Border.SetBottomBorderColor(XLColor.AshGrey);
            worksheet.Cell(18, 6).Style.Border.SetBottomBorder(XLBorderStyleValues.Thick).Border.SetBottomBorderColor(XLColor.AshGrey);

            worksheet.Cell(20, 2).Value = " Issued On: ";
            worksheet.Cell(20, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            worksheet.Cell(20, 3).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm tt");

            worksheet.Cell(22, 2).Value = " Expires: ";
            worksheet.Cell(22, 3).Style.Font.Bold = true;
            worksheet.Cell(22, 3).Style.Font.FontColor = XLColor.OrangeRed;
            worksheet.Cell(22, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            worksheet.Cell(22, 3).Value = model.ExpirationDays + " days after registration";

            return worksheet;
        }


        private void AddLicenseDetailsToDb(LicenseDetailModel model, string key)
        {
            string optns = "";
            if (model.LicenseOptions != null && model.LicenseOptions.Length > 0)
            {
                foreach (var opt in model.LicenseOptions)
                {
                    optns += opt + ",";
                }
            }

            if (optns.Length > 1)
                optns.Remove(optns.Length - 1);

            var objLicenseDetails = new LicenseDetails();
            objLicenseDetails.Application = model.Application;
            objLicenseDetails.CustomerEmail = model.CustomerEmail;
            objLicenseDetails.CustomerName = model.CustomerName;
            objLicenseDetails.ExpirationDays = model.ExpirationDays;
            objLicenseDetails.ModelNumber = model.ModelNumber;
            objLicenseDetails.OrderNumber = model.OrderNumber;
            objLicenseDetails.SerialNumber = model.SerialNumber;
            objLicenseDetails.LicenseOptions = optns;
            objLicenseDetails.CreatedBy = User.Identity.Name;
            objLicenseDetails.CreatedOn = DateTime.Now;
            objLicenseDetails.Key = key;
            objLicenseDetails.LicenseType = model.LicenseType;

            var context = this.appDBContext.Add(objLicenseDetails);
            Task<int> result = this.appDBContext.SaveChanges();
        }


        private void SendEmail(LicenseDetailModel model, string key)
        {
            string appName = helper.GetApplicationName(model.Application);
            SmtpClient smtpServer = new SmtpClient("mail.gsig.com", 25);            
            try
            {

                if (_isEmailSendUsingGmail)
                {
                    smtpServer = new SmtpClient("smtp.gmail.com", 587);
                    smtpServer.UseDefaultCredentials = false;                   
                    smtpServer.Credentials = new NetworkCredential("meetdarshinishah@gmail.com", "mwilknrrwzwtnjwk"); 
                }
                else
                {

                    //SmtpServer.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                    //SmtpServer.Credentials = new System.Net.NetworkCredential("neel.shah@novanta.com", "pooPOO2@");

                    smtpServer.Credentials = new System.Net.NetworkCredential("donotreply@novanta.com", "WelcomeDR1");
                    smtpServer.EnableSsl = false;
                    smtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                }


                MailMessage mail = new MailMessage();

                mail.From = new MailAddress("donotreply@novanta.com");
                mail.To.Add(model.CustomerEmail);
                mail.IsBodyHtml = true;
                mail.Subject = "Novanta License Key for Order Number- " + model.OrderNumber;
                mail.Body = "Hello " + model.CustomerName + ", <br/><br/> This is email regarding " + appName + " software license key. <br/><br/> Your software key is as follows: <br/> <br/> <b>"
                                                        + key + "</b><br/><br/> You can download latest " + helper.GetApplicationName(model.Application) + " software version from <a href='https://www.jadaktech.com/resources/photo-research-document-library/spectrawin-2-software/'> " + appName + "</a>."
                                                        + "<br/><br/>For any query please <a href='https://www.jadaktech.com/contact-us/'> Contact Us </a>. <br/><br/> Regards, <br/> Team Novanta Inc.";

                smtpServer.Send(mail);
                //smtpServer.Dispose();
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex.Message);
                LogWriter.LogWrite("Send Email: -" + ex.Message);
                smtpServer.Dispose();
            }

        }

        private string GetOptionsString(string[] modelOptions)
        {
            List<int> licenseOptions = new List<int>();
            foreach (string opt in modelOptions)
            {
                licenseOptions.Add(Convert.ToInt32(opt));
            }

            string options = "";
            foreach (int item in licenseOptions)
            {
                if (item == 1)
                {
                    options += ",FactoryCal ";
                }
                else if (item == 2)
                {
                    options += ",SW + UserCal ";
                }
                else if (item == 8)
                {
                    options += ",UserCal Only ";
                }
                else if (item == 16)
                {
                    options += ",RGB ";
                }
                else if (item == 32)
                {
                    options += ",Macro ";
                }
            }

            if (options.Length > 0)
                options = options.Trim().Remove(0, 1);
            return options;
        }
    }
}
