using ASPCoreFirstApp.Models;
using ASPCoreFirstApp.Models.VM;
using ASPCoreFirstApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ASPCoreFirstApp.Controllers;

public class MovieController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly RentalDbContext _rentalDb;
    private readonly RentalService _rentalService;
    private readonly MongoReviewService _reviewService;

    public MovieController(
        ApplicationDbContext db,
        RentalDbContext rentalDb,
        RentalService rentalService,
        MongoReviewService reviewService)
    {
        _db = db;
        _rentalDb = rentalDb;
        _rentalService = rentalService;
        _reviewService = reviewService;
    }

    public async Task<IActionResult> Index(string? sortOrder, int page = 1, int pageSize = 5)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 50);
        ViewBag.NameSortParam = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
        ViewBag.GenreSortParam = sortOrder == "genre" ? "genre_desc" : "genre";

        IQueryable<Movie> query = _db.Movies.AsNoTracking().Include(movie => movie.Genre);
        query = sortOrder switch
        {
            "name_desc" => query.OrderByDescending(movie => movie.Name),
            "genre" => query.OrderBy(movie => movie.Genre!.Name).ThenBy(movie => movie.Name),
            "genre_desc" => query.OrderByDescending(movie => movie.Genre!.Name).ThenBy(movie => movie.Name),
            _ => query.OrderBy(movie => movie.Name)
        };

        var totalCount = await query.CountAsync();
        var movies = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        ViewBag.SortOrder = sortOrder;
        return View(movies);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();
        var movie = await _db.Movies.AsNoTracking().Include(item => item.Genre)
            .FirstOrDefaultAsync(item => item.Id == id);
        if (movie is null) return NotFound();

        ViewBag.Customers = new SelectList(
            await _rentalDb.Customers.AsNoTracking().OrderBy(customer => customer.Name).ToListAsync(),
            "Id",
            "Name");
        ViewBag.RentError = TempData["RentError"];
        ViewBag.ReviewError = TempData["ReviewError"];

        var reviews = await _reviewService.GetByMovieAsync(movie.Id);
        var averageRating = await _reviewService.GetAverageRatingAsync(movie.Id);
        var viewModel = new MovieDetailsViewModel
        {
            Movie = movie,
            Reviews = reviews,
            AverageRating = averageRating
        };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddReview(
        int movieId,
        int rating,
        string comment,
        string? tags,
        string? language,
        bool spoiler)
    {
        if (rating < 1 || rating > 5)
        {
            TempData["ReviewError"] = "La note doit être comprise entre 1 et 5.";
            return RedirectToAction(nameof(Details), new { id = movieId });
        }

        if (string.IsNullOrWhiteSpace(comment))
        {
            TempData["ReviewError"] = "Le commentaire est obligatoire.";
            return RedirectToAction(nameof(Details), new { id = movieId });
        }

        await _reviewService.AddAsync(new MovieReview
        {
            MovieId = movieId,
            Rating = rating,
            Comment = comment.Trim(),
            Tags = string.IsNullOrWhiteSpace(tags)
                ? new List<string>()
                : tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(tag => tag.Trim())
                    .ToList(),
            Language = string.IsNullOrWhiteSpace(language) ? null : language.Trim(),
            Spoiler = spoiler,
            CreatedAt = DateTime.UtcNow
        });

        return RedirectToAction(nameof(Details), new { id = movieId });
    }

    public async Task<IActionResult> Create()
    {
        await LoadGenresAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,GenreId")] Movie movie)
    {
        if (!ModelState.IsValid)
        {
            await LoadGenresAsync(movie.GenreId);
            return View(movie);
        }

        movie.IsAvailable = true;
        _db.Movies.Add(movie);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Rent(int id, int customerId)
    {
        var result = await _rentalService.RentAsync(id, customerId);
        if (!result.Success)
        {
            TempData["RentError"] = result.Error;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();
        var movie = await _db.Movies.FindAsync(id);
        if (movie is null) return NotFound();
        await LoadGenresAsync(movie.GenreId);
        return View(movie);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,GenreId,IsAvailable")] Movie movie)
    {
        if (id != movie.Id) return NotFound();
        if (!ModelState.IsValid)
        {
            await LoadGenresAsync(movie.GenreId);
            return View(movie);
        }

        _db.Update(movie);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();
        var movie = await _db.Movies.AsNoTracking().Include(item => item.Genre)
            .FirstOrDefaultAsync(item => item.Id == id);
        return movie is null ? NotFound() : View(movie);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var movie = await _db.Movies.FindAsync(id);
        if (movie is not null)
        {
            _db.Movies.Remove(movie);
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadGenresAsync(Guid? selectedGenreId = null)
    {
        ViewBag.GenreId = new SelectList(
            await _db.Genres.AsNoTracking().OrderBy(genre => genre.Name).ToListAsync(),
            "Id", "Name", selectedGenreId);
    }
}
