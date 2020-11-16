using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace webApi.classes
{
    public class Book
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public string Image { get; set; }
        public int Price { get; set; }
        public int SallePrice { get; set; }
        public int GroupBook { get; set; }  
        public string login { get; set; }
        public bool Available { get; set; }
        public double PriceUSD { get; set; }
        public double PriceILS { get; set; }
        public double SalePriceUSD { get; set; }
        public double SalePriceILS { get; set; }

        public int Currency { get; set; }
    }
}