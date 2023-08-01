namespace PayPingTechnicalTask.Model.Transactions;

public class PostTransactionResponse : BaseResponseModel
{
    public CashFlowResponse CashFlowResponse { get; set; } = new CashFlowResponse();
}