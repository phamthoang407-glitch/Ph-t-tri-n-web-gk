using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Web_barkery1.data;

namespace Web_barkery1.Migrations;

[DbContext(typeof(BakeryDbContext))]
[Migration("20261110090000_ExpiryAndCancellation")]
public class ExpiryAndCancellation : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>("CancellationReason", "ShopOrders", maxLength: 500, nullable: true);
        migrationBuilder.AddColumn<string>("CancelledByUserId", "ShopOrders", maxLength: 450, nullable: true);
        migrationBuilder.AddColumn<DateTime>("CancelledAt", "ShopOrders", nullable: true);
        migrationBuilder.CreateTable("StockBatches", table => new
        {
            Id = table.Column<int>(nullable: false).Annotation("SqlServer:Identity", "1, 1"),
            ProductId = table.Column<int>(nullable: false),
            Quantity = table.Column<int>(nullable: false),
            ExpiryDate = table.Column<DateTime>(nullable: true),
            ReceivedAt = table.Column<DateTime>(nullable: false)
        }, constraints: table =>
        {
            table.PrimaryKey("PK_StockBatches", x => x.Id);
            table.ForeignKey("FK_StockBatches_Products_ProductId", x => x.ProductId, "Products", "Id", onDelete: ReferentialAction.Restrict);
            table.CheckConstraint("CK_StockBatches_Quantity", "[Quantity] >= 0");
        });
        migrationBuilder.CreateIndex("IX_StockBatches_ProductId_ExpiryDate", "StockBatches", new[] { "ProductId", "ExpiryDate" });
        migrationBuilder.CreateTable("OrderBatchAllocations", table => new
        {
            Id = table.Column<int>(nullable: false).Annotation("SqlServer:Identity", "1, 1"),
            ShopOrderItemId = table.Column<int>(nullable: false),
            StockBatchId = table.Column<int>(nullable: false),
            Quantity = table.Column<int>(nullable: false)
        }, constraints: table =>
        {
            table.PrimaryKey("PK_OrderBatchAllocations", x => x.Id);
            table.ForeignKey("FK_OrderBatchAllocations_ShopOrderItems_ShopOrderItemId", x => x.ShopOrderItemId, "ShopOrderItems", "Id", onDelete: ReferentialAction.Restrict);
            table.ForeignKey("FK_OrderBatchAllocations_StockBatches_StockBatchId", x => x.StockBatchId, "StockBatches", "Id", onDelete: ReferentialAction.Restrict);
        });
        migrationBuilder.CreateIndex("IX_OrderBatchAllocations_ShopOrderItemId", "OrderBatchAllocations", "ShopOrderItemId");
        migrationBuilder.CreateIndex("IX_OrderBatchAllocations_StockBatchId", "OrderBatchAllocations", "StockBatchId");
        // Lượng tồn cũ chưa có thông tin hạn: chủ quầy phải xác nhận trước khi được bán.
        migrationBuilder.Sql("INSERT INTO [StockBatches] ([ProductId], [Quantity], [ExpiryDate], [ReceivedAt]) SELECT [Id], [Stock], NULL, GETUTCDATE() FROM [Products] WHERE [Stock] > 0");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("OrderBatchAllocations");
        migrationBuilder.DropTable("StockBatches");
        migrationBuilder.DropColumn("CancellationReason", "ShopOrders");
        migrationBuilder.DropColumn("CancelledByUserId", "ShopOrders");
        migrationBuilder.DropColumn("CancelledAt", "ShopOrders");
    }
}
