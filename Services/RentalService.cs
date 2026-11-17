using ASPCoreFirstApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ASPCoreFirstApp.Services;

public class RentalService
{
    private readonly ApplicationDbContext _catalogDb;
    private readonly RentalDbContext _rentalDb;

    public RentalService(
        ApplicationDbContext catalogDb,
        RentalDbContext rentalDb)
    {
        _catalogDb = catalogDb;
        _rentalDb = rentalDb;
    }

    public async Task<(bool Success, string? Error)> RentAsync(int movieId, int customerId)
    {
        var movie = await _catalogDb.Movies.FirstOrDefaultAsync(item => item.Id == movieId);
        if (movie is null) return (false, "Le film demandé n'existe pas.");
        if (!movie.IsAvailable) return (false, "Ce film est actuellement loué.");

        var customerExists = await _rentalDb.Customers.AnyAsync(customer => customer.Id == customerId);
        if (!customerExists) return (false, "Le client sélectionné n'existe pas.");

        movie.IsAvailable = false;
        await _catalogDb.SaveChangesAsync();

        try
        {
            _rentalDb.Rentals.Add(new Rental
            {
                MovieId = movieId,
                CustomerId = customerId,
                RentedAt = DateTime.UtcNow
            });
            await _rentalDb.SaveChangesAsync();
            return (true, null);
        }
        catch (DbUpdateException)
        {
            movie.IsAvailable = true;
            await _catalogDb.SaveChangesAsync();
            return (false, "La location a échoué. Les modifications ont été annulées.");
        }
    }
}
