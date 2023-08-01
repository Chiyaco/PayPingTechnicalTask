namespace PayPingTechnicalTask.Model.Transactions;

public class TransactionSearchContext
{
    public int UserId { get; set; }

    public decimal MinAmount { get; set; }

    public decimal MaxAmount { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }
}
