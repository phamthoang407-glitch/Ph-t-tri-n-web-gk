-- Chạy bằng kết nối có quyền DDL trong SSMS. Sao lưu DB trước khi nâng cấp.
USE [Web_Bakery];
GO
SET XACT_ABORT ON;
BEGIN TRY
    BEGIN TRANSACTION;
    IF OBJECT_ID(N'dbo.__EFMigrationsHistory', N'U') IS NULL
        THROW 51000, N'Database chưa có schema ban đầu. Xem HUONG_DAN_CHAY.md phần database mới.', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.__EFMigrationsHistory WHERE MigrationId = N'20261104150742_AddIdentityAccounts')
        THROW 51001, N'Chưa có migration AddIdentityAccounts. Không chạy tiếp trước khi cập nhật schema ban đầu.', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.__EFMigrationsHistory WHERE MigrationId = N'20261104170000_CompleteManagement')
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
        INSERT INTO dbo.__EFMigrationsHistory (MigrationId, ProductVersion)
        VALUES (N'20261104170000_CompleteManagement', N'10.0.14');
    END;
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
SELECT MigrationId FROM dbo.__EFMigrationsHistory ORDER BY MigrationId;
