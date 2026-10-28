using System.ComponentModel.DataAnnotations;

namespace SubscriberService.Models;

public class RemoveSubscriberRequest
{
    [Required]
    public Guid Id { get; set; } = Guid.NewGuid();
}