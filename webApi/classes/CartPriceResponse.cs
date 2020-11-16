using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webApi.classes
{
    public class CartPriceResponse
    {
        public double Total { get; set; }
        public double TotalUSD { get; set; }
        public double TotalILS { get; set; }

        public CartPriceResponse(double _Total, double _TotalUSD, double _TotalILS)
        {
            Total = _Total;
            TotalUSD = _TotalUSD;
            TotalILS = _TotalILS;
        }
    }
}