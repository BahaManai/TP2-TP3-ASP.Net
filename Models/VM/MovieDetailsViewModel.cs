namespace ASPCoreFirstApp.Models.VM;

public class MovieDetailsViewModel
{
    public Movie Movie { get; set; } = new();
    public List<MovieReview> Reviews { get; set; } = new();
    public double AverageRating { get; set; }
}
