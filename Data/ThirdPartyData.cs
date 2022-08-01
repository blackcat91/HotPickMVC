using Alpaca.Markets;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;

namespace HotPickMVC.Data
{
    public class ThirdPartyData :IDisposable
    {
       

        public string ALPACA_KEY = "AK800MFRCDVNA1VJP3PJ";
        public string ALPACA_SECRET = "VjkqQrbMqajdeONvxSMP2rGbRNKTb2QJELwM6fHz";

        private IAlpacaDataClient? _alpacaDataClient;

        private IAlpacaDataStreamingClient? _alpacaDataStreamingClient;

        private IAlpacaTradingClient? alpacaTradingClient;

        public async Task<List<IBar>?> GetTickerStream(string ticker)
        {
            

            _alpacaDataClient = Alpaca.Markets.Environments.Paper
                .GetAlpacaDataClient(new SecretKey(ALPACA_KEY, ALPACA_SECRET));
            DateTime now = DateTime.Now.Subtract(TimeSpan.FromMinutes(16));
            DateTime from = now.Subtract(TimeSpan.FromDays(365));
            var req = new HistoricalBarsRequest(ticker,from, now, BarTimeFrame.Day );
           var quote =  await _alpacaDataClient.GetHistoricalBarsAsync(req);
            var bars = new List<IBar> { };
            _alpacaDataStreamingClient = Alpaca.Markets.Environments.Paper.GetAlpacaDataStreamingClient(new SecretKey(ALPACA_KEY, ALPACA_SECRET));

            foreach (var bar in quote.Items.AsEnumerable())
            {
                foreach(var b in bar.Value.ToList())
                {
                    bars.Add(b);
                }
                
            }
            try
            {
                await _alpacaDataStreamingClient.ConnectAndAuthenticateAsync();
                var sub = _alpacaDataStreamingClient.GetDailyBarSubscription(ticker);
                sub.Received += async bar =>
                {
                    var minutesUntilClose = now - DateTime.UtcNow;
                    if (minutesUntilClose.TotalMinutes < 15)
                    {
                        await _alpacaDataStreamingClient.DisconnectAsync();
                    }
                    else
                    {
                        bars.Add(bar);
                    }

                };
                await _alpacaDataStreamingClient.SubscribeAsync(sub);
            }
            catch(Exception ex)
            {
                return null;
            }
            
            return bars;
        }

        public async Task<IPage<INewsArticle>?> GetNewsStream(string ticker)
        {
            _alpacaDataClient = Alpaca.Markets.Environments.Paper
                 .GetAlpacaDataClient(new SecretKey(ALPACA_KEY, ALPACA_SECRET));
            DateTime now = DateTime.Now.Subtract(TimeSpan.FromMinutes(10));
            DateTime from = now.Subtract(TimeSpan.FromMinutes(40));
            try
            {
                List<string> t = new List<string> { };
                t.Add(ticker);
                var req = new NewsArticlesRequest(t);

                req.TimeInterval = now.GetIntervalTillThat();


                var news = await _alpacaDataClient.ListNewsArticlesAsync(req);
                return news;
            }
            catch(Exception ex)
            {
                return null;
            }


            
        }

        public void Dispose()
        {
            _alpacaDataClient?.Dispose();
            _alpacaDataStreamingClient?.Dispose();
            alpacaTradingClient?.Dispose();
        }
    }
}
