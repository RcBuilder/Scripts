using Google.Apis.Auth.OAuth2;
using Google.Apis.Pagespeedonline.v2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using Services.App_Code;
using Services.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Services.Controllers
{
    /*
        developers console: 
        enable 'Google Sheets API'

        nuget:
        Install-Package Google.Apis.Sheets.v4 
     
        docs:
        https://developers.google.com/sheets/api/reference/rest/v4/spreadsheets
        https://developers.google.com/sheets/api/reference/rest/v4/spreadsheets.sheets
        https://developers.google.com/sheets/api/reference/rest/v4/spreadsheets.values
        https://developers.google.com/sheets/api/samples/
     
        Authentication:
        1. developer console -> credentials -> Service Account Key -> fill data 
        2. download created json file and move it into your project 
        3. F4 (properties) -> Copy to output directory -> set to ALWAYS     

        note! 
        need to add the service account as a permitted user to access the sheet! 
        go to the developer console -> find the service account -> take it's 'Service account ID' (alternatively: also appears in the credentials.json file)
        open the sheet -> share -> advanced -> refer the 'Service account ID' as a user, add it and set the allowed permissions 
        e.g: general@api-project-635489688114.iam.gserviceaccount.com
         
     
        // Server 2 Server 
        var scopes = new string[] { SheetsService.Scope.Spreadsheets };
        var credentialsFile = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "\\", "credentials.json");

        var credential = GoogleCredential.FromStream(
            new FileStream(credentialsFile, FileMode.Open)
        ).CreateScoped(scopes);

        var service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "test",
        });
        
        // code here ...
    */

    [EnableCors(
        origins: "http://new.atdconf.com,https://www.atdconf.com,http://www.atdconf.com,http://localhost:2072", 
        headers: "*", 
        methods: "*"
    )]
    [LogTraffic]
    [ErrorTraffic]
    public class SpreadSheetController : ApiController
    {
        private static Random Rnd = new Random();

        [HttpPost]
        [Route("api/SpreadSheet/author/add")]
        /*
            POST http://localhost:4338/api/SpreadSheet/author/add 
          
            request:            
            { 
                "rowId": 1234, 
                "name": "Roby", 
                "email" : "blabla@gmail.com", 
                "phone":"054-5555555", 
                "ticketA": 2,  
                "ticketB": 1,
                "ticketC": 1
            }
        */
        public int AddAuthorData([FromBody]AuthorDataRequest request)
        {
            var service = SpreadSheetServiceFactory.Produce();
            var values =  new List<IList<object>>();

            var timestamp = DateTime.Now;

            // generate participants rows                
            Enumerable.Range(0, request.TicketA).ToList().ForEach((index) => {
                values.Add(new List<object> {                    
                    request.RowId,
                    timestamp,
                    request.Name, 
                    request.Phone, 
                    request.Email, 
                    request.TicketA, 
                    request.TicketB, 
                    request.TicketC,                 
                    "", 
                    "",
                    "",
                    Rnd.Next(100000),                     
                    "", 
                    "", 
                    "",
                    "",
                    "",
                    "TicketA"
                });
            });

            Enumerable.Range(0, request.TicketB).ToList().ForEach((index) =>{
                values.Add(new List<object> {                   
                    request.RowId,
                    timestamp,
                    request.Name, 
                    request.Phone, 
                    request.Email, 
                    request.TicketA, 
                    request.TicketB, 
                    request.TicketC,                 
                    "", 
                    "",
                    "",
                    Rnd.Next(100000),                     
                    "", 
                    "", 
                    "",
                    "",
                    "",
                    "TicketB"                    
               });
            });

            Enumerable.Range(0, request.TicketC).ToList().ForEach((index) =>{
                values.Add(new List<object> {                    
                    request.RowId,
                    timestamp,
                    request.Name, 
                    request.Phone, 
                    request.Email, 
                    request.TicketA, 
                    request.TicketB, 
                    request.TicketC,                 
                    "", 
                    "",
                    "",
                    Rnd.Next(100000),                     
                    "", 
                    "", 
                    "",
                    "",
                    "",
                    "TicketC"                    
               });
            });    

            var apiAppendRequest = service.Spreadsheets.Values.Append(new ValueRange
            {
                MajorDimension = "ROWS", // COLUMNS/ ROWS
                Values = values
            }, AppConfig.SpreadSheetId, "Sheet1");

            apiAppendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
            var apiAppendResponse = apiAppendRequest.Execute();

            return apiAppendResponse.Updates.UpdatedRows.HasValue ? apiAppendResponse.Updates.UpdatedRows.Value : 0;
        }

        [HttpPost]
        [Route("api/SpreadSheet/participant/update")]
        /*
            POST http://localhost:4338/api/SpreadSheet/participant/update 
          
            request:
            { 
                "rowId": 1234, 
                "pId": 3344,
                "name": "Avi", 
                "email" : "blabla@gmail.com", 
                "phone":"054-5555555", 
                "jobTitle": "Developer",
                "company": "Celcom", 
                "ticket": "TicketA"
            }
        */
        public int UpdateParticipantData([FromBody]ParticipantDataRequest request)
        {
            var service = SpreadSheetServiceFactory.Produce();

            var rowNumber = SpreadSheetServiceHelper.FindParticipantRowNumber(service, request.RowId, request.pId);

            var values = new List<object> { 
                request.Name, request.Phone, request.Email, request.JobTitle, request.Company
            };

            var apiUpdateRequest = service.Spreadsheets.Values.Update(new ValueRange
            {
                MajorDimension = "ROWS", // COLUMNS/ ROWS
                Values = new List<IList<object>> { 
                    values 
                }
            }, AppConfig.SpreadSheetId, string.Format("Sheet1!M{0}", rowNumber)); // M2 = starting point

            apiUpdateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            var apiUpdateResponse = apiUpdateRequest.Execute();

            return apiUpdateResponse.UpdatedColumns.HasValue ? apiUpdateResponse.UpdatedColumns.Value : 0;
        }

        [HttpPost]
        [Route("api/SpreadSheet/payment/update")]
        /*
            POST http://localhost:4338/api/SpreadSheet/payment/update 
          
            request:
            { 
                "rowId": "1234", 
                "invoice": "1234A", 
                "status": "SUCCESS",
                "amount": 200  
            }
        */
        public int UpdatePaymentData([FromBody]PaymentDataRequest request)
        {
            var service = SpreadSheetServiceFactory.Produce();

            var rowNumbers = SpreadSheetServiceHelper.FindRowNumbers(service, request.RowId);

            var values = new List<object> {
                request.Status, request.Invoice, request.Amount
            };

            var result = 0;
            foreach (var rowNumber in rowNumbers) {                
                var apiUpdateRequest = service.Spreadsheets.Values.Update(new ValueRange
                {
                    MajorDimension = "ROWS", // COLUMNS/ ROWS
                    Values = new List<IList<object>> {
                        values
                    }
                }, AppConfig.SpreadSheetId, string.Format("Sheet1!I{0}", rowNumber)); // I2 = starting point

                apiUpdateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
                var apiUpdateResponse = apiUpdateRequest.Execute();
                result += apiUpdateResponse.UpdatedColumns.HasValue? apiUpdateResponse.UpdatedColumns.Value : 0;
            }

            return result;
        }

        [HttpGet]
        [Route("api/SpreadSheet/{rowId}/tickets")]
        /*
            GET http://localhost:4338/api/SpreadSheet/34567/tickets 
            
            response:
            {
	            "rowId": 31519,
	            "ticketA": {
		            "count": 1,
		            "participants": [
			            {
				            "rowId": 31519,
				            "pId": 43239,
				            "name": "-",
				            "email": "-",
				            "phone": "-",
				            "jobTitle": "-",
				            "company": "-",
				            "ticket": "TicketA"
			            }
		            ]
	            },
	            "ticketB": {
		            "count": 2,
		            "participants": [
			            {
				            "rowId": 31519,
				            "pId": 24978,
				            "name": "-",
				            "email": "-",
				            "phone": "-",
				            "jobTitle": "-",
				            "company": "-",
				            "ticket": "TicketB"
			            }
		            ]
	            },
	            "ticketC": {
		            "count": 2,
		            "participants": [
			            {
				            "rowId": 31519,
				            "pId": 78304,
				            "name": "-",
				            "email": "-",
				            "phone": "-",
				            "jobTitle": "-",
				            "company": "-",
				            "ticket": "TicketC"
			            }
		            ]
	            }
            }
        */
        public TicketsDataResponse GetTicketsData(int rowId)
        {
            var service = SpreadSheetServiceFactory.Produce();

            var rowsData = SpreadSheetServiceHelper.FindRows(service, rowId);
            if (rowsData == null) 
                return null;

            var response = new TicketsDataResponse{ 
                RowId = rowId,
                TicketA = new TicketData(),
                TicketB = new TicketData(),
                TicketC = new TicketData()
            };

            foreach (var rowData in rowsData) {
                var participant = new ParticipantDataRequest
                {
                    RowId = rowId,
                    pId = Convert.ToInt32(rowData[11]),                    
                    Name = rowData[12].ToString(),
                    Phone = rowData[13].ToString(),
                    Email = rowData[14].ToString(),
                    JobTitle = rowData[15].ToString(),
                    Company = rowData[16].ToString(),
                    TicketName = rowData[17].ToString()
                };

                switch (participant.TicketName.ToLower()) {
                    case "ticketa": 
                        response.TicketA.Participants.Add(participant);
                        break;
                    case "ticketb":
                        response.TicketB.Participants.Add(participant);
                        break;
                    case "ticketc":
                        response.TicketC.Participants.Add(participant);
                        break;
                }
            }

            return response;
        }
    }
}
