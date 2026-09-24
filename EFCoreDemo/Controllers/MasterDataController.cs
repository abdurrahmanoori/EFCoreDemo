using EFCoreDemo.Data;
using EFCoreDemo.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFCoreDemo.Controllers;

[Authorize]
[ApiController]
[Route("api/master-data")]
public class MasterDataController(AppDbContext db) : ControllerBase
{
    [HttpGet("suppliers")]
    public async Task<ActionResult> Suppliers() =>
        Ok(await db.Suppliers.AsNoTracking().OrderBy(x => x.Name).ToListAsync());

    [Authorize(Roles = "Admin,Pharmacist")]
    [HttpPost("suppliers")]
    public async Task<ActionResult> CreateSupplier(Supplier request)
    {
        request.Id = 0;
        request.Name = request.Name.Trim();
        db.Suppliers.Add(request);
        await db.SaveChangesAsync();
        return Created($"/api/master-data/suppliers/{request.Id}", request);
    }

    [HttpGet("patients")]
    public async Task<ActionResult> Patients() =>
        Ok(await db.Patients.AsNoTracking().OrderBy(x => x.FullName).ToListAsync());

    [HttpPost("patients")]
    public async Task<ActionResult> CreatePatient(Patient request)
    {
        request.Id = 0;
        request.FullName = request.FullName.Trim();
        db.Patients.Add(request);
        await db.SaveChangesAsync();
        return Created($"/api/master-data/patients/{request.Id}", request);
    }

    [HttpGet("doctors")]
    public async Task<ActionResult> Doctors() =>
        Ok(await db.Doctors.AsNoTracking().OrderBy(x => x.FullName).ToListAsync());

    [Authorize(Roles = "Admin,Pharmacist,Doctor")]
    [HttpPost("doctors")]
    public async Task<ActionResult> CreateDoctor(Doctor request)
    {
        request.Id = 0;
        request.FullName = request.FullName.Trim();
        db.Doctors.Add(request);
        await db.SaveChangesAsync();
        return Created($"/api/master-data/doctors/{request.Id}", request);
    }

    [Authorize(Roles = "Admin,Pharmacist")]
    [HttpPost("categories")]
    public async Task<ActionResult> CreateCategory(Category request)
    {
        request.Id = 0;
        request.Name = request.Name.Trim();
        db.Categories.Add(request);
        await db.SaveChangesAsync();
        return Created($"/api/master-data/categories/{request.Id}", request);
    }

    [Authorize(Roles = "Admin,Pharmacist")]
    [HttpPost("units")]
    public async Task<ActionResult> CreateUnit(UnitOfMeasure request)
    {
        request.Id = 0;
        request.Name = request.Name.Trim();
        request.Abbreviation = request.Abbreviation.Trim();
        db.UnitsOfMeasure.Add(request);
        await db.SaveChangesAsync();
        return Created($"/api/master-data/units/{request.Id}", request);
    }

    [Authorize(Roles = "Admin,Pharmacist")]
    [HttpPost("manufacturers")]
    public async Task<ActionResult> CreateManufacturer(Manufacturer request)
    {
        request.Id = 0;
        request.Name = request.Name.Trim();
        db.Manufacturers.Add(request);
        await db.SaveChangesAsync();
        return Created($"/api/master-data/manufacturers/{request.Id}", request);
    }
}