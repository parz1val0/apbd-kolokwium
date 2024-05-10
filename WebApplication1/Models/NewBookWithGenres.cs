namespace WebApplication1.Models;

public class NewBookWithGenres
{
    public string Title { get; set; } = string.Empty;
    public List<Genres> Genres { get; set; } = new List<Genres>();
}