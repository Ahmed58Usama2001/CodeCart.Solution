using AutoMapper;
using CodeCart.API.Helpers;
using CodeCart.Core.Entities;
using CodeCart.Core.Repositories.Contract;
using CodeCart.Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace CodeCart.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BaseApiController : ControllerBase
{
    protected async Task<ActionResult<Pagination<TDto>>> CreatePagedResult<T, TDto>(
        IGenericRepository<T> repository,
        ISpecification<T> specification,
        int pageIndex,
        int pageSize,
        IMapper mapper)
        where T : BaseEntity
    {
        var items = await repository.GetAllWithSpecAsync(specification);

        var count = await repository.GetCountAsync(specification);

        var data = mapper.Map<IReadOnlyList<TDto>>(items);

        return Ok(new Pagination<TDto>(pageIndex, pageSize, count, data));
    }
}
