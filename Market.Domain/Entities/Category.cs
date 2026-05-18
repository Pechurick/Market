using System.ComponentModel.DataAnnotations.Schema;

namespace Market.Domain.Entities;

public class Category
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    
    public Guid? ParentId { get; set; } 
    
    [ForeignKey(nameof(ParentId))]
    public Category? ParentCategory { get; set; } 
    
   
    public ICollection<Category> SubCategories { get; set; } = []; 
    
   
    public ICollection<Product> Products { get; set; } = [];
}
