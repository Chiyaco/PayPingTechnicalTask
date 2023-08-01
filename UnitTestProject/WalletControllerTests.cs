using Microsoft.AspNetCore.Mvc;
using Moq;
using PayPingTechnicalTask.Controllers;
using PayPingTechnicalTask.Entity;
using PayPingTechnicalTask.Model;
using PayPingTechnicalTask.Model.Enums;
using PayPingTechnicalTask.Model.Transactions;
using PayPingTechnicalTask.Service;
using Xunit;

namespace UnitTestProject;

public class WalletControllerTests
{
    private readonly WalletController _walletController;
    private readonly Mock<ITransactionService> _serviceMock;

    public WalletControllerTests()
    {
        _serviceMock = new Mock<ITransactionService>();
        _walletController = new WalletController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAllTransactions_Success()
    {
        //Arrange 

        //Act

        var request = new GetTransactionsRequest
        {
            UserId = 1,
            MinAmount = 2,
            MaxAmount = 100,
            FromDate = new DateTime(2023, 7, 1),
            ToDate = new DateTime(2023, 7, 4),
        };

        var expectedTransactions = GetMockTransactions();

        Assert.NotNull(expectedTransactions);


        _serviceMock.Setup(service => service.GetTransactions(It.IsAny<TransactionSearchContext>()))
    .ReturnsAsync((TransactionSearchContext searchContext) =>
    {
        var filteredTransactions = expectedTransactions.FindAll(t =>
            (searchContext.UserId == 0 || t.UserId == searchContext.UserId) &&
            (searchContext.MinAmount == 0 || t.Amount >= searchContext.MinAmount) &&
            (searchContext.MaxAmount == 0 || t.Amount <= searchContext.MaxAmount) &&
            (searchContext.FromDate == null || t.TransactionDate.Date >= searchContext.FromDate.Value.Date) &&
            (searchContext.ToDate == null || t.TransactionDate.Date <= searchContext.ToDate.Value.Date)
        ).OrderBy(t => t.TransactionDate);

        return filteredTransactions.ToList();
    });

        var result = (await _walletController.Transactions(request.UserId, request.MinAmount, request.MaxAmount, request.FromDate, request.ToDate)) as OkObjectResult;

        // Additional check to handle null result.Value
        Assert.NotNull(result);
        Assert.NotNull(result.Value);

        var response = result.Value as GetTransactionsResponse;

        // Assert
        Assert.NotNull(response);
        Assert.Equal(ResultType.Success, response.Result);

        Assert.Contains(ConstMessage.SuccessMessage, response.Messages);
    }

    [Fact]
    public async Task Transactions_Should_Return_Error_When_Exception_Occurs()
    {
        // Arrange
        var request = new GetTransactionsRequest
        {
            UserId = 1,
            MinAmount = 30,
            MaxAmount = 100,
            FromDate = new DateTime(2023, 7, 1),
            ToDate = new DateTime(2023, 7, 3),
        };
        _serviceMock.Setup(service => service.GetTransactions(It.IsAny<TransactionSearchContext>()))
            .ThrowsAsync(new Exception("Some error occurred."));

        // Act
        var result = await _walletController.Transactions(request.UserId, request.MinAmount, request.MaxAmount, request.FromDate, request.ToDate) as BadRequestObjectResult;
        var response = result.Value as GetTransactionsResponse;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(response);
        Assert.Equal(ResultType.Error, response.Result);
        Assert.Contains(ConstMessage.ErrorMessage, response.Messages);
    }

    [Fact]
    public async Task Transaction_Should_Return_Correct_Response()
    {
        // Arrange

        var request = new PostTransactionRequest
        {
            UserId = 1,
            TransactionType = TransactionType.Deposit,
            TransactionAmount = 100
        };

        var expectedWalletBalance = 230; // Set the expected wallet balance

        var expectedResult = new CashFlowResponse
        {
            WalletBalance = expectedWalletBalance
        };

        // Configure the transactionServiceMock for the Deposit method
        _serviceMock.Setup(service => service.Transaction(It.IsAny<PostTransactionRequest>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _walletController.Transaction(request) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);

        var response = result.Value as PostTransactionResponse;
        Assert.NotNull(response);
        Assert.Equal(ResultType.Success, response.Result);
        Assert.Equal(expectedResult.WalletBalance, response.CashFlowResponse.WalletBalance);
        Assert.Contains(ConstMessage.SuccessMessage, response.Messages);
    }

    [Fact]
    public async Task Withdraw_Should_Return_BadRequest_When_BalanceIsNotEnough()
    {
        // Arrange
        var request = new PostTransactionRequest
        {
            UserId = 1,
            TransactionType = TransactionType.Withdraw,
            TransactionAmount = 1000
        };


        var expectedResult = new CashFlowResponse
        {
            WalletBalance = 0,
            Message = ConstMessage.BalanceIsNotEnough
        };

        // Arrange

        // Configure the transactionServiceMock for the Transaction method
        _serviceMock.Setup(service => service.Transaction(It.IsAny<PostTransactionRequest>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _walletController.Transaction(request);

        // Assert
        Assert.NotNull(result);

        // Check if it's a BadRequestObjectResult
        Assert.IsType<BadRequestObjectResult>(result);

        var response = result as BadRequestObjectResult;
        Assert.NotNull(response);

        var message = response.Value as PostTransactionResponse;

        Assert.Equal(ConstMessage.BalanceIsNotEnough, message.CashFlowResponse.Message);
    }

    private List<Transaction> GetMockTransactions()
    {
        // Provide some mock data for testing
        return new List<Transaction>
                    {
                        new Transaction { Id = Guid.NewGuid(), UserId = 1, TransactionDate = new DateTime(2023, 7, 1), Amount = 50 },
                        new Transaction { Id = Guid.NewGuid(), UserId = 2, TransactionDate = new DateTime(2023, 7, 2), Amount = 100 },
                        new Transaction { Id = Guid.NewGuid(), UserId = 1, TransactionDate = new DateTime(2023, 7, 3), Amount = 25 },
                        new Transaction { Id = Guid.NewGuid(), UserId = 3, TransactionDate = new DateTime(2023, 7, 4), Amount = 75 },
                    };
    }
}
