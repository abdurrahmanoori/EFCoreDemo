using System.Text;
using EFCoreDemo.Data;
using EFCoreDemo.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Pharmacy Management API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { Name = "Authorization", Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT", In = ParameterLocation.Header });
});
if (!builder.Environment.IsEnvironment("Testing"))
{
    var cs = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("DefaultConnection is not configured.");
    builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(cs));
}
var key=builder.Configuration["Jwt:Key"]??throw new InvalidOperationException("Jwt:Key is not configured.");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o=>o.TokenValidationParameters=new TokenValidationParameters
{
    ValidateIssuer=true,ValidateAudience=true,ValidateLifetime=true,ValidateIssuerSigningKey=true,
    ValidIssuer=builder.Configuration["Jwt:Issuer"],ValidAudience=builder.Configuration["Jwt:Audience"],
    IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),ClockSkew=TimeSpan.FromMinutes(1)
});
builder.Services.AddAuthorization();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<SeedService>();
builder.Services.AddCors(o=>o.AddPolicy("app",p=>p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
var app=builder.Build();
if(app.Environment.IsDevelopment()){app.UseSwagger();app.UseSwaggerUI();}
app.UseExceptionHandler();
app.UseCors("app");
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/health",()=>Results.Ok(new{status="healthy",utc=DateTime.UtcNow})).AllowAnonymous();
app.MapControllers();
using(var scope=app.Services.CreateScope())
{
    var db=scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();
    await scope.ServiceProvider.GetRequiredService<SeedService>().SeedAsync();
}
app.Run();
public partial class Program { }