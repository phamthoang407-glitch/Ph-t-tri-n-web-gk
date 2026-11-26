using System.ComponentModel.DataAnnotations;
namespace Web_barkery1.Models;
public class NewsArticle
{
    [StringLength(500)] public string? ImageUrl { get; set; }
    [StringLength(100)] public string? Category { get; set; }
    public int Id { get; set; }
    [Required, StringLength(200)] public string Title { get; set; } = string.Empty;
    [Required, StringLength(10000)] public string Content { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
