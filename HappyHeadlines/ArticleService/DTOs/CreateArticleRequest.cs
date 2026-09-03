using System.ComponentModel.DataAnnotations;
using ArticleService.Models;

namespace ArticleService.DTOs;

public class CreateArticleRequest
{
    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Author { get; set; } = string.Empty;

    [Required]
    public ArticleRegion Region { get; set; }
}