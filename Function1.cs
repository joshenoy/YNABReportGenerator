using System;
using System.Net.Http.Headers;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Newtonsoft.Json;
using YNABReportGenerator.Models;
using System.Text;
using System.Net.Http.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;

namespace YNABReportGenerator
{
    public class Function1
    {
        private ILogger _log { get; set; }

        [FunctionName("Function1")]
        //public async Task Run([TimerTrigger("0 8 * * 0")]TimerInfo myTimer, ILogger log)
        public async Task Run([TimerTrigger("0 0 8 * * 7")] TimerInfo myTimer, ILogger log)
        {
            _log = log;

            _log.LogInformation($"Generating YNAB expense report: {DateTime.Now}");
            await RequestWeeklyYNABTransactions();
        }

        public async Task RequestWeeklyYNABTransactions()
        {
            string ynabEndPoint = Environment.GetEnvironmentVariable("YNABEndpoint");
            string ynabAccessKey = Environment.GetEnvironmentVariable("YNABAccessKey");
            string ynabBudgetId = Environment.GetEnvironmentVariable("YNABBudgetID");

            string reportStartDate = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");

            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ynabAccessKey);

            string ynabTransactions = await client.GetStringAsync(ynabEndPoint + ynabBudgetId + "/transactions?since_date=" + reportStartDate);

            if (!String.IsNullOrEmpty(ynabTransactions))
                await SendExpenseReportToDiscord(ynabTransactions);
            else
                _log.LogError($"YNAB transaction request failed.");
        }

        public async Task SendExpenseReportToDiscord(string content)
        {
            string webHookURL = System.Environment.GetEnvironmentVariable("WebHookURL");

            TransactionQueryResult reportData = JsonConvert.DeserializeObject<TransactionQueryResult>(content);
            ReportData report = new ReportData(reportData); 

            List<object> fields = new List<object>{};
            for (int i = 0; i < report.reportItems.Count; i++)
            {
                fields.Add(new
                {
                    name = report.reportItems[i].CategoryName,
                    value = report.reportItems[i].Amount.ToString("C2"),
                    inline = false
                });
            }

            var webhookContent = new
            {
                embeds = new List<object>
                {
                    new
                    {
                        type = "rich",
                        title="Weekly Expense report",
                        fields = fields
                    }
                }
            };

            StringContent httpContent = new StringContent(JsonConvert.SerializeObject(webhookContent), Encoding.UTF8, "application/json");
            
            using HttpClient client = new();
            var response = await client.PostAsync(webHookURL, httpContent);

            if(!response.IsSuccessStatusCode)
                _log.LogError($"discord webhook failure: {response.ReasonPhrase}");
        }
    }
}
