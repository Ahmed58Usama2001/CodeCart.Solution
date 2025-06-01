﻿using AutoMapper;
using CodeCart.API.DTOs.AccountDtos;
using CodeCart.API.Errors;
using CodeCart.Core.Entities;
using CodeCart.Core.Entities.Identity;
using CodeCart.Core.Entities.Identity.Gmail;
using CodeCart.Core.Services.Contracts;
using CodeCart.Core.Services.Contracts.SecurityModule;
using CodeCart.Service.SecurityModule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;

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

        var tokens = await authService.CreateTokensAsync(user, userManager);

        return Ok(new UserDto
        {
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Token = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken
        });
    }


    [HttpPost("GoogleSignIn")]
    public async Task<ActionResult<UserDto>> GoogleSignIn(GoogleSignInVM model)
    {
        try
        {
            var result = await authService.SignInWithGoogle(model);
            if (result != null)
            {
                var tokens = await authService.CreateTokensAsync(result, userManager);

                UserDto userDto = new UserDto()
                {
                    UserName = result.UserName ?? string.Empty,
                    Email = result.Email ?? string.Empty,
                    Token = tokens.AccessToken,
                    RefreshToken = tokens.RefreshToken
                };

                return Ok(userDto);
            }
            else
                return BadRequest(new ApiResponse(400));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse(400));
        }
    }

    [HttpPost("FacebookSignIn")]
    public async Task<ActionResult<UserDto>> FacebookSignIn(FacebookSignInVM model)
    {
        try
        {
            var result = await authService.SignInWithFacebook(model);
            if (result != null)
            {
                var tokens = await authService.CreateTokensAsync(result, userManager);

                UserDto userDto = new UserDto()
                {
                    UserName = result.UserName ?? string.Empty,
                    Email = result.Email ?? string.Empty,
                    Token = tokens.AccessToken,
                    RefreshToken = tokens.RefreshToken
                };

                return Ok(userDto);
            }
            else
                return BadRequest(new ApiResponse(400));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse(400));
        }
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

            var tokens = await authService.CreateTokensAsync(user, userManager);

            return Ok(new UserDto
            {
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Token = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken
            });
        }
        return Unauthorized(new ApiResponse(401));
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<UserDto>> RefreshToken(RefreshTokenDto refreshTokenDto)
    {
        var result = await authService.RefreshTokenAsync(refreshTokenDto.RefreshToken, userManager);

        if (!result.IsSuccess)
            return Unauthorized(new ApiResponse(401, result.ErrorMessage));

        return Ok(new UserDto
        {
            UserName = result.User.UserName ?? string.Empty,
            Email = result.User.Email ?? string.Empty,
            Token = result.AccessToken,
            RefreshToken = result.RefreshToken
        });
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
        await authService.RevokeUserRefreshTokensAsync(userId);

        return Ok(new { message = "Logged out successfully" });
    }

    [Authorize]
    [HttpGet("get-current-user")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var user = await FindUserWithAddressAsync(userManager, User);
        if (user == null) return Unauthorized(new ApiResponse(401));

        var tokens = await authService.CreateTokensAsync(user, userManager);

        return Ok(new UserDto()
        {
            UserName = user?.UserName ?? string.Empty,
            Email = user?.Email ?? string.Empty,
            Address= user?.Address is not null ? mapper.Map<AddressDto>(user.Address) : null!,
            Token = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken
        });
    }

    [HttpPost("forgetPassword")]
    public async Task<ActionResult<UserDto>> ForgetPassword(ForgetPasswordDto model)
    {
        if (ModelState.IsValid)
        {
            var user = await userManager.FindByEmailAsync(model.Email);


            if (user is not null)
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var resetPasswordLink = Url.Action("ResetPassword", "Account", new { Email = model.Email, Token = token }, "http", configuration["ClientBaseUrl"]);
                

                var bodyUrl = $"{Directory.GetCurrentDirectory()}\\wwwroot\\TempleteHtml\\ForgetPasswordTemplete.html";
                var body = new StreamReader(bodyUrl);
                var mailText = body.ReadToEnd();
                body.Close();

                mailText = mailText.Replace("[username]", user.UserName).Replace("[LinkHere]", resetPasswordLink);

                var result = await mailService.SendEmailAsync(model.Email, "Reset Password", mailText);
                if (result == false)
                    return BadRequest(new ApiResponse(400));


                return Ok(model);
            }
            return Unauthorized(new ApiResponse(401));
        }

        return Ok(model);
    }

    [HttpPost("ResetPassword")]
    public async Task<ActionResult<UserDto>> ResetPassword(ResetPasswordDto model)
    {
        if (ModelState.IsValid)
        {
            var user = await userManager.FindByEmailAsync(model.Email);


            if (user is not null)
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var result = await userManager.ResetPasswordAsync(user, token, model.Password);
                if (result.Succeeded)
                    return Ok(model);
                string errors = string.Join(", ", result.Errors.Select(error => error.Description));
                return BadRequest(new ApiResponse(400, errors));

            }
        }

        return Ok(model);
    }

    [Authorize]
    [HttpGet("address")]
    public async Task<ActionResult<AddressDto>> GetUserAddress()
    {

        var user = await FindUserWithAddressAsync(userManager,User);

        if (user.Address == null)
           return NotFound(new ApiResponse(404 , "The user has no address"));

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