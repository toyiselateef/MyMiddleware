using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;

[Authorize] 
 
public class WebHookController : BaseController
{
    private readonly ILogger<WebHookController> logger;
    private readonly AppDBContext db_context;

    public WebHookController(ILogger<WebHookController> logger, AppDBContext db_context)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.db_context = db_context ?? throw new ArgumentNullException(nameof(db_context)); 
    }

    [HttpPost("Receive")]
    #region Swagger Description
    [SwaggerOperation(Summary = "Receive Webhook Notification", Description = "Receive webhook events for transaction update on payment gateway.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request successful", typeof(ApiResponse<bool>))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error", typeof(ApiResponse<object?>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request", typeof(ApiResponse<object?>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized request", typeof(ApiResponse<object?>))]
    #endregion 
    public async Task<ActionResult> ReceiveWebhookNotification([FromBody] WebhookPayload payload)
    {
        var request_string = JsonConvert.SerializeObject(payload);
        logger.LogInformation("received Webhook event, with payload: {0}", request_string);
        Transaction transaction = db_context.Transactions.FirstOrDefault(t => t.Id == payload.Id);

        if (transaction == null)
            return NotFound();

        transaction.Status = (TransactionStatus)payload.Status;

        return Ok();
    } 

}

