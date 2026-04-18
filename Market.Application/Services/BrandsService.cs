using Market.Application.Models.Requests;
using Market.Application.Models.Responses;
using Market.Application.Repositories;
using Market.Application.Services.Abstractions;
using Market.Domain.Entities;
using FluentValidation;

namespace Market.Application.Services;

public class BrandsService(IBrandsRepository brandsRepository, IValidator<BrandCreateDto> createValidator, IValidator<BrandUpdateDto> updateValidator) : IBrandsService
{
    public async Task<IEnumerable<BrandDto>> GetAll(CancellationToken cancellationToken)
    {
        var brands = await brandsRepository.GetAll(cancellationToken);
        
        return brands.Select(b => new BrandDto
        {
            Id = b.Id,
            Name = b.Name,
            Description = b.Description
        });
    }

    public async Task Create(BrandCreateDto request, CancellationToken cancellationToken)
    {
        await createValidator.ValidateAndThrowAsync(request, cancellationToken);
        var brand = new Brand
        {
            Name = request.Name,
            Description = request.Description
        };

        await brandsRepository.Add(brand, cancellationToken);
    }

    // Додай ці три методи всередину BrandsService
    public async Task<BrandDto?> GetById(Guid id, CancellationToken cancellationToken)
    {
        var brand = await brandsRepository.Get(id, cancellationToken);
        
        if (brand is null) return null;

        return new BrandDto
        {
            Id = brand.Id,
            Name = brand.Name,
            Description = brand.Description
        };
    }

    public async Task Update(Guid id, BrandUpdateDto request, CancellationToken cancellationToken)
    {
        await updateValidator.ValidateAndThrowAsync(request, cancellationToken);
        var brand = await brandsRepository.Get(id, cancellationToken);

        if (brand is null)
        {
            throw new ArgumentNullException(nameof(brand));
        }

        brand.Name = request.Name;
        brand.Description = request.Description;

        await brandsRepository.Update(brand, cancellationToken);
    }

    public async Task Delete(Guid id, CancellationToken cancellationToken)
    {
        // 🛡️ Наш надійний захист від зламаних зв'язків
        if (await brandsRepository.HasProducts(id, cancellationToken))
        {
            throw new InvalidOperationException($"Нееможливо видалити бренд {id}, оскільки існують прив'язані до нього товари.");
        }

        var brand = await brandsRepository.Get(id, cancellationToken);

        if (brand is null)
        {
            throw new ArgumentNullException(nameof(brand));
        }

        await brandsRepository.Delete(brand, cancellationToken);
    }
}