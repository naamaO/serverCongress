namespace webExample.Controllers
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int IdBook { get; set; }
        public string NameBook { get; set; }
        //  public string DetailsBook { get; set; }
        public double PriceBook { get; set; }
        public string ImageBook { get; set; }
        public int Quantity { get; set; }
        public double SallePrice { get; set; }
        public int Currency { get; set; }
        public double PriceILS { get; set; }
        public double PriceUSD { get; set; }

        public double SalePriceUSD { get; set; }
        public double SalePriceILS { get; set; }

    }
}