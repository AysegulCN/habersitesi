-- 1. Categories tablosu
CREATE TABLE Categories (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    IsActive BIT DEFAULT 1
);
GO

-- 2. News tablosu
CREATE TABLE News (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(200) NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    ShortDescription NVARCHAR(500) NULL,
    ImageUrl NVARCHAR(500) NULL,
    CreatedDate DATETIME DEFAULT GETDATE(),
    UpdatedDate DATETIME NULL,
    IsActive BIT DEFAULT 1,
    ViewCount INT DEFAULT 0,
    CategoryId INT NOT NULL,
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
);
GO

-- 3. Test verileri (5 kategori)
INSERT INTO Categories (Name, Description, IsActive) 
VALUES 
('Ekonomi', 'Ekonomi haberleri', 1),
('Dünya', 'Dünya haberleri', 1),
('Spor', 'Spor haberleri', 1),
('Kadın', 'Kadın haberleri', 1),
('Teknoloji', 'Teknoloji haberleri', 1);
GO

-- 4. Test haberleri (isteğe bağlı)
INSERT INTO News (Title, Content, CategoryId, IsActive)
VALUES 
('Ekonomide yeni düzenleme', 'Ekonomi içeriği buraya gelecek...', 1, 1),
('Dünya gündemi', 'Dünya içeriği buraya gelecek...', 2, 1),
('Spor haberleri', 'Spor içeriği buraya gelecek...', 3, 1);
GO