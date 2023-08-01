namespace PayPingTechnicalTask.Entity;

public interface IAuditEntity
{
    DateTime CreatedAt { get; set; }

    DateTime ModifiedAt { get; set; }
}
