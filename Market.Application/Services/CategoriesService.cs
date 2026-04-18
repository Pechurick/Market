using Market.Application.Models.Requests;
using Market.Application.Models.Responses;
using Market.Application.Repositories;
using Market.Application.Services.Abstractions;
using Market.Domain.Entities;
using FluentValidation;

namespace Market.Application.Services;

public class CategoriesService(ICategoriesRepository categoriesRepository, IProductsRepository productsRepository, IValidator<CategoryCreateDto> validator, IValidator<CategoryUpdateDto> updateValidator)
    : ICategoriesService
{
    public async Task<IEnumerable<CategoryListDto>> GetAll(CancellationToken cancellationToken)
    {
        // 1. Отримуємо всі категорії (EF Core щойно зібрав з них дерево під капотом)
        var allCategories = await categoriesRepository.GetAll(cancellationToken);
        
        // 2. Вибираємо тільки верхівки дерева (кореневі категорії)
        var rootCategories = allCategories.Where(c => c.ParentId == null);
        
        // 3. Мапимо рекурсивно. Тепер воно пірне хоч на 100 рівнів униз!
        return rootCategories.Select(MapToDto);
    }

    public async Task<CategoryListDto?> GetById(Guid id, CancellationToken cancellationToken)
    {
        // Звертаємось до репозиторію. Якщо пам'ятаєш, там уже є метод Get
        // який робить .FirstOrDefaultAsync()
        var category = await categoriesRepository.Get(id, cancellationToken);

        if (category is null)
        {
            return null; // Повертаємо null, якщо такої категорії немає
        }

        // Використовуємо наш чистий мапер, який ми написали раніше!
        return MapToDto(category);
    }
    
    public async Task Create(CategoryCreateDto request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var category = new Category
        {
            Name = request.Name,
            // Тепер ми просто зберігаємо ID батька. Ніяких милиць зі склеюванням шляхів!
            ParentId = request.ParentId 
        };
        
        await categoriesRepository.Add(category, cancellationToken);
    }

    public async Task Update(Guid id, CategoryUpdateDto request, CancellationToken cancellationToken)
    {
        await updateValidator.ValidateAndThrowAsync(request, cancellationToken);
        var category = await categoriesRepository.Get(id, cancellationToken);

        if (category is null)
        {
            throw new ArgumentNullException(nameof(category));
        }
        
        category.Name = request.Name;
        await categoriesRepository.Update(category, cancellationToken);
    }

    public async Task Delete(Guid id, CancellationToken cancellationToken)
    {
        // Стара перевірка: чи є товари?
        if (await productsRepository.IsProductInCategory(id, cancellationToken))
        {
            throw new InvalidOperationException($"Cannot delete category with id {id} because it contains products.");
        }
        
        // --- НОВА ПЕРЕВІРКА: чи є підкатегорії? ---
        if (await categoriesRepository.HasSubCategories(id, cancellationToken))
        {
            throw new InvalidOperationException($"Cannot delete category with id {id} because it contains subcategories.");
        }
        // ------------------------------------------
        
        var category = await categoriesRepository.Get(id, cancellationToken);
        
        if (category is null)
        {
            throw new ArgumentNullException(nameof(category));
        }
        
        await categoriesRepository.Delete(category, cancellationToken);
    }
    
    // --- НОВИЙ ЧИСТИЙ МЕТОД ---
    // Рекурсивно перетворює сутність Category на CategoryListDto
    private CategoryListDto MapToDto(Category category)
    {
        return new CategoryListDto
        {
            Id = category.Id,
            Name = category.Name,
            // Якщо є підкатегорії - викликаємо цей самий метод для них. Якщо ні - повертаємо порожній список.
            Categories = category.SubCategories?.Select(MapToDto).ToList() ?? []
        };
    }
}