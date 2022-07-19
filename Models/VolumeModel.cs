using Alpaca.Markets;

namespace HotPickMVC.Models
{
    public class VolumeModel
    {

        public decimal Volume { get; set; } 
        public DateTime Date { get; set; }

        public VolumeModel(IBar bar)
        {
            Volume = bar.Volume;
            Date = bar.TimeUtc.ToLocalTime();
        }
    }
}
