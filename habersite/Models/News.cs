// Models/News.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace habersite.Models
{
    public class News
    {
        [Key]
        public int NewsId { get; set; } // Anahtar alan bu olmalı

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Content { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageId { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now; // Yayınlanma/Oluşturma Tarihi
        public bool IsActive { get; set; }

        // Foreign Key
        public int CategoryId { get; set; }

        // Navigation Property
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        public string? Slug { get; set; } // URL dostu, benzersiz metin
    }
}