-- Önce Categories tablosuna bak
SELECT * FROM Categories;

-- Eğer boşsa 5 kategori ekle
IF NOT EXISTS (SELECT * FROM Categories)
BEGIN
    INSERT INTO Categories (Name, Description, IsActive) 
    VALUES 
    ('Ekonomi', 'Ekonomi haberleri', 1),
    ('Dünya', 'Dünya haberleri', 1),
    ('Spor', 'Spor haberleri', 1),
    ('Kadın', 'Kadın haberleri', 1),
    ('Teknoloji', 'Teknoloji haberleri', 1);
    
    PRINT '5 kategori eklendi!';
END
ELSE
BEGIN
    PRINT 'Kategoriler zaten var.';
END