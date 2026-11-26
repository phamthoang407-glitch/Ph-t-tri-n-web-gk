using Microsoft.EntityFrameworkCore;
using Web_barkery1.Models;

namespace Web_barkery1.data;

// Dữ liệu mới chỉ phục vụ demo. Không sao chép đơn hàng hay tài khoản thật.
public static class DemoDataSeeder
{
    public static async Task SeedAsync(BakeryDbContext db)
    {
        if (await db.Products.AnyAsync()) return;

        var samples = new (string Name, string Category, decimal Price, string Description, string Image)[]
        {
            ("Bánh kem dâu tây", "Bánh kem", 185000, "Cốt bánh mềm, kem tươi và dâu tây.", "https://images.unsplash.com/photo-1464305795204-6f5bbfc7fb81?auto=format&fit=crop&w=850&q=85"),
            ("Bánh kem socola", "Bánh kem", 210000, "Bánh socola với lớp kem mịn.", "https://images.unsplash.com/photo-1578985545062-69928b1d9587?auto=format&fit=crop&w=850&q=85"),
            ("Bánh mousse chanh dây", "Bánh kem", 165000, "Mousse mát lạnh, chua ngọt.", "https://images.unsplash.com/photo-1488477181946-6428a0291777?auto=format&fit=crop&w=850&q=85"),
            ("Bánh tiramisu", "Bánh ngọt", 89000, "Cà phê, cacao và mascarpone.", "https://images.unsplash.com/photo-1571877227200-a0d98ea607e9?auto=format&fit=crop&w=850&q=85"),
            ("Croissant bơ", "Bánh mì", 39000, "Bánh sừng bò giòn thơm.", "https://images.unsplash.com/photo-1555507036-ab1f4038808a?auto=format&fit=crop&w=850&q=85"),
            ("Bánh mì phô mai", "Bánh mì", 45000, "Bánh mì mềm phủ phô mai.", "https://images.unsplash.com/photo-1509440159596-0249088772ff?auto=format&fit=crop&w=850&q=85"),
            ("Cupcake vani", "Bánh ngọt", 42000, "Cupcake phủ kem bơ.", "https://images.unsplash.com/photo-1488477181946-6428a0291777?auto=format&fit=crop&w=850&q=85"),
            ("Bánh quy bơ", "Bánh quy", 59000, "Bánh quy bơ giòn.", "https://images.unsplash.com/photo-1499636136210-6f4ee915583e?auto=format&fit=crop&w=850&q=85"),
            ("Donut phủ socola", "Bánh ngọt", 35000, "Donut mềm phủ socola.", "https://images.unsplash.com/photo-1551024506-0bccd828d307?auto=format&fit=crop&w=850&q=85"),
            ("Bánh sinh nhật trái cây", "Bánh sinh nhật", 320000, "Kem tươi trang trí trái cây.", "https://images.unsplash.com/photo-1563729784474-d77dbb933a9e?auto=format&fit=crop&w=850&q=85")
        };

        var categories = samples.Select(x => x.Category).Distinct()
            .Select(name => new Category { Name = name, Description = "Danh mục bánh minh họa" }).ToList();
        db.Categories.AddRange(categories);
        await db.SaveChangesAsync();

        foreach (var item in samples)
        {
            var product = new Product
            {
                Name = item.Name,
                CategoryId = categories.Single(x => x.Name == item.Category).Id,
                Price = item.Price,
                Description = item.Description,
                ImageUrl = item.Image,
                Stock = 12,
                Rating = 5
            };
            db.Products.Add(product);
            await db.SaveChangesAsync();
            db.StockBatches.Add(new StockBatch
            {
                ProductId = product.Id,
                Quantity = 12,
                ExpiryDate = DateTime.UtcNow.Date.AddDays(7)
            });
        }
        await db.SaveChangesAsync();
    }
}
