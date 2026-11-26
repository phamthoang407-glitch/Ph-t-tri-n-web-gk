USE [Web_Bakery];
GO
SET XACT_ABORT ON;
GO
BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104170000_CompleteManagement'
)
BEGIN
    CREATE TABLE [NewsArticles] (
        [Id] int NOT NULL IDENTITY,
        [Title] nvarchar(200) NOT NULL,
        [Content] nvarchar(max) NOT NULL,
        [IsPublished] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_NewsArticles] PRIMARY KEY ([Id])
    );
    CREATE TABLE [StoreInfos] (
        [Id] int NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [Address] nvarchar(300) NOT NULL,
        [Phone] nvarchar(30) NULL,
        [OpeningHours] nvarchar(100) NULL,
        CONSTRAINT [PK_StoreInfos] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_StoreInfos_Single] CHECK ([Id] = 1)
    );
    INSERT INTO [StoreInfos] ([Id], [Name], [Address], [Phone], [OpeningHours])
    VALUES (1, N'Quầy bánh Vạn Phúc', N'Hẻm 1, ngách 22/18, làng Lụa Vạn Phúc', NULL, NULL);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104170000_CompleteManagement'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261104170000_CompleteManagement', N'10.0.14');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104172427_ShopExperience'
)
BEGIN
    ALTER TABLE [Products] ADD [OldPrice] decimal(18,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104172427_ShopExperience'
)
BEGIN
    ALTER TABLE [Products] ADD [Rating] decimal(2,1) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104172427_ShopExperience'
)
BEGIN
    ALTER TABLE [NewsArticles] ADD [Category] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104172427_ShopExperience'
)
BEGIN
    ALTER TABLE [NewsArticles] ADD [ImageUrl] nvarchar(500) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104172427_ShopExperience'
)
BEGIN
    ALTER TABLE [Categories] ADD [Description] nvarchar(1000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104172427_ShopExperience'
)
BEGIN
    CREATE TABLE [ShopCartItems] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ProductId] int NOT NULL,
        [Quantity] int NOT NULL,
        CONSTRAINT [PK_ShopCartItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ShopCartItems_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ShopCartItems_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104172427_ShopExperience'
)
BEGIN
    CREATE TABLE [ShopOrders] (
        [Id] int NOT NULL IDENTITY,
        [Code] nvarchar(40) NOT NULL,
        [UserId] nvarchar(450) NULL,
        [GuestTokenHash] nvarchar(64) NULL,
        [CustomerName] nvarchar(100) NOT NULL,
        [CustomerPhone] nvarchar(15) NOT NULL,
        [ShippingAddress] nvarchar(300) NOT NULL,
        [PaymentMethod] nvarchar(10) NOT NULL,
        [Status] nvarchar(20) NOT NULL,
        [TotalAmount] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CompletedAt] datetime2 NULL,
        CONSTRAINT [PK_ShopOrders] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104172427_ShopExperience'
)
BEGIN
    CREATE TABLE [ShopOrderItems] (
        [Id] int NOT NULL IDENTITY,
        [ShopOrderId] int NOT NULL,
        [ProductId] int NOT NULL,
        [ProductName] nvarchar(150) NOT NULL,
        [Price] decimal(18,2) NOT NULL,
        [Quantity] int NOT NULL,
        CONSTRAINT [PK_ShopOrderItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ShopOrderItems_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ShopOrderItems_ShopOrders_ShopOrderId] FOREIGN KEY ([ShopOrderId]) REFERENCES [ShopOrders] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104172427_ShopExperience'
)
BEGIN
    CREATE INDEX [IX_ShopCartItems_ProductId] ON [ShopCartItems] ([ProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104172427_ShopExperience'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ShopCartItems_UserId_ProductId] ON [ShopCartItems] ([UserId], [ProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104172427_ShopExperience'
)
BEGIN
    CREATE INDEX [IX_ShopOrderItems_ProductId] ON [ShopOrderItems] ([ProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104172427_ShopExperience'
)
BEGIN
    CREATE INDEX [IX_ShopOrderItems_ShopOrderId] ON [ShopOrderItems] ([ShopOrderId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104172427_ShopExperience'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ShopOrders_Code] ON [ShopOrders] ([Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104172427_ShopExperience'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_ShopOrders_GuestTokenHash] ON [ShopOrders] ([GuestTokenHash]) WHERE [GuestTokenHash] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104172427_ShopExperience'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261104172427_ShopExperience', N'10.0.14');
END;

COMMIT;
GO

