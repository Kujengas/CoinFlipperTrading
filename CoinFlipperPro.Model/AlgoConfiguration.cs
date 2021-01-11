using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinFlipperPro.Model
{
   public class AlgoConfiguration
    {
       //public bool IsAlgoOn { get; set; }
       public decimal BuyThreshold { get; set; }
       public decimal SellThreshold { get; set; }
       public decimal FixedSellThreshold { get; set; }
       public int MacdIntervalTime { get; set; }
       public bool UseStrictSellThreshold { get; set; }
       public decimal StopLossPercentage { get; set; }
       public int StopLossTime { get; set; }
       public StopMode StopLossMode { get; set; }
       public TradingAlgorithm Algorithm { get; set; }
       public decimal ReservedCoin { get; set; }
       
   }
}
