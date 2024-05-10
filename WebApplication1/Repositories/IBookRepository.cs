using WebApplication1.Models;

namespace WebApplication1.Repositories;

public interface IBookRepository
{ 
    Task<bool> DoesBookExist(int id);
    Task<BookWithGenres> GetBook(int id);
    Task AddNewBookWithGenres(NewBookWithGenres newBookWithGenres);
}