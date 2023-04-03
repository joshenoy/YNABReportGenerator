using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YNABReportGenerator.Models
{
    public partial class TransactionQueryResult
    {
        [JsonProperty("data")]
        public TransactionData Data { get; set; }
    }

    public class TransactionData
    {
        [JsonProperty("transactions")]
        public List<Transaction> Transactions { get; set; }

        [JsonProperty("server_knowledge")]
        public long ServerKnowledge { get; set; }
    }

    public class Transaction
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("amount")]
        public long Amount { get; set; }

        [JsonProperty("memo")]
        public string Memo { get; set; }

        [JsonProperty("cleared")]
        public string Cleared { get; set; }

        [JsonProperty("approved")]
        public bool Approved { get; set; }

        [JsonProperty("flag_color")]
        public string FlagColor { get; set; }

        [JsonProperty("account_id")]
        public string AccountId { get; set; }

        [JsonProperty("payee_id")]
        public string PayeeId { get; set; }

        [JsonProperty("category_id")]
        public string CategoryId { get; set; }

        [JsonProperty("transfer_account_id")]
        public string TransferAccountId { get; set; }

        [JsonProperty("transfer_transaction_id")]
        public string TransferTransactionId { get; set; }

        [JsonProperty("matched_transaction_id")]
        public string MatchedTransactionId { get; set; }

        [JsonProperty("import_id")]
        public string ImportId { get; set; }

        [JsonProperty("import_payee_name")]
        public string ImportPayeeName { get; set; }

        [JsonProperty("import_payee_name_original")]
        public string ImportPayeeNameOriginal { get; set; }

        [JsonProperty("debt_transaction_type")]
        public string DebtTransactionType { get; set; }

        [JsonProperty("deleted")]
        public bool Deleted { get; set; }

        [JsonProperty("account_name")]
        public string AccountName { get; set; }

        [JsonProperty("payee_name")]
        public string PayeeName { get; set; }

        [JsonProperty("category_name")]
        public string CategoryName { get; set; }

        [JsonProperty("subtransactions")]
        public List<Subtransaction> Subtransactions { get; set; }
    }

    public class Subtransaction
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("transaction_id")]
        public string TransactionId { get; set; }

        [JsonProperty("amount")]
        public long Amount { get; set; }

        [JsonProperty("memo")]
        public string Memo { get; set; }

        [JsonProperty("payee_id")]
        public string PayeeId { get; set; }

        [JsonProperty("payee_name")]
        public string PayeeName { get; set; }

        [JsonProperty("category_id")]
        public string CategoryId { get; set; }

        [JsonProperty("category_name")]
        public string CategoryName { get; set; }

        [JsonProperty("transfer_account_id")]
        public string TransferAccountId { get; set; }

        [JsonProperty("transfer_transaction_id")]
        public string TransferTransactionId { get; set; }

        [JsonProperty("deleted")]
        public bool Deleted { get; set; }
    }

    public class ReportData
    {
       public List<ReportItem> reportItems { get; set; }

        public ReportData()
        {
            this.reportItems = new List<ReportItem>();
        }

        public ReportData(TransactionQueryResult data)
        {
            this.reportItems = new List<ReportItem>();

            if (data.Data.Transactions.Count > 0)
            {
                this.reportItems = data.Data.Transactions
                                    .GroupBy(x => x.CategoryName)
                                    .Select(g => new ReportItem
                                    {
                                        Amount = g.Sum(x => x.Amount)/-1000.00,
                                        CategoryName = g.Key
                                    })
                                    .Where(g => g.Amount > 0).ToList();
            }
        }
    }

    public class ReportItem
    {
        public double Amount { get; set; }
        public string CategoryName { get; set; }
    }
}
