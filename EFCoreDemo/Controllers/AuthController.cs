using EFCoreDemo.Contracts;
using EFCoreDemo.Data;
using EFCoreDemo.Domain.Entities;
using EFCoreDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace EFCoreDemo.Controllers;
[ApiController][Route("api/auth")]
public class AuthController(AppDbContext db,TokenService tokens):ControllerBase
{
    [AllowAnonymous][HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var user=await db.Users.SingleOrDefaultAsync(x=>x.Username==request.Username&&x.IsActive);if(user is null)return Unauthorized();
        if(new PasswordHasher<User>().VerifyHashedPassword(user,user.PasswordHash,request.Password)==PasswordVerificationResult.Failed)return Unauthorized();
        var t=tokens.Create(user);return new LoginResponse(t.Token,t.ExpiresAtUtc,user.FullName,user.Role);
    }
    [Authorize(Roles="Admin")][HttpPost("users")]
    public async Task<ActionResult> CreateUser(CreateUserRequest request)
    {
        if(await db.Users.AnyAsync(x=>x.Username==request.Username))return Conflict("Username already exists.");
        var u=new User{Username=request.Username.Trim(),FullName=request.FullName.Trim(),Role=request.Role};u.PasswordHash=new PasswordHasher<User>().HashPassword(u,request.Password);
        db.Users.Add(u);await db.SaveChangesAsync();return Created($"/api/auth/users/{u.Id}",new{u.Id,u.Username,u.FullName,u.Role});
    }
}