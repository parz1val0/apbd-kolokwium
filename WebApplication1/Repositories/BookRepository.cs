using System.Data.SqlClient;
using WebApplication1.Models;

namespace WebApplication1.Repositories;

public class BookRepository : IBookRepository
{
    private readonly IConfiguration _configuration;

    public BookRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> DoesBookExist(int id)
    {
        var query = "SELECT 1 FROM Books WHERE PK = @ID";
        await using SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();
        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }





    public async Task<BookWithGenres> GetBook(int id)
    {
        var query = @"SELECT books.PK , books.title, [genres].name
                        FROM books
                        JOIN books_genres ON books_genres.FK_book = books.PK
                        JOIN [genres] ON [genres].PK = books_genres.FK_genre where books.PK=@PK";

        await using SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@PK", id);

        await connection.OpenAsync();

        var reader = await command.ExecuteReaderAsync();

        var idBookOrdinal = reader.GetOrdinal("PK");
        var titleBookOrdinal = reader.GetOrdinal("title");
        var titleGenreOrdinal = reader.GetOrdinal("name");


        BookWithGenres bookWithGenres = null;

        while (await reader.ReadAsync())
        {
            if (bookWithGenres is not null)
            {
                bookWithGenres.Genres.Add(new Genres()
                {
                    name = reader.GetString(titleGenreOrdinal)
                });
            }
            else
            {
                bookWithGenres = new BookWithGenres()
                {
                    PK = reader.GetInt32(idBookOrdinal),
                    title = reader.GetString(titleBookOrdinal),
                    Genres = new List<Genres>()
                    {
                        new Genres()
                        {
                            name = reader.GetString(titleGenreOrdinal)

                        }
                    }
                };
            }
        }

        if (bookWithGenres is null) throw new Exception();

        return bookWithGenres;
    }

    public async Task AddNewBookWithGenres(NewBookWithGenres newBookWithGenres)
    {
        var insert = @"INSERT INTO books VALUES(@Title);";

        await using SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = insert;

        command.Parameters.AddWithValue("@Title", newBookWithGenres.Title);

        await connection.OpenAsync();

        var transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;

        try
        {
            var id = await command.ExecuteScalarAsync();

            foreach (var genres in newBookWithGenres.Genres)
            {
                command.Parameters.Clear();
                command.CommandText = "INSERT INTO books_genres VALUES((Select max(Pk) from books), @genresid);";
                command.Parameters.AddWithValue("@genresid", genres.PK);


                await command.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}

