using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Services.App_Code
{
    public class SpreadSheetServiceFactory
    {
        private static SheetsService service = null;

        public static SheetsService Produce()
        {
            if (service != null) 
                return service;

            //lazy loading ...

            var scopes = new string[] { SheetsService.Scope.Spreadsheets };
            var credentialsFile = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "\\", "credentials.json");

            var credential = GoogleCredential.FromStream(
                new FileStream(credentialsFile, FileMode.Open)
            ).CreateScoped(scopes);

            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "SpreadSheet Service",
            });

            return service;
        }
    }
}