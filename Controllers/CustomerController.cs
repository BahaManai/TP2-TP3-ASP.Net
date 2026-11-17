using ASPCoreFirstApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ASPCoreFirstApp.Controllers;

public class CustomerController : Controller
{
    private readonly RentalDbContext _db;
    private readonly ILogger<CustomerController> _logger;

    public CustomerController(RentalDbContext db, ILogger<CustomerController> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var includeQuery = _db.Customers
            .AsNoTracking()
            .Include(customer => customer.MembershipType)
            .OrderBy(customer => customer.Name);

        var projectionQuery = _db.Customers
            .AsNoTracking()
            .OrderBy(customer => customer.Name)
            .Select(customer => new
            {
                customer.Name,
                customer.Email,
                MembershipType = customer.MembershipType!.Name,
                customer.MembershipType!.DiscountPercent
            });

        _logger.LogInformation("SQL avec Include:\n{Sql}", includeQuery.ToQueryString());
        _logger.LogInformation("SQL avec projection Select:\n{Sql}", projectionQuery.ToQueryString());

        ViewBag.IncludeSql = includeQuery.ToQueryString();
        ViewBag.ProjectionSql = projectionQuery.ToQueryString();
        return View(await includeQuery.ToListAsync());
    }

    public async Task<IActionResult> Create()
    {
        await LoadMembershipsAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Email,MembershipTypeId")] Customer customer)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Errors = ModelState.Values
                .SelectMany(value => value.Errors)
                .Select(error => error.ErrorMessage)
                .Where(message => !string.IsNullOrWhiteSpace(message))
                .ToList();
            await LoadMembershipsAsync(customer.MembershipTypeId);
            return View(customer);
        }

        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadMembershipsAsync(int? selectedId = null)
    {
        ViewBag.Memberships = new SelectList(
            await _db.MembershipTypes
                .AsNoTracking()
                .OrderBy(membership => membership.Name)
                .ToListAsync(),
            "Id",
            "Name",
            selectedId);
    }
}
