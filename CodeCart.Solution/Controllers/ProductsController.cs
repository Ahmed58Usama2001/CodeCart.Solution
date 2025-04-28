using AutoMapper;
using CodeCart.API.DTOs;
using CodeCart.Core.Entities;
using CodeCart.Core.Repositories.Contract;
using Microsoft.AspNetCore.Mvc;

namespace CodeCart.API.Controllers;

public class ProductsController : BaseApiController
{
    private readonly IGenericRepository<Product> _productRepository;
    private readonly IMapper _mapper;

    public ProductsController(IGenericRepository<Product> productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProducts()
    {
        var products = await _productRepository.GetAllAsync();
        return Ok(_mapper.Map<IReadOnlyList<ProductToReturnDto>>(products));
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
}