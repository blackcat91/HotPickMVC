using Alpaca.Markets;
using Newtonsoft.Json;

using System.Web;

namespace HotPickMVC.Models
{
    public class AllStockDataModel
    {

        public StockModel Stock { get; set; }

        public List<INewsArticle>?  News{ get; set; } =     new List<INewsArticle>();

        public List<BarModel>? Bars{ get; set; } = new List<BarModel>();

        public List<PortfolioModel>? Portfolios { get; set; } = new List<PortfolioModel> { };

        public List<VolumeModel>? Volumes{ get; set; } = new List<VolumeModel>();
       

        public AllStockDataModel( List<IBar> bars, StockModel stock)
        {
            Stock = stock;
            foreach (var bar in bars)
            {

                Bars.Add(new BarModel(bar));
                Volumes.Add(new VolumeModel(bar));
            }
            
            


        }

    }
}
