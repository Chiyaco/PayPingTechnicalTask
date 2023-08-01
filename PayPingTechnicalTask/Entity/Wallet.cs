namespace PayPingTechnicalTask.Entity;

public class Wallet : BaseEntity, IAuditEntity
{
    public decimal Balance { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ModifiedAt { get; set; }
}
