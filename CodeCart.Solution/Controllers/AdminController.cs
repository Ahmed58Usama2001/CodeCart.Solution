using AutoMapper;
using CodeCart.API.DTOs;
using CodeCart.API.Helpers;
using CodeCart.Core.Entities.OrderAggregation;
using CodeCart.Core.Repositories.Contracts;
using CodeCart.Core.Specifications.OrderSpecs;
using CodeCart.Core.Specifications.ProductSpecs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeCart.API.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController(IUnitOfWork unitOfWork,IMapper mapper) : BaseApiController
{
    [HttpGet("orders")]
    public async Task<ActionResult<Pagination<OrderToReturnDto>>> GetOrders([FromQuery] OrderSpecParams specParams)
    {
        var spec = new OrderSpecifications(specParams);
        var count = await unitOfWork.Repository<Order>().GetCountAsync(new OrderForCountSpecifications(specParams));


        var parameters = new PagedResultParameters<Order, OrderToReturnDto>
        {
            Repository = unitOfWork.Repository<Order>(),
            Specification = spec,
            PageIndex = specParams.PageIndex,
            PageSize = specParams.PageSize,
            Mapper = mapper,
            Count = count
        };

        return await CreatePagedResult(parameters);
    }

    [HttpGet("orders/{id:int}")]
    public async Task<ActionResult<OrderToReturnDto>> GetOrder(int id)
    {
        var spec = new OrderSpecifications(id);
        var order = await unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);

        if (order == null) return BadRequest("No order with that ID");

        return Ok(mapper.Map<OrderToReturnDto>(order));
    }
}
