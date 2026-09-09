using ProfanityService.Models;

namespace ProfanityService.DTOs;

public class ProfanityCheckRequest
{
    public string Text { get; set; } = string.Empty;

}