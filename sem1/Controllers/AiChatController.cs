using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using sem1.Data;

namespace sem1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AiChatController : ControllerBase
    {
        private readonly IChatClient _chatClient;
        private readonly LibraryContext _db;

        public AiChatController(IChatClient chatClient, LibraryContext db)
        {
            _chatClient = chatClient;
            _db = db;
        }

        [HttpGet("pitch/{id}")]
        public async Task<IActionResult> GetBookPitch(int id)
        {
            var book = _db.Book.Find(id);
            if (book == null) return NotFound();

            string prompt = $"Scrie un pitch scurt și atractiv pentru cartea: {book.Title}.";

            // În v9.7.0 stabil, apelul standard pe IChatClient folosește CompleteAsync dar cu un array de ChatMessage sau metoda de extensie GetResponseAsync
            var response = await _chatClient.GetResponseAsync(prompt);

            return Ok(response.Text);
        }

        [HttpGet("ask")]
        public async Task<IActionResult> Ask([FromQuery] string q)
        {
            var response = await _chatClient.GetResponseAsync(q);

            return Ok(response.Text);
        }
    }
}