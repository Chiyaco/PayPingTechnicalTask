using PayPingTechnicalTask.Data;
using PayPingTechnicalTask.Entity;
using PayPingTechnicalTask.Model;
using PayPingTechnicalTask.Model.Enums;
using PayPingTechnicalTask.Model.Transactions;

namespace PayPingTechnicalTask.Service;

public class TransactionService : ITransactionService
{
    private readonly IRepository<Transaction> _transactionRepository;
    private readonly IRepository<Wallet> _walletRepository;

    public TransactionService(IRepository<Transaction> transactionRepository,
        IRepository<Wallet> walletRepository)
    {
        _transactionRepository = transactionRepository;
        _walletRepository = walletRepository;
    }

    public async Task<CashFlowResponse> Transaction(PostTransactionRequest request)
    {
        var result = new CashFlowResponse();

        if (request.TransactionType is TransactionType.Deposit)
        {
            result = await Deposit(request);
        }

        else if (request.TransactionType is TransactionType.Withdraw)
        {
            result = await Withdraw(request);
        }

        return result;
    }

    public async Task<CashFlowResponse> Deposit(PostTransactionRequest request)
    {
        var transaction = new Transaction
        {
            Amount = request.TransactionAmount,
            TransactionDate = DateTime.Now,
            UserId = request.UserId,
            Status = ResultType.Error,
            TransactionType = TransactionType.Deposit,
        };

        var result = new CashFlowResponse();

        try
        {

            var wallet = (await _walletRepository.GetAll(w => w.UserId == request.UserId)).FirstOrDefault();

            if (wallet is null)
            {
                result.WalletBalance = 0;
                return result;
            }

            var balance = wallet.Balance;


            wallet.Balance = balance + request.TransactionAmount;

            await _walletRepository.Update(wallet);

            transaction.Status = ResultType.Success;
            await _transactionRepository.Insert(transaction);

            await _transactionRepository.Commit();

            result.WalletBalance = wallet.Balance;

            return result;
        }
        catch (Exception)
        {

            transaction.Status = ResultType.Error;

            await _transactionRepository.Insert(transaction);

            await _transactionRepository.Commit();

            result.WalletBalance = 0;

            return result;
        }
    }

    public async Task<CashFlowResponse> Withdraw(PostTransactionRequest request)
    {
        var result = new CashFlowResponse { WalletBalance = 0 };

        var transaction = new Transaction
        {
            Amount = request.TransactionAmount,
            TransactionDate = DateTime.Now,
            UserId = request.UserId,
            Status = ResultType.Error,
            TransactionType = TransactionType.Withdraw,
        };

        try
        {

            var wallet = (await _walletRepository.GetAll(w => w.UserId == request.UserId)).FirstOrDefault();


            if (wallet is null)
            {
                result.Message = ConstMessage.WalletNotFound;
                return result;
            }

            if (wallet.Balance <= request.TransactionAmount)
            {
                result.Message = ConstMessage.BalanceIsNotEnough;
                return result;
            }

            var balance = wallet.Balance;

            wallet.Balance = balance - request.TransactionAmount;

            await _walletRepository.Update(wallet);

            transaction.Status = ResultType.Success;

            await _transactionRepository.Insert(transaction);

            await _transactionRepository.Commit();

            result.WalletBalance = wallet.Balance;

            return result;
        }
        catch (Exception)
        {
            transaction.Status = ResultType.Error;

            await _transactionRepository.Insert(transaction);

            await _transactionRepository.Commit();

            result.WalletBalance = 0;

            return result;
        }
    }

    public async Task<List<Transaction>> GetTransactions(TransactionSearchContext searchContext)
    {
        var transactions = await _transactionRepository.GetAll();

        if (searchContext.FromDate != default)
        {
            transactions = transactions.Where(a => a.TransactionDate.Date >= searchContext.FromDate.Value.Date);
        }
        if (searchContext.ToDate != default)
        {
            transactions = transactions.Where(a => a.TransactionDate.Date <= searchContext.ToDate.Value.Date);
        }

        if (searchContext.UserId != 0)
        {
            transactions = transactions.Where(a => a.UserId == searchContext.UserId);
        }
        if (searchContext.MaxAmount != 0)
        {
            transactions = transactions.Where(a => a.Amount <= searchContext.MaxAmount);
        }
        if (searchContext.MinAmount != 0)
        {
            transactions = transactions.Where(a => a.Amount >= searchContext.MinAmount);
        }

        return transactions.OrderBy(t => t.TransactionDate).ToList();
    }
}