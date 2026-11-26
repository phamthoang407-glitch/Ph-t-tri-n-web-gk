using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace Web_barkery1.Migrations;
public partial class CompleteManagement : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
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
""");
    }
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "NewsArticles");
        migrationBuilder.DropTable(name: "StoreInfos");
    }
}
