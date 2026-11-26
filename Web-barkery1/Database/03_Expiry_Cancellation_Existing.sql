-- Chạy script này CHỈ khi database được tạo bằng bộ script 00/02 thay vì EF migrations.
-- Sao lưu database trước khi chạy. Script có thể chạy lại và không xóa dữ liệu cũ.
IF COL_LENGTH('dbo.ShopOrders', 'CancellationReason') IS NULL
    ALTER TABLE dbo.ShopOrders ADD CancellationReason nvarchar(500) NULL;
IF COL_LENGTH('dbo.ShopOrders', 'CancelledByUserId') IS NULL
    ALTER TABLE dbo.ShopOrders ADD CancelledByUserId nvarchar(450) NULL;
IF COL_LENGTH('dbo.ShopOrders', 'CancelledAt') IS NULL
    ALTER TABLE dbo.ShopOrders ADD CancelledAt datetime2 NULL;

IF OBJECT_ID('dbo.StockBatches', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.StockBatches (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_StockBatches PRIMARY KEY,
        ProductId int NOT NULL,
        Quantity int NOT NULL CONSTRAINT CK_StockBatches_Quantity CHECK (Quantity >= 0),
        ExpiryDate datetime2 NULL,
        ReceivedAt datetime2 NOT NULL,
        CONSTRAINT FK_StockBatches_Products_ProductId FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id)
    );
    CREATE INDEX IX_StockBatches_ProductId_ExpiryDate ON dbo.StockBatches(ProductId, ExpiryDate);
    INSERT INTO dbo.StockBatches(ProductId, Quantity, ExpiryDate, ReceivedAt)
        SELECT Id, Stock, NULL, GETUTCDATE() FROM dbo.Products WHERE Stock > 0;
END;

IF OBJECT_ID('dbo.OrderBatchAllocations', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.OrderBatchAllocations (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_OrderBatchAllocations PRIMARY KEY,
        ShopOrderItemId int NOT NULL,
        StockBatchId int NOT NULL,
        Quantity int NOT NULL,
        CONSTRAINT FK_OrderBatchAllocations_ShopOrderItems_ShopOrderItemId FOREIGN KEY (ShopOrderItemId) REFERENCES dbo.ShopOrderItems(Id),
        CONSTRAINT FK_OrderBatchAllocations_StockBatches_StockBatchId FOREIGN KEY (StockBatchId) REFERENCES dbo.StockBatches(Id)
    );
    CREATE INDEX IX_OrderBatchAllocations_ShopOrderItemId ON dbo.OrderBatchAllocations(ShopOrderItemId);
    CREATE INDEX IX_OrderBatchAllocations_StockBatchId ON dbo.OrderBatchAllocations(StockBatchId);
END;
