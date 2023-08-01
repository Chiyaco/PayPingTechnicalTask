using PayPingTechnicalTask.Model.Enums;

namespace PayPingTechnicalTask.Entity;

public class Transaction : BaseEntity, IAuditEntity
{
    /// <summary>
    /// مبلغ
    /// </summary>
    public decimal Amount { get; set; }

    public int UserId { get; set; }

    public TransactionType TransactionType { get; set; }

    public ResultType Status { get; set; }

    public DateTime TransactionDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ModifiedAt { get; set; }
}