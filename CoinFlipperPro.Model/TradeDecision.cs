using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinFlipperPro.Model
{
    public class TradeDecision
    {
        public Boolean doTrade { get; set; }
        public bool useMarket { get; set; }
        public decimal? amount { get; set; }
        public decimal? price { get; set; }
    }
}
