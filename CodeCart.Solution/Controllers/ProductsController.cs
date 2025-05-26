using AutoMapper;
using CodeCart.API.DTOs;
using CodeCart.API.Helpers;
using CodeCart.Core.Entities;
using CodeCart.Core.Repositories.Contracts;
using CodeCart.Core.Specifications.ProductSpecs;
using Microsoft.AspNetCore.Mvc;

namespace CodeCart.API.Controllers;

public class ProductsController : BaseApiController
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ProductsController(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts(
       [FromQuery] ProductSpecificationsParams specParams)
    {
        var specs = new ProductSpecifications(specParams);
        var count = await _productRepository.GetCountAsync(new ProductsForCountSpecifications(specParams));

        var parameters = new PagedResultParameters<Product, ProductToReturnDto>
        {
            Repository = _productRepository,
            Specification = specs,
            PageIndex = specParams.PageIndex,
            PageSize = specParams.PageSize,
            Count = count,
            Mapper = _mapper
        };

        return await CreatePagedResult(parameters);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null) return NotFound();

        return _mapper.Map<ProductToReturnDto>(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductToReturnDto>> CreateProduct(CreateProductDto productDto)
    {
        var product = _mapper.Map<Product>(productDto);

        await _productRepository.CreateAsync(product);
        var saved = await _productRepository.SaveAllAsync();

        if (!saved) return BadRequest("Problem creating product");

        return CreatedAtAction(
            nameof(GetProduct),
            new { id = product.Id },
            _mapper.Map<ProductToReturnDto>(product));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProductToReturnDto>> UpdateProduct(int id, UpdateProductDto productDto)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null) return NotFound();

        _mapper.Map(productDto, product);
        _productRepository.Update(product);

        var saved = await _productRepository.SaveAllAsync();

        if (!saved) return BadRequest("Problem updating product");

        return Ok(_mapper.Map<ProductToReturnDto>(product));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null) return NotFound();

        _productRepository.Delete(product);
        var saved = await _productRepository.SaveAllAsync();

        if (!saved) return BadRequest("Problem deleting product");

        return NoContent();
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetProductBrands()
    {
        return Ok(await _productRepository.GetProductBrandsAsync());
    }

    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetProductTypes()
    {
        return Ok(await _productRepository.GetProductTypesAsync());
    }
}