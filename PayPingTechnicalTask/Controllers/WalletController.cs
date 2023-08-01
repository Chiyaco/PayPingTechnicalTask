using Microsoft.AspNetCore.Mvc;
using PayPingTechnicalTask.Model;
using PayPingTechnicalTask.Model.Enums;
using PayPingTechnicalTask.Model.Transactions;
using PayPingTechnicalTask.Service;

namespace PayPingTechnicalTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public WalletController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("transaction")]
        public async Task<IActionResult> Transaction([FromBody] PostTransactionRequest request)
        {
            var response = new PostTransactionResponse { Result = ResultType.Error };
            try
            {
                var transactionResult = await _transactionService.Transaction(request);

                if (transactionResult.WalletBalance == 0)
                {
                    response.Messages.Add(transactionResult.Message);
                    response.CashFlowResponse = transactionResult;

                    return BadRequest(response);
                }

                response.CashFlowResponse = transactionResult;
                response.Messages.Add(ConstMessage.SuccessMessage);
                response.Result = ResultType.Success;
                return Ok(response);
            }
            catch (Exception)
            {
                response.Result = ResultType.Error;
                response.Messages.Add(ConstMessage.ErrorMessage);
                return BadRequest(response);
            }
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> Transactions(int userId, decimal minAmount, decimal maxAmount, DateTime? fromDate, DateTime? toDate)
        {
            var response = new GetTransactionsResponse { Result = ResultType.Error };
            try
            {
                var searchContext = new TransactionSearchContext
                {
                    FromDate = fromDate,
                    MaxAmount = maxAmount,
                    MinAmount = minAmount,
                    ToDate = toDate,
                    UserId = userId,
                };

                var transactions = await _transactionService.GetTransactions(searchContext);

                response.Transactions = transactions;
                response.Messages.Add(ConstMessage.SuccessMessage);
                response.Result = ResultType.Success;
                return Ok(response);

            }
            catch (Exception)
            {
                response.Result = ResultType.Error;
                response.Messages.Add(ConstMessage.ErrorMessage);
                return BadRequest(response);
            }
        }
    }
}
