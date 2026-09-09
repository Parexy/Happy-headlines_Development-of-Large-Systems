using ProfanityService.Models;

namespace ProfanityService.DTOs;

public class CheckProfanityRequest
{
    public string Text { get; set; } = string.Empty;

}