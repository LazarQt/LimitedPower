using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;

namespace LimitedPower.Remote
{
    public class GoogleDocsHelper
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static string ApplicationName = "LimitedPower.Info";

        private readonly SheetsService SheetsService;
        private readonly string SpreadsheetId;

        public GoogleDocsHelper(string spreadsheetId)
        {
            SpreadsheetId = spreadsheetId;

            UserCredential credential;

            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            // Create Google Sheets API service.
            SheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        public List<List<object>> GetRows(string[] ranges)
        {
            List<List<object>> values = new List<List<object>>();

            foreach (var range in ranges)
            {
                SpreadsheetsResource.ValuesResource.GetRequest request = SheetsService.Spreadsheets.Values.Get(SpreadsheetId, range);
                ValueRange response = request.Execute();
                if (response.Values == null) continue;
                values.AddRange(response.Values.Select(v => v.ToList().ToList()));
            }
            return values;
        }

        public void UpdateRow(IList<IList<object>> values, string range)
        {
            CheckRangeForAvailability(range);
            InsertRowOperation(values, range,
                SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum.OVERWRITE);
        }

        public void AppendRow(IList<IList<object>> values, string range)
        {
            CheckRangeForAvailability(range);
            InsertRowOperation(values, range,
                SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum.INSERTROWS);
        }

        private void CheckRangeForAvailability(string range)
        {
            if (GetRows(new[] { range }).Any()) throw new Exception("cant overwrite data");
        }

        private void InsertRowOperation(IList<IList<object>> values, string range, SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum dataOption)
        {
            var request = new SpreadsheetsResource.ValuesResource.AppendRequest(SheetsService,
                new ValueRange() { Values = values }, SpreadsheetId, range)
            {
                InsertDataOption = dataOption,
                ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW
            };
            request.Execute();
        }
    }
}