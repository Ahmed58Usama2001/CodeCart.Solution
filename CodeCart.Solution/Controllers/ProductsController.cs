using CodeCart.Core.Entities;
using CodeCart.Core.Repositories.Contract;
using Microsoft.AspNetCore.Mvc;

namespace CodeCart.API.Controllers;

public class ProductsController : BaseApiController
{
    private readonly IGenericRepository<Product> _productRepository;

    public ProductsController(IGenericRepository<Product> productRepository)
    {
        _productRepository = productRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts()
    {
        var products = await _productRepository.GetAllAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        await _productRepository.CreateAsync(product);
        var saved = await _productRepository.SaveAllAsync();

        if (!saved)
        {
            return BadRequest("Problem creating product");
        }

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateProduct(int id, Product product)
    {
        if (id != product.Id)
        {
            return BadRequest("Ids don't match");
        }

        if (!_productRepository.Exists(id))
        {
            return NotFound();
        }

        _productRepository.Update(product);
        var saved = await _productRepository.SaveAllAsync();

        if (!saved)
        {
            return BadRequest("Problem updating product");
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        _productRepository.Delete(product);
        var saved = await _productRepository.SaveAllAsync();

        if (!saved)
        {
            return BadRequest("Problem deleting product");
        }

        return NoContent();
    }
}