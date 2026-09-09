using ProfanityService.Models;

namespace ProfanityService.DTOs;

public class CreateProfanityRequest
{
    public int id { get; set; }

    public string Word { get; set; } = string.Empty;

}