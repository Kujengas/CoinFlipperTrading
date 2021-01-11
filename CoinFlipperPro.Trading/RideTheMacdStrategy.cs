using CoinFlipperPro.DataAccess;
using CoinFlipperPro.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoinFlipperPro.Trading
{
   public class RideTheMacdStrategy :StrategyBase
    {
     
        protected override Model.TradeDecision ShouldBuyImpl(Model.FlipperDataModel fdm)
        {
            Ticker te = fdm.ticker.FirstOrDefault();
            //Decimal threshold = 0.03M;
            decimal saftey = ((te.high - te.low) * .8M) + te.low;
            decimal safteyLow = ((te.high - te.low) * .2M) + te.low;

            decimal avgHigh = fdm.macdIntervalSlow.CurrentHighPriceThresh();
            decimal avgLow = fdm.macdIntervalSlow.CurrentLowPriceThresh();
            var td = new TradeDecision { doTrade = false, useMarket = false };


            if (fdm.macdIntervalSlow.isShortSMAGoingUp() && fdm.macdIntervalFast.isMacdGoingUp() /*&& fdm.macdIntervalSlow.isMacdGoingUp()*/)
            {
                td.doTrade = true;
                td.useMarket = true;
                //   td.price = fdm.macdIntervalSlow[0].AveragePrice + fdm.algoConfig.BuyThreshold;
            }



            return td;
        }

        protected override Model.TradeDecision ShouldSellImpl(Model.FlipperDataModel fdm)
        {
            var td = new TradeDecision { doTrade = false, useMarket = false };
            if (fdm.macdIntervalFast[0].CompareShortPrice > currentHighPrice)
                currentHighPrice = fdm.macdIntervalFast[0].CompareShortPrice;

            decimal calculatedMarketPrice = fdm.depth.ActualMarketBid(fdm.stats.Volume - fdm.algoConfig.ReservedCoin).Price;

            if (fdm.macd.Count > 5)
            {
          
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
                    else if (calculatedMarketPrice < (fdm.algoConfig.StopLossPercentage * currentHighPrice))
                    {
                        sellPrice = calculatedMarketPrice;
                        td.doTrade = true;
                        td.useMarket = true;
                        td.price = sellPrice;
                    }
                }

            }
            return td;
        }

    }
}
