using System.Net.WebSockets;
using Microsoft.Extensions.Logging;
using System.Text;

namespace LibraryAPI.Services 
{
    public class OverdueRentalNotificationService : IOverdueRentalNotificationService
    {
        private readonly IBookRentalService _rentalService;
        private readonly ILogger<OverdueRentalNotificationService> _logger;

        public OverdueRentalNotificationService(IBookRentalService rentalService, ILogger<OverdueRentalNotificationService> logger) {
            _rentalService = rentalService;
            _logger = logger;
        }

        public async Task HandleNotification(string userId, WebSocket webSocket, CancellationToken cancellationToken)
        {
            try
            {
                while (webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
                {
                    var rentals = await _rentalService.GetOverdueBooks(userId);
                    _logger.LogInformation(rentals.Count().ToString());
                    if (rentals.Count() > 0)
                    {
                        var responseBuffer = Encoding.UTF8.GetBytes($"You have overdue books: {string.Join(", ", rentals.Select(r => r.Book.Title))}");
                        await webSocket.SendAsync(new ArraySegment<byte>(responseBuffer), WebSocketMessageType.Text, true, cancellationToken);
                    }
                    await Task.Delay(5000, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in WebSocket handling.");
            }
            finally
            {
                if (webSocket.State != WebSocketState.Closed)
                {
                    try
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "WebSocket closed by the server.", cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error closing WebSocket.");
                    }
                }
            }
        }
    }
}
