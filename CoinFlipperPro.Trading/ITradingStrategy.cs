using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinFlipperPro.Model;
using CoinFlipperPro.DataAccess;

namespace CoinFlipperPro.Trading
{
  public interface ITradingStrategy
    {
       Boolean IsStopLossInProgress {get;set;}
        AccountStatus Buy(FlipperDataModel fdm, TradeDecision td,ITradeApi tradeApi);
        AccountStatus Sell(FlipperDataModel fdm, TradeDecision td, ITradeApi tradeApi);
        AccountStatus StopLoss(FlipperDataModel fdm, TradeDecision td, ITradeApi tradeApi);
        TradeDecision ShouldBuy(FlipperDataModel fdm);
        TradeDecision ShouldSell(FlipperDataModel fdm);
        TradeDecision ShouldStopLoss(FlipperDataModel fdm);
    }

}









    

