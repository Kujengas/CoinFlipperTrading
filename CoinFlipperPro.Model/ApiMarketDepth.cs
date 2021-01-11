using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CoinFlipperPro.Model
{
    public class ApiMarketDepth
    {

        public List<List<double>> asks { get; set; }
        public List<List<double>> bids { get; set; }
        public MarketDepth ToMarketDepth()
        {
            var depth = new MarketDepth();

            depth.Asks = (from ask in asks
                          select new OrderPair { Price = ask[0].ConvertToDecimal(), Amount = ask[1].ConvertToDecimal() }).OrderBy(a=>a.Price).ToList();
            depth.Bids = (from bid in bids
                          select new OrderPair { Price = bid[0].ConvertToDecimal(), Amount = bid[1].ConvertToDecimal() }).OrderByDescending(a => a.Price).ToList();
            return depth;
        }


    }
}
