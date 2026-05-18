using Market.Application.Models.Requests;
using Market.Application.Models.Responses;
using Market.Application.Repositories;
using Market.Application.Services.Abstractions;
using Market.Domain.Entities;
using FluentValidation;

namespace Market.Application.Services;

public class ProductsService(IProductsRepository repository, IValidator<ProductCreateDto> createValidator, IValidator<ProductUpdateDto> updateValidator, IValidator<PagedRequestDto> pagedValidator) : IProductsService
{
	public async Task<IEnumerable<ProductListDto>> GetAll(Guid? categoryId, CancellationToken cancellationToken)
	{
		var products = await repository.GetAll(categoryId, cancellationToken);

		return products.Select(x => new ProductListDto
		{
			Id = x.Id,
			Name = x.Name,
			Category = new ProductListCategoryDto
			{
				Id = x.CategoryId,
				Name = x.Category!.Name
			},
			Amount = x.Amount,
			Price = x.Price,
			BrandId = x.BrandId,
    		BrandName = x.Brand?.Name 
		});
	}
	
	public async Task<ProductListDto> Get(long id, CancellationToken cancellationToken)
	{
		var product = await repository.Get(id, cancellationToken);

		if (product is null)
		{
			throw new ArgumentNullException(nameof(product));
		}

		return new ProductListDto
		{
			Id = product.Id,
			Name = product.Name,
			Category = new ProductListCategoryDto
			{
				Id = product.CategoryId,
				Name = product.Category!.Name
			},
			Amount = product.Amount,
			Price = product.Price,
			BrandId = product.BrandId,
    		BrandName = product.Brand?.Name 
		};
	}

	public async Task Create(ProductCreateDto request, CancellationToken cancellationToken)
	{
		await createValidator.ValidateAndThrowAsync(request, cancellationToken);
		var product = new Product
		{
			Name = request.Name,
			CategoryId = request.CategoryId,
			Amount = request.Amount!.Value,
			Price = request.Price!.Value,
			IsDeleted = false,
			BrandId = request.BrandId
		};
		
		await repository.Add(product, cancellationToken);
	}
	
	public async Task Update(long id, ProductUpdateDto request, CancellationToken cancellationToken)
	{
		await updateValidator.ValidateAndThrowAsync(request, cancellationToken);
		var product = await repository.Get(id, cancellationToken);
		
		if (product is null)
		{
			throw new ArgumentNullException(nameof(product));
		}
		
		product.Name = request.Name;
		product.CategoryId = request.CategoryId;
		product.Amount = request.Amount!.Value;
		product.Price = request.Price!.Value;
		
		await repository.Update(product, cancellationToken);
	}
	
	public async Task Delete(long id, CancellationToken cancellationToken)
	{
		var product = await repository.Get(id, cancellationToken);
		
		if (product is null)
		{
			throw new ArgumentNullException(nameof(product));
		}
		
		product.IsDeleted = true;
		
		await repository.Update(product, cancellationToken);
	}

	public async Task<PagedResultDto<ProductListDto>> GetPaged(
    Guid? categoryId, 
    PagedRequestDto pagedRequest, 
    CancellationToken cancellationToken)
	{
		await pagedValidator.ValidateAndThrowAsync(pagedRequest, cancellationToken);
    	
    	var (products, totalCount) = await repository.GetPaged(
        	categoryId, 
        	pagedRequest.PageNumber, 
        	pagedRequest.PageSize, 
        	cancellationToken);

    	
    	var dtos = products.Select(x => new ProductListDto
    	{
        	Id = x.Id,
        	Name = x.Name,
        	Price = x.Price,
        	Amount = x.Amount,
        	BrandName = x.Brand?.Name,
        	Category = new ProductListCategoryDto 
        	{ 
            	Id = x.CategoryId, 
            	Name = x.Category?.Name ?? "Без категорії"
        	}
    	});

    	return new PagedResultDto<ProductListDto>
    	{
        	Items = dtos,
        	TotalCount = totalCount,
        	PageNumber = pagedRequest.PageNumber,
        	PageSize = pagedRequest.PageSize
    	};
	}
}
