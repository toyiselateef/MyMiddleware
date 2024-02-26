
using System.ComponentModel.DataAnnotations;

public class WebhookPayload
{
    [Required(ErrorMessage = "Id is required")]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$", ErrorMessage = "Id must be a valid GUID")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Status is required")]
    public int Status { get; set; }
}
