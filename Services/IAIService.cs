using AIProject.Models;

namespace AIProject.Services
{
    public interface IAiService
    {
        Task<string> GetResponseAsync(List<ChatMessage> messages);
    }
}