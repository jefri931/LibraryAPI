using AutoMapper;
using LibraryAPI.Database.Models;
using LibraryAPI.DTOs;
using LibraryAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "PublisherScheme")]
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : BaseController
    {
        private readonly IBookManagementService _bookManager;
        private readonly IMapper _mapper;
        public BookController(IBookManagementService bookManager, IMapper mapper) {
            _bookManager = bookManager;
            _mapper = mapper;
        }

        [HttpGet("AllBooks")]
        [AllowAnonymous]
        public async Task<IActionResult> AllBooks()
        {
            var books = await _bookManager.GetAll();
            return Ok(books.Select(_mapper.Map<BookInfoDTO>));
        }

        [HttpGet("PublishedBooks")]
        public async Task<IActionResult> PublishedBooks()
        {
            return Ok(await _bookManager.GetAll(UserID));
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] BookDTO book)
        {
            if(ModelState.IsValid) {
                var bookModel = _mapper.Map<Book>(book);
                bookModel.PublisherId = UserID;
                await _bookManager.Create(bookModel);
                return CreatedAtAction("Add", new { id = bookModel.Id });
            }

            return BadRequest(ModelState);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BookDTO updatedBook)
        {
            if(ModelState.IsValid) {
                var existingBook = await _bookManager.Get(id);
                if (existingBook == null)
                {
                    return NotFound();
                }

                if(!await _bookManager.IsBookPublisher(id, UserID)) {
                    return Unauthorized();
                }

                _mapper.Map(updatedBook, existingBook);
                await _bookManager.Update(existingBook);

                return NoContent();
            }

            return BadRequest(ModelState);
        }

        [HttpDelete("Remove/{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            var existingBook = await _bookManager.Get(id);
            if (existingBook == null)
            {
                return NotFound();
            }

            if(!await _bookManager.IsBookPublisher(id, UserID)) {
                return Unauthorized();
            }

            await _bookManager.Delete(existingBook);
            return NoContent();
        }
    }
}
