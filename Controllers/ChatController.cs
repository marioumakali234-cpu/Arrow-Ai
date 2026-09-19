using AIProject.Models;
using AIProject.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AIProject.Controllers
{
    public class ChatController : Controller
    {
        private readonly IAiService _aiService;

        public ChatController(IAiService aiService)
        {
            _aiService = aiService;
        }

        public IActionResult Index()
        {
            var chatHistory = HttpContext.Session.GetString("ChatHistory");

            var messages = string.IsNullOrEmpty(chatHistory)
                ? new List<ChatMessage>()
                : JsonSerializer.Deserialize<List<ChatMessage>>(chatHistory)
                  ?? new List<ChatMessage>();

            return View(messages);
        }

        [HttpPost]
        public async Task<IActionResult> Send([FromBody] ChatRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new
                {
                    error = "Message cannot be empty."
                });
            }

            var chatHistory = HttpContext.Session.GetString("ChatHistory");

            var messages = string.IsNullOrEmpty(chatHistory)
                ? new List<ChatMessage>()
                : JsonSerializer.Deserialize<List<ChatMessage>>(chatHistory)
                  ?? new List<ChatMessage>();

            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new
                {
                    error = "Message is empty"
                });
            }
            messages.Add(new ChatMessage
            {
                Role = "user",
                Content = request.Message.Trim()
            });
            try
            {
                var aiResponse = await _aiService.GetResponseAsync(messages);

                messages.Add(new ChatMessage
                {
                    Role = "assistant",
                    Content = aiResponse
                });

                HttpContext.Session.SetString(
                    "ChatHistory",
                    JsonSerializer.Serialize(messages)
                );

                return Json(new
                {
                    response = aiResponse
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = ex.Message
                });
            }
        }

        public IActionResult NewChat()
        {
            HttpContext.Session.Remove("ChatHistory");

            return RedirectToAction("Index");
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; }
    }
}