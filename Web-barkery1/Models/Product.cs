using System.ComponentModel.DataAnnotations;

namespace Web_barkery1.Models;

public class Product
{
    [Range(0, 999999999)] public decimal OldPrice { get; set; }
    [Range(0, 5)] public decimal Rating { get; set; }
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên bánh.")]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Range(typeof(decimal), "0.01", "999999999", ParseLimitsInInvariantCulture = true,
        ErrorMessage = "Giá bánh phải lớn hơn 0.")]
    public decimal Price { get; set; }

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    // Số bánh còn có thể bán, ban đầu bằng 0.
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    [Range(1, int.MaxValue,
        ErrorMessage = "Vui lòng chọn danh mục.")]
    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;
}