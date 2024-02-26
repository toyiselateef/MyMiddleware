
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc; 
using Swashbuckle.AspNetCore.Annotations;

[Authorize] 
public class TransactionsController : BaseController
{
    private readonly ILogger<TransactionsController> logger;
    private readonly AppDBContext db_context;

    public TransactionsController(ILogger<TransactionsController> logger, AppDBContext db_context)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.db_context = db_context ?? throw new ArgumentNullException(nameof(db_context));
    }
    [HttpPost("initiate")]

    #region Swagger Description
    [SwaggerOperation(Summary = "initiate transaction", Description = "initiate transaction on payment gateway.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request successful", typeof(ApiResponse<Transaction>))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error", typeof(ApiResponse<object?>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request", typeof(ApiResponse<object?>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized request", typeof(ApiResponse<object?>))]
    #endregion 
    public ActionResult<Transaction> InitiateTransaction(decimal amount)
    {
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Amount = amount,
            Status = TransactionStatus.Pending,
            Timestamp = DateTime.UtcNow
        };

        db_context.Transactions.Add(transaction);
        db_context.SaveChanges();   
        return Ok(transaction);
    }

    [HttpGet("{id}/status")]

    #region Swagger Description
    [SwaggerOperation(Summary = "get transaction status", Description = "fetch transaction status on payment gateway using transaction id.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request successful", typeof(ApiResponse<string>))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error", typeof(ApiResponse<object?>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request", typeof(ApiResponse<object?>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized request", typeof(ApiResponse<object?>))]
    #endregion 
    public ActionResult<string> GetTransactionStatus(Guid id)
    {
        Transaction transaction = db_context.Transactions?.FirstOrDefault(t => t.Id == id) ?? new();

        if (transaction == null)
           throw new NotFoundException("transaction not found");

        
        return Ok(new ApiResponse<string>
        {
            Success = true,
            Message = "Request successful.",
            Data = transaction.Status.ToString(),
            StatusCode = StatusCodes.Status200OK
        });

    }
}
 
