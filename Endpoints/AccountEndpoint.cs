using System;
using API.Common;
using API.DTOs;
using API.Extenions;
using API.Models;
using API.services;
using API.utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Endpoints;

public static class AccountEndpoint
{
    public static RouteGroupBuilder MapAccountEndpoint(this WebApplication app)
    {
        var group = app.MapGroup("/api/account").WithTags("account");
        group.MapPost("/register", async (HttpContext httpContext, UserManager<AppUser> userMAnager, [FromForm] string fullName, [FromForm] string email, [FromForm] string password, [FromForm] string userName, [FromForm] IFormFile? profileImage,[FromServices] IPhotoService photoService) =>
        {
            var userFromDb = await userMAnager.FindByEmailAsync(email);
            if (userFromDb is not null)
            {
                return Results.BadRequest(Response<string>.Failure("Email is already registered"));
            }
            if (profileImage is null)
            {
                return Results.BadRequest(Response<string>.Failure("Profile image is required"));
            }
            PhotoUploadResult uploadResult;
            try
            {
                uploadResult = await photoService.UploadImageAsync(profileImage);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(Response<string>.Failure("Failed to upload image: " + ex.Message));
            }
            
            var user = new AppUser
            {
                FullName = fullName,
                Email = email,
                UserName = userName,
                ProfileImage = uploadResult.Url,
            };
            var result = await userMAnager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                await photoService.DeleteImageAsync(uploadResult.PublicId);
                return Results.BadRequest(Response<string>.Failure(result.Errors.Select(x => x.Description).FirstOrDefault()!));
            }
            return Results.Ok(Response<string>.Success("User registered successfully", ""));
        }).DisableAntiforgery();

        group.MapPost("/login", async (UserManager<AppUser> userManager, TokenService tokenService, LoginDto dto) =>
        {
            if (dto is null)
            {
                return Results.BadRequest(Response<string>.Failure("Invalid login details"));
            }

            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user is null)
            {
                return Results.BadRequest(Response<string>.Failure("Invalid email or password"));
            }
            var result = await userManager.CheckPasswordAsync(user!, dto.Password);
            if (!result)
            {
                return Results.BadRequest(Response<string>.Failure("Invalid email or password"));
            }
            var token = tokenService.GenerateToken(user.Id, user.UserName!);
            return Results.Ok(Response<string>.Success("Login successful", token));
        });

        group.MapGet("/me", async (HttpContext context, UserManager<AppUser> userManager) =>
        {
            var currentLoggedInUserId = context.User.GetUserId()!;
            var currentLoggedInUser = await userManager.Users.SingleOrDefaultAsync(x => x.Id == currentLoggedInUserId.ToString());
            return Results.Ok(Response<AppUser>.Success("User fetched successfully.", currentLoggedInUser!));
            
        }).RequireAuthorization();
        return group;
    }
}               
