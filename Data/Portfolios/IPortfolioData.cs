using HotPickMVC.Models;

namespace HotPickMVC.Data.Portfolios
{
    public interface IPortfolioData
    {
        Task AddStock(string pId, StockModel ticker);
        Task CreatePortfolio(PortfolioModel portfolio);
        Task DeletePortfolio(PortfolioModel portfolio);
        Task EditPortfolio(PortfolioModel portfolio);
        Task<List<PortfolioModel>> getPortfolios(string? user);
        Task RemoveStock(string pId, StockModel ticker);
    }
}