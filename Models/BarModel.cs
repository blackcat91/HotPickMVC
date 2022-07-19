using Alpaca.Markets;

namespace HotPickMVC.Models
{
    public class BarModel
    {

        public List<dynamic> BarData { get; set; } = new List<dynamic>();
        public decimal Open { get; set; }

        public decimal Close { get; set; }

        public decimal Low { get; set; }
        public decimal High { get; set; }

        public DateTime Date { get; set; }


        public BarModel()
        {

        }
       public BarModel(IBar bar)
        {

            BarData.Add(bar.TimeUtc.ToLocalTime());
            BarData.Add(bar.Open);
            BarData.Add(bar.High);
            BarData.Add(bar.Low);
            BarData.Add(bar.Close);
          
        }
    }
}
