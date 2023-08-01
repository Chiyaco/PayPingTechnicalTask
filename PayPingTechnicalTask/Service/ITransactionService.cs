using PayPingTechnicalTask.Entity;
using PayPingTechnicalTask.Model;
using PayPingTechnicalTask.Model.Transactions;

namespace PayPingTechnicalTask.Service;

public interface ITransactionService
{
    Task<CashFlowResponse> Transaction(PostTransactionRequest request);

    Task<CashFlowResponse> Deposit(PostTransactionRequest request);

    Task<CashFlowResponse> Withdraw(PostTransactionRequest request);

    Task<List<Transaction>> GetTransactions(TransactionSearchContext context);
}
