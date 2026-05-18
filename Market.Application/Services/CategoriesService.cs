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
        
        var allCategories = await categoriesRepository.GetAll(cancellationToken);
        
        
        var rootCategories = allCategories.Where(c => c.ParentId == null);
        
        
        return rootCategories.Select(MapToDto);
    }

    public async Task<CategoryListDto?> GetById(Guid id, CancellationToken cancellationToken)
    {
        
        var category = await categoriesRepository.Get(id, cancellationToken);

        if (category is null)
        {
            return null; 
        }

        
        return MapToDto(category);
    }
    
    public async Task Create(CategoryCreateDto request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var category = new Category
        {
            Name = request.Name,
            
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
        
        if (await productsRepository.IsProductInCategory(id, cancellationToken))
        {
            throw new InvalidOperationException($"Cannot delete category with id {id} because it contains products.");
        }
        
        
        if (await categoriesRepository.HasSubCategories(id, cancellationToken))
        {
            throw new InvalidOperationException($"Cannot delete category with id {id} because it contains subcategories.");
        }
        
        
        var category = await categoriesRepository.Get(id, cancellationToken);
        
        if (category is null)
        {
            throw new ArgumentNullException(nameof(category));
        }
        
        await categoriesRepository.Delete(category, cancellationToken);
    }
    
    
    private CategoryListDto MapToDto(Category category)
    {
        return new CategoryListDto
        {
            Id = category.Id,
            Name = category.Name,
            Categories = category.SubCategories?.Select(MapToDto).ToList() ?? []
        };
    }
}