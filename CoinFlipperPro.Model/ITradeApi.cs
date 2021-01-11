using System;
namespace CoinFlipperPro.Model
{

   public interface ITradeApi
    {
        bool CancelOrder(long orderId);
        ApiOrderResult DoTrade(CoinFlipperPro.Model.TradeType tradeType, double? price, double? amount);
        ApiAccountInfo GetAccountinfo();
        ApiSettings GetApiSettings();
        OrderSet GetUnfulfilledOrders(long orderId);
        ApiOrderResult Trade(double? price, decimal? amount, CoinFlipperPro.Model.TradeType tradeType, CoinFlipperPro.Model.ApiAccountInfo accountInfo);
    }
}
