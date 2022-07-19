using HotPickMVC.Models;

namespace HotPickMVC.Data.Stocks
{
    public interface IStockData
    {
        Task<List<StockModel>?> GetStock(string tickers);
        Task<List<StockModel>?> PaginateStocks(int? startId);
        Task<List<StockModel>?> PaginateStocksPrev(string? startId);
        Task<List<StockModel>?> SearchStocks(string query);
        Task<List<StockModel>> TopTen();
    }
}