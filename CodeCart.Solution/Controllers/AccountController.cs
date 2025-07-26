using AutoMapper;
using CodeCart.API.DTOs.AccountDtos;
using CodeCart.API.Errors;
using CodeCart.Core.Entities;
using CodeCart.Core.Entities.Identity;
using CodeCart.Core.Services.Contracts;
using CodeCart.Core.Services.Contracts.SecurityModule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CodeCart.API.Controllers;

public class AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager,
    IAuthService authService, ITokenBlacklistService tokenBlacklistService,
    IMapper mapper, IConfiguration configuration,
        IMailingService mailService) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterDto registerDto)
    {
        var user = new AppUser
        {
            Email = registerDto.Email,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            UserName = registerDto.Email.Split('@')[0]
        };
        var result = await userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
            return BadRequest(new ApiResponse(400));

        var token = await authService.CreateAccessTokenAsync(user, userManager);

        return Ok(new UserDto
        {
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Token = token
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto model)
    {
        if (ModelState.IsValid)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user is null)
                return Unauthorized(new ApiResponse(401));

            var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return Unauthorized(new ApiResponse(401));

            var token = await authService.CreateAccessTokenAsync(user, userManager);

            return Ok(new UserDto
            {
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Token = token
            });
        }
        return Unauthorized(new ApiResponse(401));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userId))
            return BadRequest(new ApiResponse(400, "Invalid token"));

        await tokenBlacklistService.BlacklistTokenAsync(token);

        return Ok(new { message = "Logged out successfully" });
    }

    [Authorize]
    [HttpGet("get-current-user")]
    public async Task<ActionResult> GetCurrentUser()
    {
        var user = await FindUserWithAddressAsync(userManager, User);
        if (user == null) return Unauthorized(new ApiResponse(401));

        var token = await authService.CreateAccessTokenAsync(user, userManager);

        return Ok(new 
        {
            UserName = user?.UserName ?? string.Empty,
            Email = user?.Email ?? string.Empty,
            Address = user?.Address is not null ? mapper.Map<AddressDto>(user.Address) : null!,
            Roles = User.FindFirstValue(ClaimTypes.Role),
            Token = token
        });
    }


 

    [Authorize]
    [HttpGet("address")]
    public async Task<ActionResult<AddressDto>> GetUserAddress()
    {
        var user = await FindUserWithAddressAsync(userManager, User);

        if (user.Address == null)
            return NotFound(new ApiResponse(404, "The user has no address"));

        var address = mapper.Map<AddressDto>(user.Address);

        return Ok(address);
    }

    [Authorize]
    [HttpPost("address")]
    public async Task<ActionResult<AddressDto>> CreateOrUpdateUserAddress(AddressDto addressDto)
    {
        var user = await FindUserWithAddressAsync(userManager, User);

        if (user == null)
            return NotFound(new ApiResponse(404, "User not found"));

        if (user.Address == null)
        {
            // Map new address from DTO
            user.Address = mapper.Map<Address>(addressDto);
        }
        else
        {
            // Update existing address
            mapper.Map(addressDto, user.Address);
        }

        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
            return BadRequest(new ApiResponse(400, "Failed to update user"));

        // Map back the Address to AddressDto to ensure it's in sync (e.g., if Address has ID or formatted fields)
        var returnDto = mapper.Map<AddressDto>(user.Address);

        return Ok(returnDto);
    }

    private async Task<AppUser> FindUserWithAddressAsync(UserManager<AppUser> userManager, ClaimsPrincipal User)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);

        var user = await userManager.Users.Include(U => U.Address).SingleOrDefaultAsync(U => U.Email == email);

        return user!;
    }
}