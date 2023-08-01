using PayPingTechnicalTask.Model.Enums;

namespace PayPingTechnicalTask.Model.Transactions;

public class PostTransactionRequest
{
    public int UserId { get; set; }

    public TransactionType TransactionType { get; set; }

    public decimal TransactionAmount { get; set; }
}