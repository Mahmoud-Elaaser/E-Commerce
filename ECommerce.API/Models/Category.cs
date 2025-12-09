using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.Models
{
    public class Category
    {
        public Guid Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(500)]
        public string? Description { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public int DisplayOrder { get; set; }
        [MaxLength(500)]
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;

        public Category? ParentCategory { get; set; }
        public ICollection<Category> SubCategories { get; set; } = new List<Category>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
