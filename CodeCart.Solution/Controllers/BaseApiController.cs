using AutoMapper;
using CodeCart.API.DTOs;
using CodeCart.API.Helpers;
using CodeCart.Core.Entities;
using CodeCart.Core.Repositories.Contract;
using CodeCart.Core.Specifications;
using CodeCart.Core.Specifications.ProductSpecs;
using Microsoft.AspNetCore.Mvc;

namespace CodeCart.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BaseApiController : ControllerBase
{
    protected async Task<ActionResult<Pagination<TDto>>> CreatePagedResult<T, TDto>(
      PagedResultParameters<T, TDto> parameters)
      where T : BaseEntity
    {
        var items = await parameters.Repository.GetAllWithSpecAsync(parameters.Specification);
        var data = parameters.Mapper.Map<IReadOnlyList<TDto>>(items);

        return Ok(new Pagination<TDto>(
            parameters.PageIndex,
            parameters.PageSize,
            parameters.Count,
            data));
    }
}
