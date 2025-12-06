// Models/Category.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace habersite.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; } // Anahtar alan bu olmalı

        [Required(ErrorMessage = "Kategori Adı zorunludur.")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }

        // CS1061 (CreatedDate) hatasını çözen kritik alan
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // İlişki
        public ICollection<News>? News { get; set; }
    }
}