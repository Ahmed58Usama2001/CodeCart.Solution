using CodeCart.API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeCart.API.Controllers
{

    public class BuggyController : BaseApiController
    {

        [HttpGet("not-found")]
        public IActionResult GetNotFound()
        {
            return NotFound();
        }


        [HttpGet("bad-request")]
        public IActionResult GetBadRequest()
        {
            return BadRequest("This is a bad request");
        }


        [HttpGet("unauthorized")]
        public IActionResult GetUnauthorized()
        {
            return Unauthorized();
        }

        [HttpGet("server-error")]
        public IActionResult GetServerError()
        {
            throw new Exception("This is a server error");
        }

        [HttpPost("validation-error")]
        public IActionResult GetValidationError(CreateProductDto product)
        {
            return BadRequest("Please provide required values");
        }
    }
}
