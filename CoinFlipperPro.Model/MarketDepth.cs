using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinFlipperPro.Model
{
    public class MarketDepth
    {
      public  List<OrderPair> Bids { get; set; }
      public List<OrderPair> Asks { get; set; }

    }
}
