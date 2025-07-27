using AutoMapper;
using CodeCart.API.DTOs;
using CodeCart.API.Helpers;
using CodeCart.Core.Entities;
using CodeCart.Core.Repositories.Contracts;
using CodeCart.Core.Specifications.ProductSpecs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeCart.API.Controllers;

public class ProductsController(IUnitOfWork unitOfWork , IMapper mapper, IProductRepository productRepo) : BaseApiController
{
    [Cache(600)]
    [HttpGet]
    public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts(
       [FromQuery] ProductSpecificationsParams specParams)
    {
        var specs = new ProductSpecifications(specParams);
        var count = await unitOfWork.Repository<Product>().GetCountAsync(new ProductsForCountSpecifications(specParams));

        var parameters = new PagedResultParameters<Product, ProductToReturnDto>
        {
            Repository = unitOfWork.Repository<Product>(),
            Specification = specs,
            PageIndex = specParams.PageIndex,
            PageSize = specParams.PageSize,
            Count = count,
            Mapper = mapper
        };

        return await CreatePagedResult(parameters);
    }

    [Cache(600)]
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
    {
        var product = await unitOfWork.Repository<Product>().GetByIdAsync(id);

        if (product == null) return NotFound();

        return mapper.Map<ProductToReturnDto>(product);
    }

    [InvalidateCache("api/products|")]
    [Authorize(Roles ="Admin")]
    [HttpPost]
    public async Task<ActionResult<ProductToReturnDto>> CreateProduct(CreateProductDto productDto)
    {
        var product = mapper.Map<Product>(productDto);

        await unitOfWork.CompleteAsync();
        var saved = await unitOfWork.CompleteAsync();

        if (saved>0) return BadRequest("Problem creating product");

        return CreatedAtAction(
            nameof(GetProduct),
            new { id = product.Id },
            mapper.Map<ProductToReturnDto>(product));
    }

    [InvalidateCache("api/products|")]
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<ProductToReturnDto>> UpdateProduct(int id, UpdateProductDto productDto)
    {
        var product = await unitOfWork.Repository<Product>().GetByIdAsync(id);

        if (product == null) return NotFound();

        mapper.Map(productDto, product);
        unitOfWork.Repository<Product>().Update(product);

        var saved = await unitOfWork.CompleteAsync();

        if (saved>0) return BadRequest("Problem updating product");

        return Ok(mapper.Map<ProductToReturnDto>(product));
    }

    [InvalidateCache("api/products|")]
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await unitOfWork.Repository<Product>().GetByIdAsync(id);

        if (product == null) return NotFound();

        unitOfWork.Repository<Product>().Delete(product);
        var saved = await unitOfWork.CompleteAsync();

        if (saved > 0) return BadRequest("Problem deleting product");

        return NoContent();
    }

    [Cache(10000)]
    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetProductBrands()
    {
        return Ok(await productRepo.GetProductBrandsAsync());
    }

    [Cache(10000)]
    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetProductTypes()
    {
        return Ok(await productRepo.GetProductTypesAsync());
    }
}