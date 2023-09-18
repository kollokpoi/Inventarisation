using Microsoft.AspNetCore.SignalR;

namespace Inventarisation.Services
{
    public class ScanHub : Hub
    {
        // Метод для отправки результатов сканирования клиенту
        public async Task SendScanResult(string result)
        {
            await Clients.All.SendAsync("ReceiveScanResult", result);
        }
    }
}
