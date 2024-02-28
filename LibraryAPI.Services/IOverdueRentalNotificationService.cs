using System.Net.WebSockets;
using System.Text;

namespace LibraryAPI.Services 
{
    public interface IOverdueRentalNotificationService 
    {
        Task HandleNotification(string userId, WebSocket webSocket, CancellationToken cancellationToken);
    }
}