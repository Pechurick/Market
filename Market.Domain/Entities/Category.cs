using System.ComponentModel.DataAnnotations.Schema;

namespace Market.Domain.Entities;

public class Category
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    // Зв'язок з батьківською категорією (Adjacency List)
    public Guid? ParentId { get; set; } 
    
    [ForeignKey(nameof(ParentId))]
    public Category? ParentCategory { get; set; } 
    
    // Вкладені підкатегорії
    public ICollection<Category> SubCategories { get; set; } = []; 
    
    // Товари в цій категорії
    public ICollection<Product> Products { get; set; } = [];
}
