using System.ComponentModel.DataAnnotations;

namespace Web_barkery1.Models;

public class Category
{
    [StringLength(1000)] public string? Description { get; set; }
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên danh mục.")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Product> Products { get; set; }
        = new List<Product>();
}