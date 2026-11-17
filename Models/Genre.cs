namespace ASPCoreFirstApp.Models;

public class Genre
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;
     public ICollection<Movie> Movies { get; set; } = new List<Movie>();
}
