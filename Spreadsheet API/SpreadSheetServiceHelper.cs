using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Services.App_Code
{
    public class SpreadSheetServiceHelper
    {
        public static int FindRowNumber(SheetsService service, int RowId)
        {
            var rowNumbers = FindRowNumbers(service, RowId);            
            if (rowNumbers == null)
                return -1;
            return rowNumbers.FirstOrDefault();
        }

        public static IEnumerable<int> FindRowNumbers(SheetsService service, int RowId)
        {
            var apiGetRequest = service.Spreadsheets.Values.Get(AppConfig.SpreadSheetId, "Sheet1!A1:B"); // all data in column A
            var apiGetResponse = apiGetRequest.Execute();

            var apiGetResponseRows = apiGetResponse.Values;            
            if (apiGetResponseRows == null)
                return null;

            return apiGetResponseRows
                .Select((r, i) => new { row = r, index = i }) // create a new wrapper for the original index and row 
                .Skip(1) // skip header 
                .Where(x => Convert.ToInt32(x.row[0]) == RowId) // filter out non relevant rows
                .Select((x) => x.index + 1) // return the rowNumber foreach match
                .ToList(); 
        }

        public static int FindParticipantRowNumber(SheetsService service, int RowId, int ParticipantId)
        {
            var apiGetRequest = service.Spreadsheets.Values.Get(AppConfig.SpreadSheetId, "Sheet1!A1:M"); // all data from A-L
            var apiGetResponse = apiGetRequest.Execute();

            var apiGetResponseRows = apiGetResponse.Values;

            var rowNumber = -1;
            if (apiGetResponseRows == null)
                return rowNumber;

            // find the row number by value
            for (var i = 1 /*skip header*/; i < apiGetResponseRows.Count; i++)
            {
                var row = apiGetResponseRows[i];
                if (Convert.ToInt32(row[0]) == RowId && Convert.ToInt32(row[11]) == ParticipantId)
                {
                    rowNumber = i + 1; // convert index to sheet number
                    break;
                }
            }

            return rowNumber;
        }

        public static int GetLastRowNumber(SheetsService service)
        {
            var apiGetRequest = service.Spreadsheets.Values.Get(AppConfig.SpreadSheetId, "Sheet1!A2:B"); // all data in column A
            var apiGetResponse = apiGetRequest.Execute();

            var apiGetResponseRows = apiGetResponse.Values;
            return apiGetResponseRows.Count;            
        }

        public static List<object> FindRow(SheetsService service, int RowId)
        {
            var rows = FindRows(service, RowId);
            if (rows == null)
                return null;

            return rows.FirstOrDefault().ToList();            
        }
        
        public static IEnumerable<IList<object>> FindRows(SheetsService service, int RowId)
        {
            var apiGetRequest = service.Spreadsheets.Values.Get(AppConfig.SpreadSheetId, "Sheet1!A2:Z"); // all data in sheet
            var apiGetResponse = apiGetRequest.Execute();

            var apiGetResponseRows = apiGetResponse.Values;
            if (apiGetResponseRows == null)
                return null;

            return apiGetResponseRows.Where(r => Convert.ToInt32(r[0]) == RowId).ToList();
        }
    }
}