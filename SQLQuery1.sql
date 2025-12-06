-- Kategori ekleme SQL'i
INSERT INTO Categories (Name, Description, IsActive) VALUES
('Ekonomi', 'Ekonomi haberleri', 1),
('Dünya', 'Dünya haberleri', 1),
('Spor', 'Spor haberleri', 1),
('Kadın', 'Kadın haberleri', 1),
('Teknoloji', 'Teknoloji haberleri', 1);

-- Kontrol et
SELECT * FROM Categories;