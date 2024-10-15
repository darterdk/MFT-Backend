using Microsoft.AspNetCore.SignalR;

namespace BackendServer;

public class StockPriceGenerator : IHostedService, IDisposable
{
    private Timer _timer;
    private readonly IHubContext<StockPriceHub> _hubContext;
    private readonly Random _random = new ();

    public StockPriceGenerator(IHubContext<StockPriceHub> hubContext, Timer timer)
    {
        _hubContext = hubContext;
        _timer = timer;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Set the timer to call GenerateStockPrices every 1-2 seconds
        _timer = new Timer(GenerateStockPrices, null, TimeSpan.Zero, TimeSpan.FromSeconds(1.5));
        return Task.CompletedTask;
    }

    private async void GenerateStockPrices(object state)
    {
        // Define a list of stock names 
        var stockNames = new List<string> { "AAPL", "GOOGL", "MSFT" };
        var stockPrices = new List<StockPrice>();

        // Assign random prices to the stocks
        foreach (var name in stockNames)
        {
            var stock = new StockPrice
            {
                Name = name,
                Price = GetRandomPrice()
            };
            stockPrices.Add(stock);
        }

        // Send stock price model to all clients
        await _hubContext.Clients.All.SendAsync("ReceiveStockPrices", stockPrices);
    }

    private decimal GetRandomPrice()
    {
        // Generate a random price between 100 and 1500
        return Math.Round((decimal)(_random.NextDouble() * 1400 + 100), 2);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}