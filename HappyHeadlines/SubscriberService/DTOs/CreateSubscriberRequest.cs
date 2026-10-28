namespace SubscriberService.Models;

public class CreateSubscriberRequest
{
    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;
}