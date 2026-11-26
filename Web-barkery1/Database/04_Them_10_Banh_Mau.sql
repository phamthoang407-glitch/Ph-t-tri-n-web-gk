-- DỮ LIỆU MINH HỌA cho bài bảo vệ, không phải số lượng kho thực tế.
-- Chỉ chạy SAU khi đã cập nhật database phần HSD (03 hoặc EF migration).
-- Script không sửa sản phẩm hiện có, chạy lại không thêm trùng theo tên bánh.
-- Ảnh lấy qua HTTPS Unsplash; cần Internet để hiện ảnh, chỉ dùng cho demo.
-- Chọn đúng database dự án trong SSMS trước khi chạy.
SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @Catalog TABLE (Name nvarchar(150), CategoryName nvarchar(100), Price decimal(18,2), Description nvarchar(2000), ImageUrl nvarchar(500));
INSERT INTO @Catalog VALUES
(N'Bánh kem dâu tây', N'Bánh kem', 185000, N'Cốt bánh bông lan mềm, kem tươi và dâu tây.', N'https://images.unsplash.com/photo-1464305795204-6f5bbfc7fb81?auto=format&fit=crop&w=850&q=85'),
(N'Bánh kem socola', N'Bánh kem', 210000, N'Bánh socola đậm vị với lớp kem mịn.', N'https://images.unsplash.com/photo-1578985545062-69928b1d9587?auto=format&fit=crop&w=850&q=85'),
(N'Bánh mousse chanh dây', N'Bánh kem', 165000, N'Mousse mát lạnh với vị chua ngọt thanh nhẹ.', N'https://images.unsplash.com/photo-1488477181946-6428a0291777?auto=format&fit=crop&w=850&q=85'),
(N'Bánh tiramisu', N'Bánh ngọt', 89000, N'Hương cà phê và cacao hòa quyện trong lớp kem mascarpone.', N'https://images.unsplash.com/photo-1571877227200-a0d98ea607e9?auto=format&fit=crop&w=850&q=85'),
(N'Croissant bơ', N'Bánh mì', 39000, N'Bánh sừng bò nhiều lớp giòn thơm bơ.', N'https://images.unsplash.com/photo-1555507036-ab1f4038808a?auto=format&fit=crop&w=850&q=85'),
(N'Bánh mì phô mai', N'Bánh mì', 45000, N'Bánh mì mềm phủ phô mai béo thơm.', N'https://images.unsplash.com/photo-1509440159596-0249088772ff?auto=format&fit=crop&w=850&q=85'),
(N'Cupcake vani', N'Bánh ngọt', 42000, N'Cupcake vani nhỏ xinh, phủ kem bơ.', N'https://images.unsplash.com/photo-1488477181946-6428a0291777?auto=format&fit=crop&w=850&q=85'),
(N'Bánh quy bơ', N'Bánh quy', 59000, N'Hộp bánh quy giòn, vị bơ truyền thống.', N'https://images.unsplash.com/photo-1499636136210-6f4ee915583e?auto=format&fit=crop&w=850&q=85'),
(N'Donut phủ socola', N'Bánh ngọt', 35000, N'Bánh vòng mềm với lớp phủ socola.', N'https://images.unsplash.com/photo-1551024506-0bccd828d307?auto=format&fit=crop&w=850&q=85'),
(N'Bánh sinh nhật trái cây', N'Bánh sinh nhật', 320000, N'Bánh kem tươi trang trí nhiều loại trái cây.', N'https://images.unsplash.com/photo-1563729784474-d77dbb933a9e?auto=format&fit=crop&w=850&q=85');

INSERT INTO dbo.Categories(Name, Description)
SELECT DISTINCT c.CategoryName, N'Danh mục bánh minh họa'
FROM @Catalog c WHERE NOT EXISTS (SELECT 1 FROM dbo.Categories t WHERE t.Name = c.CategoryName);

DECLARE @Added TABLE (Id int, Name nvarchar(150));
MERGE dbo.Products WITH (HOLDLOCK) AS destination
USING (SELECT c.Name, c.Price, c.Description, c.ImageUrl, t.Id CategoryId FROM @Catalog c INNER JOIN dbo.Categories t ON t.Name=c.CategoryName) AS source
ON destination.Name = source.Name
WHEN NOT MATCHED BY TARGET THEN
  INSERT (Name, Price, Description, ImageUrl, CategoryId, Stock, OldPrice, Rating)
  VALUES (source.Name, source.Price, source.Description, source.ImageUrl, source.CategoryId, 12, 0, 5)
OUTPUT inserted.Id, inserted.Name INTO @Added;

-- Bánh mẫu để thuyết trình: 12 chiếc/lô, hạn 7 ngày tính từ thời điểm chạy.
INSERT INTO dbo.StockBatches(ProductId, Quantity, ExpiryDate, ReceivedAt)
SELECT Id, 12, DATEADD(DAY, 7, CONVERT(date, DATEADD(HOUR, 7, GETUTCDATE()))), GETUTCDATE() FROM @Added;

SELECT Name AS N'Bánh mẫu mới thêm' FROM @Added;
COMMIT TRANSACTION;
