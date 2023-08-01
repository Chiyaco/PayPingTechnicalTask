using PayPingTechnicalTask.Entity;

namespace PayPingTechnicalTask.Model.Transactions;

public class GetTransactionsResponse : BaseResponseModel
{
    public List<Transaction> Transactions { get; set; } = new List<Transaction>();
}
