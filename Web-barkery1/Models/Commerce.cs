using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Web_barkery1.Models;
public class ShopOrder
{
    public int Id { get; set; }

    [MaxLength(40)]
    public string Code { get; set; } = "BK-" + Guid.NewGuid().ToString("N");

    [MaxLength(450)]
    public string? UserId { get; set; }

    [MaxLength(64)]
    public string? GuestTokenHash { get; set; }

    [MaxLength(100)]
    public string CustomerName { get; set; } = "";

    [MaxLength(15)]
    public string CustomerPhone { get; set; } = "";

    [MaxLength(300)]
    public string ShippingAddress { get; set; } = "";

    [MaxLength(10)]
    public string PaymentMethod { get; set; } = "COD";

    [MaxLength(20)]
    public string Status { get; set; } = "PENDING";

    [Precision(18, 2)]
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    [MaxLength(500)]
    public string? CancellationReason { get; set; }
    [MaxLength(450)]
    public string? CancelledByUserId { get; set; }
    public DateTime? CancelledAt { get; set; }
    public List<ShopOrderItem> Items { get; set; } = new();
}

public class StockBatch
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
    // NULL chỉ dùng cho lượng tồn cũ chưa được chủ quầy xác nhận hạn.
    public DateTime? ExpiryDate { get; set; }
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
}

public class OrderBatchAllocation
{
    public int Id { get; set; }
    public int ShopOrderItemId { get; set; }
    public ShopOrderItem ShopOrderItem { get; set; } = null!;
    public int StockBatchId { get; set; }
    public StockBatch StockBatch { get; set; } = null!;
    public int Quantity { get; set; }
}

public class ShopOrderItem
{
    public int Id { get; set; }
    public int ShopOrderId { get; set; }
    public int ProductId { get; set; }

    [MaxLength(150)]
    public string ProductName { get; set; } = "";

    [Precision(18, 2)]
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

public class ShopCartItem
{
    public int Id { get; set; }

    [MaxLength(450)]
    public string UserId { get; set; } = "";
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
