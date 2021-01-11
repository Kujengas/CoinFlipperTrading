using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinFlipperPro.Model;
using System.Configuration;
using System.Collections.Specialized;


namespace CoinFlipperPro.Api
{
    class BterTradeApi:ITradeApi
    {
        public bool CancelOrder(long orderId)
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

        public CoinFlipperPro.Model.ApiOrderResult DoTrade(CoinFlipperPro.Model.TradeType tradeType, double? price, double? amount)
        {
            throw new NotImplementedException();
        }

        public CoinFlipperPro.Model.ApiAccountInfo GetAccountinfo()
        {
            throw new NotImplementedException();
        }

        public CoinFlipperPro.Model.ApiSettings GetApiSettings()
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

        public CoinFlipperPro.Model.OrderSet GetUnfulfilledOrders(long orderId)
        {
            throw new NotImplementedException();
        }

        public CoinFlipperPro.Model.ApiOrderResult Trade(double? price, decimal? amount, CoinFlipperPro.Model.TradeType tradeType, CoinFlipperPro.Model.ApiAccountInfo accountInfo)
        {
            throw new NotImplementedException();
        }
    }
}
