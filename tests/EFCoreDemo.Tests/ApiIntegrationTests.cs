using System.Net;
using EFCoreDemo.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
namespace EFCoreDemo.Tests;
public class ApiIntegrationTests:IClassFixture<PharmacyFactory>
{
    readonly HttpClient client;public ApiIntegrationTests(PharmacyFactory f)=>client=f.CreateClient();
    [Fact] public async Task Health_is_ok()=>Assert.Equal(HttpStatusCode.OK,(await client.GetAsync("/health")).StatusCode);
    [Fact] public async Task Dashboard_requires_auth()=>Assert.Equal(HttpStatusCode.Unauthorized,(await client.GetAsync("/api/dashboard")).StatusCode);
}
public class PharmacyFactory:WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");builder.ConfigureServices(s=>{s.RemoveAll(typeof(DbContextOptions<AppDbContext>));s.RemoveAll(typeof(AppDbContext));s.AddDbContext<AppDbContext>(o=>o.UseInMemoryDatabase("integration-"+Guid.NewGuid()));});
    }
}