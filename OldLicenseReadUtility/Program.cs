using System;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Configuration;
using System.Globalization;

namespace OldLicenseReadUtility
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //Console.WriteLine("Hello World!");
                Console.WriteLine("Process started");
                string directory = ConfigurationManager.AppSettings["DirectoryPath"];
                //get Files from folder
                DirectoryInfo d = new DirectoryInfo(directory);
                FileInfo[] files = d.GetFiles();

                for (int i = 0; i < files.Length; i++)
                {
                    string filename = files[i].FullName;
                    filename.Replace(".pdf.pdf", ".pdf", StringComparison.OrdinalIgnoreCase);
                    Console.WriteLine("Processing for " + i + "th file and file name = " + filename);
                    try
                    {

                        PdfReader reader = new PdfReader(filename);
                        int intPageNum = reader.NumberOfPages;
                        string[] words = null;
                        string line;
                        try
                        {
                            for (int j = 1; j <= intPageNum; j++)
                            {
                                string text = PdfTextExtractor.GetTextFromPage(reader, j, new LocationTextExtractionStrategy());
                                if (!String.IsNullOrEmpty(text))
                                {
                                    words = text.Split('\n');
                                    for (int k = 0, len = words.Length; k < len; k++)
                                    {
                                        line = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j]));
                                    }
                                }
                                else
                                {
                                    LogWriter.LogWrite("Error for File Name : " + filename + " and Error is  file is not readable. " + "/n");
                                }
                            }

                            if (words != null && words.Length == 12)
                            {
                                //process old file
                                var details = ProcessOldFile(words);
                                addLicenseDetailsIntoDB(details);
                            }
                            else if (words != null && words.Length == 17)
                            {
                                //process new files
                                var details = ProcessNewFile(words);
                                addLicenseDetailsIntoDB(details);
                            }
                            else if (words != null && words.Length == 18)
                            {
                                //process new files
                                var details = ProcessNewV2File(words);
                                addLicenseDetailsIntoDB(details);
                            }
                            else
                            {
                                LogWriter.LogWrite("Error for File Name : " + files[i].Name + " and Error is  different format. " + "/n");
                            }
                        }
                        catch (Exception ex)
                        {
                            LogWriter.LogWrite("Error for File Name : " + files[i].Name + " and Error is " + ex.Message + "/n");
                        }
                    }
                    catch (Exception ex)
                    {
                        LogWriter.LogWrite("Error for File Name : " + files[i].Name + " and Error is " + ex.Message + "/n");
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.LogWrite(ex.Message + "/n");
            }
            Console.WriteLine("Process Done");
            Console.ReadLine();
        }

        public static LicenseDetails ProcessOldFile(string[] datalines)
        {
            var licenseDetails = new LicenseDetails();
            licenseDetails.LicenseType = "User";
            licenseDetails.CustomerEmail = "";
            licenseDetails.CreatedBy = "Admin Admin";

            string[] custName = datalines[3].Split(":");
            licenseDetails.CustomerName = custName[1].Trim();

            string[] createdOn = datalines[4].Trim().Split("   ");
            licenseDetails.CreatedOn = DateTime.ParseExact(createdOn[2].Trim(), "ddd MMM dd HH:mm:ss yyyy", CultureInfo.InvariantCulture);

            licenseDetails.Application = GetApplicationName(datalines[5].Split(':')[1].Trim());

            if (datalines[6] == "Any Model")
            {
                licenseDetails.ModelNumber = 999;
            }
            else
            {
                string[] modNo = datalines[6].Split(":");
                licenseDetails.ModelNumber = Convert.ToInt32(modNo[1].Trim());
            }

            string[] serNo = datalines[7].Split(":");
            licenseDetails.SerialNumber = serNo[1].Trim();

            licenseDetails.Key = datalines[9].Trim();

            licenseDetails.OrderNumber = "";

            string[] expiry = datalines[11].Trim().Split(" ");
            licenseDetails.ExpirationDays = Convert.ToInt32(expiry[1].Trim());

            return licenseDetails;
        }

        public static LicenseDetails ProcessNewFile(string[] datalines)
        {
            var licenseDetails = new LicenseDetails();
            licenseDetails.LicenseType = "User";
            licenseDetails.CustomerEmail = "";
            licenseDetails.CreatedBy = "Admin Admin";
            licenseDetails.CreatedOn = DateTime.Now;

            string[] modNo = datalines[7].Split(" ");
            licenseDetails.ModelNumber = Convert.ToInt32(modNo[0].Trim().Remove(0, 3));
            licenseDetails.SerialNumber = modNo[3].Trim();

            string[] custName = datalines[8].Split(":");
            licenseDetails.CustomerName = custName[1].Trim();

            string[] ordNo = datalines[9].Split(":");
            licenseDetails.OrderNumber = ordNo[1].Trim();

            licenseDetails.Key = datalines[11].Trim();

            string[] expiry = datalines[14].Split(" ");
            licenseDetails.ExpirationDays = Convert.ToInt32(expiry[1].Trim());

            licenseDetails.Application = GetApplicationName(datalines[10].Trim());

            return licenseDetails;

        }

        public static LicenseDetails ProcessNewV2File(string[] datalines)
        {
            var licenseDetails = new LicenseDetails();
            licenseDetails.LicenseType = "User";
            licenseDetails.CustomerEmail = "";
            licenseDetails.CreatedBy = "Admin Admin";

            string[] createdOn = datalines[13].Trim().Split(":", 2);
            try
            {
                licenseDetails.CreatedOn = DateTime.ParseExact(createdOn[1].Trim(), "M/d/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
            }
            catch
            {
                licenseDetails.CreatedOn = DateTime.ParseExact(createdOn[1].Trim(), "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
            }
            

            string[] modNo = datalines[7].Split(" ");
            licenseDetails.ModelNumber = Convert.ToInt32(modNo[0].Trim().Remove(0, 3));
            licenseDetails.SerialNumber = modNo[3].Trim();

            string[] custName = datalines[8].Split(":");
            licenseDetails.CustomerName = custName[1].Trim();

            string[] ordNo = datalines[9].Split(":");
            licenseDetails.OrderNumber = ordNo[1].Trim();

            licenseDetails.Key = datalines[11].Trim();

            string[] expiry = datalines[15].Split(" ");
            licenseDetails.ExpirationDays = Convert.ToInt32(expiry[1].Trim());

            licenseDetails.Application = GetApplicationName(datalines[10].Trim());

            return licenseDetails;

        }

        public static string GetApplicationName(string swVersion)
        {
            swVersion = swVersion.Split(" ")[0];
            if (swVersion.ToLower() == "spectrawin" || swVersion.ToLower() == "spectrawin-2" || swVersion.ToLower() == "spectrawin2-lite" || swVersion.ToLower() == "spectrawin2-pro"
                || swVersion.ToLower() == "spectrawin-pro" || swVersion.ToLower() == "spectrawin-lite" || swVersion.ToLower() == "spectrawin2-" || swVersion.ToLower() == "spectrawin-rgb")
            {
                return "99980600";
            }
            else if (swVersion.ToLower() == "spectrawin-3")
            {
                return "99091200";

            }
            else if (swVersion.ToLower() == "photowin")
            {
                return "99073000";
            }
            else if (swVersion.ToLower() == "photowin-2")
            {
                return "99091300";
            }
            else if (swVersion.ToLower() == "5xxcalibrator")
            {
                return "99074200";
            }
            else if (swVersion.ToLower() == "sdk")
            {
                return "99095100";
            }
            else if (swVersion.ToLower() == "photoreader")
            {
                return "99100300";
            }
            else if (swVersion.ToLower() == "photoview")
            {
                return "99103400";
            }
            else
                return swVersion;
        }

        public static void addLicenseDetailsIntoDB(LicenseDetails details)
        {
            try
            {
                AppDBContext appDBContext = new AppDBContext(ConfigurationManager.ConnectionStrings["Softlock"].ToString());
                appDBContext.LicenseDetails.Add(details);
                int result = appDBContext.SaveChanges();
            }
            catch (Exception ex)
            {
                LogWriter.LogWrite("DB Error For Entity : " + details.Key + " and Error is " + ex.Message + "/n");
            }
        }
    }
}
