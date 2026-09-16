using System.ComponentModel.DataAnnotations;
using DraftService.Models;

namespace DraftService.DTOs;

public class CreateDraftRequest
{
    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Author { get; set; } = string.Empty;
}