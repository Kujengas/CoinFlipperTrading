using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json;
using System.IO;
using CoinFlipperPro.Model;

namespace CoinFlipperPro.Api
{
    public  class OkCoinTradeApi : ITradeApi
    {
        public OkCoinTradeApi() 
        {
        
        
        }
        
        public  ApiAccountInfo GetAccountinfo()
        {
            try
            {
                string hashInput;
                var apisettings = GetApiSettings();


                hashInput = String.Format("partner={0}{1}", apisettings.partnerId, apisettings.secretKey);
                string sig = Crypto.CalculateMD5Hash(hashInput);
                string result;

                using (var wb = new FlipperWebClient())
                      {
                  
                          var data = new NameValueCollection();
                          data["partner"] = apisettings.partnerId;
                          data["sign"] = sig.ToUpper();
                          var response = wb.UploadValues(apisettings.accountUri, "POST", data);
                          result = System.Text.Encoding.Default.GetString(response);
                         
                      }
               
                     
               

                return (ApiAccountInfo)JsonConvert.DeserializeObject<ApiAccountInfo>(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public  ApiOrderResult Trade(double? price, decimal? amount, TradeType tradeType, ApiAccountInfo accountInfo)
        {
            try
            {

                string hashInput = string.Empty;

                var apisettings = GetApiSettings();

                string action = string.Empty;

                switch (tradeType)
                {
                    case TradeType.Buy:
                        action = "buy";
                        hashInput = string.Format("amount={2}&partner={3}&rate={1}&symbol=ltc_cny&type={0}{4}", action, price, amount, apisettings.partnerId, apisettings.secretKey);
                        break;
                    case TradeType.BuyMarket:
                        action = "buy_market";
                        hashInput = string.Format("partner={2}&rate={1}&symbol=ltc_cny&type={0}{3}", action, price, apisettings.partnerId, apisettings.secretKey);
                        break;
                    case TradeType.Sell:
                        action = "sell";
                        hashInput = string.Format("amount={2}&partner={3}&rate={1}&symbol=ltc_cny&type={0}{4}", action, price, amount, apisettings.partnerId, apisettings.secretKey);

                        break;
                    case TradeType.SellMarket:
                        action = "sell_market";
                        hashInput = string.Format("amount={1}&partner={2}&symbol=ltc_cny&type={0}{3}", action, amount, apisettings.partnerId, apisettings.secretKey);

                        break;

                }


                string sig = Crypto.CalculateMD5Hash(hashInput);
                string result;

                using (var wb = new FlipperWebClient())
                {
                    var data = new NameValueCollection();
                    data["amount"] = amount.ToString();
                    data["partner"] = apisettings.partnerId;
                    data["rate"] = price.ToString();
                    data["symbol"] = "ltc_cny";
                    data["type"] = action;
                    data["sign"] = sig.ToUpper();
                    var response = wb.UploadValues(apisettings.tradeUri, "POST", data);
                    result = System.Text.Encoding.Default.GetString(response);
                }

                return (ApiOrderResult)JsonConvert.DeserializeObject<ApiOrderResult>(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public  ApiSettings GetApiSettings()
        {
            ApiSettings apisettings;

            if (ConfigurationManager.AppSettings["db"] == "OkCoinChina")
            {


                apisettings = new ApiSettings
               {
                   secretKey = Constants.configOkCoinChinaSecretKey,
                   partnerId = Constants.configOkCoinChinaPartner,
                   tradeUri = Constants.configOkCoinChinaRestEndPointPlaceOrder,
                   accountUri = Constants.configOkCoinChinaRestEndPointAccountInfo,
                   batchOrderUri = Constants.configOkCoinChinaRestEndPointBatchOrder,
                   orderHistoryUri = Constants.configOkCoinChinaRestEndPointOrderHistory,
                   getOrderUri = Constants.configOkCoinChinaRestEndPointGetOrder,
                   getBatchOrderUri = Constants.configOkCoinChinaRestEndPointBatchOrderGet,
                   getMarketDepthUri = Constants.configOkCoinChinaRestEndPointMarketDepth,
                   getCancelOrderUri = Constants.configOkCoinChinaRestEndPointCancelOrder
               };
            }
            else// if (ConfigurationManager.AppSettings["db"] == "OkCoinUS")
            {
                apisettings = new ApiSettings
               {
                   secretKey = Constants.configOkCoinUSSecretKey,
                   partnerId = Constants.configOkCoinUSPartner,
                   tradeUri = Constants.configOkCoinUSRestEndPointPlaceOrder,
                   accountUri = Constants.configOkCoinUSRestEndPointAccountInfo,
                   batchOrderUri = Constants.configOkCoinUSRestEndPointBatchOrder,
                   orderHistoryUri = Constants.configOkCoinUSRestEndPointOrderHistory,
                   getOrderUri = Constants.configOkCoinUSRestEndPointGetOrder,
                   getBatchOrderUri = Constants.configOkCoinUSRestEndPointBatchOrderGet,
                   getMarketDepthUri = Constants.configOkCoinUSRestEndPointMarketDepth,
                   getCancelOrderUri = Constants.configOkCoinUSRestEndPointCancelOrder

               };

            }

            return apisettings;

        }

        public  ApiOrderResult DoTrade(TradeType tradeType, double? price, double? amount)
        {

            try
            {
                ApiAccountInfo accountInfo = GetAccountinfo();
                ApiOrderResult result = new ApiOrderResult();
                switch (tradeType)
                {
                    case TradeType.BuyMarket:
                        result = this.Trade(Convert.ToDouble(accountInfo.info.funds.free.cny), null, tradeType, accountInfo);

                        break;
                    case TradeType.Buy:
                        result = this.Trade(price, Convert.ToDecimal(accountInfo.info.funds.free.cny) / Convert.ToDecimal(price), tradeType, accountInfo);
                        break;
                    case TradeType.SellMarket:
                        result = this.Trade(null, Convert.ToDecimal(accountInfo.info.funds.free.ltc), tradeType, accountInfo);
                        break;
                    case TradeType.Sell:
                        result = this.Trade(price, Convert.ToDecimal(accountInfo.info.funds.free.ltc), tradeType, accountInfo);
                        break;


                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public  bool CancelOrder(long orderId)
        {
            try
            {

                string hashInput = string.Empty;

                var apisettings = GetApiSettings();

                string action = string.Empty;

                hashInput = string.Format("order_id={0}&partner={1}&symbol=ltc_cny{2}", orderId, apisettings.partnerId, apisettings.secretKey);



                string sig = Crypto.CalculateMD5Hash(hashInput);
                string result;

                using (var wb = new FlipperWebClient())
                {

                    var data = new NameValueCollection();
                    data["order_id"] = orderId.ToString();
                    data["partner"] = apisettings.partnerId;
                    data["symbol"] = "ltc_cny";
                    data["sign"] = sig.ToUpper();
                    var response = wb.UploadValues(apisettings.getCancelOrderUri, "POST", data);
                    result = System.Text.Encoding.Default.GetString(response);

                }
                return (result.Contains("success"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public  OrderSet GetUnfulfilledOrders(long orderId)
        {

            try
            {
                string hashInput = string.Empty;

                var apisettings = GetApiSettings();

                string action = string.Empty;

                hashInput = string.Format("order_id={0}&partner={1}&symbol=ltc_cny{2}", orderId, apisettings.partnerId, apisettings.secretKey);



                string sig = Crypto.CalculateMD5Hash(hashInput);
                string result;

                using (var wb = new FlipperWebClient())
                {

                    var data = new NameValueCollection();
                    data["order_id"] = orderId.ToString();
                    data["partner"] = apisettings.partnerId;
                    data["symbol"] = "ltc_cny";
                    data["sign"] = sig.ToUpper();
                    var response = wb.UploadValues(apisettings.getOrderUri, "POST", data);
                    result = System.Text.Encoding.Default.GetString(response);
                }
                return (OrderSet)JsonConvert.DeserializeObject<OrderSet>(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
