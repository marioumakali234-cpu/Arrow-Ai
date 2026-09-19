using AIProject.Models;
using AIProject.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ArrowAI.Controllers
{
    public class AiTestController : Controller
    {
        private readonly IAiService _aiService;

        public AiTestController(IAiService aiService)
        {
            _aiService = aiService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string? message)
        {
            ViewBag.Message = message;

            if (string.IsNullOrWhiteSpace(message))
            {
                ViewBag.Error = "Please enter a message.";
                return View();
            }

            if (message.Length > 4000)
            {
                ViewBag.Error =
                    "Your message must not exceed 4,000 characters.";

                return View();
            }

            var messages = new List<ChatMessage>
            {
                new ChatMessage
                {
                    Role = "user",
                    Content = message.Trim()
                }
            };

            try
            {
                ViewBag.Reply =
                    await _aiService.GetResponseAsync(messages);
            }
            catch (InvalidOperationException ex)
            {
                ViewBag.Error = ex.Message;
            }
            catch (HttpRequestException)
            {
                ViewBag.Error =
                    "Unable to connect to OpenRouter. " +
                    "Check your internet connection and try again.";
            }
            catch (TaskCanceledException)
            {
                ViewBag.Error =
                    "The request timed out. Please try again.";
            }
            catch (JsonException)
            {
                ViewBag.Error =
                    "Unable to read the response from OpenRouter.";
            }

            return View();
        }
    }
}