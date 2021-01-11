using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinFlipperPro.Model
{
  public class FlipperDataModel
    {
        public List<Ticker> ticker { get; set; }
        public decimal average { get; set; }
        public decimal recentHighPrice { get; set; }
        public decimal recentLowPrice { get; set; }
        public AccountStatus stats { get; set; }
        public ApiAccountInfo accountInfo { get; set; }
        public AlgoConfiguration algoConfig { get; set; }
        public ApiOrderResult lastOrderResult { get; set; }
        public List<FlipperCandlestick> macd { get; set; }
        public List<FlipperCandlestick> macdIntervalFast { get; set; }
        public List<FlipperCandlestick> macdIntervalSlow { get; set; }
        public OrderSet openOrders { get; set; }
        public bool isStopLossInProgress { get; set; }
        public MarketDepth depth { get; set; }
    }
}
