using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinFlipperPro.Model;
using CoinFlipperPro.DataAccess;

namespace CoinFlipperPro.Trading
{
    public class FastOverSlowStrategy : StrategyBase
    {
        protected override TradeDecision ShouldBuyImpl(FlipperDataModel fdm)
        {
            Ticker te = fdm.ticker.FirstOrDefault();
            //Decimal threshold = 0.03M;
            decimal saftey = ((te.high - te.low) * .8M) + te.low;
            decimal safteyLow = ((te.high - te.low) * .2M) + te.low;

            decimal avgHigh = fdm.macdIntervalSlow.CurrentHighPriceThresh();
            decimal avgLow = fdm.macdIntervalSlow.CurrentLowPriceThresh();
            var td = new TradeDecision { doTrade = false, useMarket = false };

            
            if (fdm.macdIntervalSlow.isShortSMAGoingUp() /*&& macdIntervalFast.isMacdPositive() && fdm.macdIntervalSlow.isMacdGoingUp()*/)
            {
                td.doTrade = true;
                td.useMarket = true;
               // td.price = fdm.macdIntervalSlow[0].AveragePrice + fdm.algoConfig.BuyThreshold;
            }



             return td;
            //    }
            //  else return false;

            // return ((te.last < (average + algoConfig.BuyThreshold)) && stats.CurrentBalance > 0);
        }

        protected override TradeDecision ShouldSellImpl(FlipperDataModel fdm)
        {
            var td = new TradeDecision { doTrade = false, useMarket = false };
            if (fdm.macdIntervalFast[0].CompareShortPrice > currentHighPrice)
                currentHighPrice = fdm.macdIntervalFast[0].CompareShortPrice;

            Ticker te = fdm.ticker.FirstOrDefault();
           
            if (fdm.macd.Count > 5)
            {
                //   if (te.last > stats.LastPurchasePrice)
                // {



                // if (!fdm.macdIntervalFast.isMacdGoingUp() || te.last >= (fdm.stats.LastPurchasePrice + fdm.algoConfig.FixedSellThreshold) || fdm.algoConfig.UseStrictSellThreshold)
                //     points += 4;



                if (fdm.stats.Volume > 10)
                {
                    decimal sellPrice;

                    if (fdm.algoConfig.UseStrictSellThreshold)
                    {
                        sellPrice = fdm.stats.LastPurchasePrice + fdm.algoConfig.FixedSellThreshold;
                        td.doTrade = true;
                        td.useMarket = false;
                        td.price = sellPrice;
                    }
                    else if (fdm.macdIntervalFast[0].ClosePrice < (fdm.algoConfig.StopLossPercentage * currentHighPrice) || (!fdm.macdIntervalFast.isShortSMAOnTop()))
                    {
                        sellPrice = fdm.ticker[0].buy;
                        td.doTrade = true;
                        td.useMarket = false;
                        td.price = sellPrice;
                    }
                }

            }
            return td;

        }

    }
}
