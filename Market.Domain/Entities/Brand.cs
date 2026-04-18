namespace Market.Domain.Entities;

public class Brand
{
    public Guid Id { get; set; }
    
    public required string Name { get; set; } // Наприклад: "Razer", "Logitech"
    
    public string? Description { get; set; } // Опис бренду (необов'язково)

    // Зв'язок один-до-багатьох: один бренд має багато товарів
    public ICollection<Product> Products { get; set; } = [];
}
