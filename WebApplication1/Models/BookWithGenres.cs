namespace WebApplication1.Models;

public class BookWithGenres
{
    public int PK { get; set; }
    public string title { get; set; }
    public List<Genres> Genres { get; set; } = new List<Genres>();
}

public class Genres
{
    public string name { get; set; } = string.Empty;
    public int PK { get; set; } 
}