using Microsoft.AspNetCore.SignalR;

namespace BackendServer;

public class StockPriceHub : Hub
{
    public async Task SendStockPrice(List<StockPrice> stockPriceModel)
    {
        await Clients.All.SendAsync("ReceiveStockPrices", stockPriceModel);
    }
}