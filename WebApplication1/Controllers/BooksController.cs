using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Controllers;

[Route($"api/[controller]")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly IBookRepository _bookRepository;
    public BooksController(IBookRepository bookRepository)
    { 
        _bookRepository = bookRepository;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBook(int id)
    {
        if (!await _bookRepository.DoesBookExist(id))
            return NotFound($"Book with given ID - {id} doesn't exist");

        var book = await _bookRepository.GetBook(id);

        return Ok(book);
    }
    [HttpPost]
    public async Task<IActionResult> AddBook(NewBookWithGenres newBookWithAuthors)
    {
        await _bookRepository.AddNewBookWithGenres(newBookWithAuthors);

        return Created(Request.Path.Value ?? "api/books", newBookWithAuthors);
    }
}