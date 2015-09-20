using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApiTokenAuth.Controller
{
    public class HomeController : System.Web.Mvc.Controller
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            return View();
        }

        public void setResumeDownload()
        {

            StorageCredentials credentials = new StorageCredentials("ultimateacc", "4F8JttoCMFvdrJ+ogx2mTZrU+xd1pBPwaa3O2CYTWFH3QEaWaUzc53ymj6N1q+0Xey4Ght6F9kQsRzb1FwIP2A==");
            CloudStorageAccount account = new CloudStorageAccount(credentials, false);


            // var account = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
            var blobs = account.CreateCloudBlobClient();
            var existingprops = blobs.GetServiceProperties();
            blobs.SetServiceProperties(new Microsoft.WindowsAzure.Storage.Shared.Protocol.ServiceProperties
            {
                DefaultServiceVersion = "2011-08-18",
                Logging = existingprops.Logging,
                HourMetrics = existingprops.HourMetrics,
                Metrics = existingprops.Metrics,
                MinuteMetrics = existingprops.MinuteMetrics,
                Cors = existingprops.Cors,


            });
        
        }
    }
}