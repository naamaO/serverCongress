using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webApi.classes
{
    public class Cart
    {
        // public int Id { get; set; }
        public string UserName { get; set; }
        public int IdBook { get; set; }
        public string NameBook { get; set; }
        public string DetailsBook { get; set; }
        public int PriceBook { get; set; }
        public string ImageBook { get; set; }
        public int Quantity { get; set; }
        public int SallePrice { get; set; }
        public int Currency { get; set; }
    }
}
