namespace HotPickMVC.Models
{
    public class UserPageModel
    {

        public UserModel? User { get; set; }

        public List<PortfolioModel>? Portfolios { get; set; } = new List<PortfolioModel> { };
    }
}
