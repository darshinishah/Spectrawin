using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spectrawin.DataAccess;

namespace Softlock.App_Code
{
    public class DataHelper
    {
        public List<SpectrawinModels> GetApplicationModels(AppDBContext dbContext, string applicationValue)
        {
            var result = (from d in dbContext.SpectrawinModels
                         join m in dbContext.ModelApplicationMapping on d.ID equals m.ModelId
                         join a in dbContext.Applications on m.ApplicationId equals a.ID 
                         where a.AppValues == applicationValue && d.IsActive == true
                         select new SpectrawinModels()
                         {
                             ID = d.ID,
                             ModelName = d.ModelName,
                             ModelNumber = d.ModelNumber,
                             IsActive = d.IsActive,
                             CreatedDate = d.CreatedDate
                         }).ToList();

            return result;
            
        }

        public List<Options> GetApplicationOptions(AppDBContext dbContext, string applicationValue)
        {
            var result = (from d in dbContext.Options
                          join m in dbContext.OptionApplicationMapping on d.ID equals m.OptionId
                          join a in dbContext.Applications on m.ApplicationId equals a.ID
                          where a.AppValues == applicationValue && d.IsActive == true
                          select new Options()
                          {
                              ID = d.ID,
                              OptionName = d.OptionName,
                              IsActive = d.IsActive,
                              Value = d.Value,
                              CreatedDate = d.CreatedDate
                          }).ToList();

            return result;

        }

        public List<Labels> GetApplicationLabels(AppDBContext dbContext, string applicationValue)
        {
            var result = (from d in dbContext.Labels
                          join m in dbContext.LabelApplicationMapping on d.Id equals m.LabelId
                          join a in dbContext.Applications on m.ApplicationId equals a.ID
                          where a.AppValues == applicationValue && d.IsActive == true
                          select new Labels()
                          {
                              Id = d.Id,
                              LabelName = d.LabelName,
                              IsActive = d.IsActive,
                              CreatedDate = d.CreatedDate
                          }).ToList();

            return result;

        }

        public string GetApplicationName(string swVersion)
        {
            if (swVersion.ToLower() == "99980600")
            {
                return "SpectraWin 2 ( " + swVersion + " )";
            }
            else if (swVersion.ToLower() == "99091200")
            {
                return "SpectraWin 3 ( " + swVersion + " )";
            }
            else if (swVersion.ToLower() == "99091300")
            {
                return "PhotoWin 2 ( " + swVersion + " )";
            }
            else if (swVersion.ToLower() == "99074200")
            {
                return "5xxCalibrator ( " + swVersion + " )";
            }
            else if (swVersion.ToLower() == "99095100")
            {
                return "SDK ( " + swVersion + " )";
            }
            else if (swVersion.ToLower() == "99100300")
            {
                return "PhotoReader ( " + swVersion + " )";
            }
            else if (swVersion.ToLower() == "99103400")
            {
                return "PhotoView ( " + swVersion + " )";
            }
            else
                return swVersion;
        }
    }
}
