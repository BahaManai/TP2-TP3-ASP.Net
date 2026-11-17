using ASPCoreFirstApp.Models;
using MongoDB.Driver;

namespace ASPCoreFirstApp.Services;

public class MongoReviewService
{
    private readonly IMongoCollection<MovieReview> _reviews;

    public MongoReviewService(IConfiguration configuration)
    {
        var connectionString = configuration["MongoDb:ConnectionString"]
            ?? throw new InvalidOperationException("MongoDb:ConnectionString is not configured.");
        var databaseName = configuration["MongoDb:DatabaseName"]
            ?? throw new InvalidOperationException("MongoDb:DatabaseName is not configured.");

        _reviews = new MongoClient(connectionString)
            .GetDatabase(databaseName)
            .GetCollection<MovieReview>("movieReviews");
    }

    public Task<List<MovieReview>> GetByMovieAsync(int movieId)
        => _reviews.Find(review => review.MovieId == movieId).ToListAsync();

    public Task AddAsync(MovieReview review)
        => _reviews.InsertOneAsync(review);

    public async Task<double> GetAverageRatingAsync(int movieId)
    {
        var reviews = await GetByMovieAsync(movieId);
        if (reviews.Count == 0) return 0;
        return reviews.Average(review => review.Rating);
    }
}