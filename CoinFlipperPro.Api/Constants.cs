using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinFlipperPro.Api
{
    public static class Constants
    {

        public const string configOkCoinChinaRestEndPointTicker = "https://www.okcoin.cn/api/ticker.do?symbol=ltc_cny";
        public const string configOkCoinChinaRestEndPointAccountInfo = "https://www.okcoin.cn/api/userinfo.do";
        public const string configOkCoinChinaRestEndPointPlaceOrder = "https://www.okcoin.cn/api/trade.do";
        public const string configOkCoinChinaRestEndPointBatchOrder = "https://www.okcoin.cn/api/batchTrade.do";
        public const string configOkCoinChinaRestEndPointGetOrder = "https://www.okcoin.cn/api/getorder.do";
        public const string configOkCoinChinaRestEndPointCancelOrder = "https://www.okcoin.cn/api/cancelorder.do";
        public const string configOkCoinChinaRestEndPointOrderHistory = "https://www.okcoin.cn/api/getOrderHistory.do";
        public const string configOkCoinChinaRestEndPointBatchOrderGet = "https://www.okcoin.cn/api/getBatchOrder.do";
        public const string configOkCoinChinaRestEndPointMarketDepth = "https://www.okcoin.cn/api/depth.do?symbol=ltc_cny";

        public const string configOkCoinChinaWSEndPoint = "wss://real.okcoin.cn:10440/websocket/okcoinapi";
        public const string configOkCoinChinaPartner = "";
        public const string configOkCoinChinaSecretKey = "";


        public const string configOkCoinUSRestEndPointAccountInfo = "https://www.okcoin.com/api/userinfo.do";
        public const string configOkCoinUSRestEndPointTicker = "https://www.okcoin.com/api/ticker.do?symbol=ltc_usd&ok=1";
        public const string configOkCoinUSRestEndPointPlaceOrder = "https://www.okcoin.com/api/trade.do";
        public const string configOkCoinUSRestEndPointBatchOrder = "https://www.okcoin.com/api/batchTrade.do";
        public const string configOkCoinUSRestEndPointGetOrder = "https://www.okcoin.com/api/getorder.do";
        public const string configOkCoinUSRestEndPointCancelOrder = "https://www.okcoin.com/api/cancelorder.do";
        public const string configOkCoinUSRestEndPointOrderHistory = "https://www.okcoin.com/api/getOrderHistory.do";
        public const string configOkCoinUSRestEndPointBatchOrderGet = "https://www.okcoin.com/api/getBatchOrder.do";
        public const string configOkCoinUSRestEndPointMarketDepth = "https://www.okcoin.com/api/depth.do?symbol=ltc_usd&ok=1";
        public const string configOkCoinUSWSEndPoint = "wss://real.okcoin.com:10440/websocket/okcoinapi";
        public const string configOkCoinUSPartner = "";
        public const string configOkCoinUSSecretKey = "";


        public const string configBterRestEndPointAccountInfo = "https://bter.com/api/1/private/getfunds";
        public const string configBterRestEndPointTicker = "https://bter.com/api/1/ticker/ticker/ltc_cny";
        public const string configBterRestEndPointPlaceOrder = "https://bter.com/api/1/private/placeorder";
       // public const string configBterRestEndPointBatchOrder = "https://bter.com/api/1/private/batchTrade.do";
        public const string configBterRestEndPointGetOrder = "https://bter.com/api/1/private/getorder";
        public const string configBterRestEndPointCancelOrder = "https://bter.com/api/1/private/cancelorder";
        public const string configBterRestEndPointOrderHistory = "https://bter.com/api/1/private/orderlist";
       // public const string configBterRestEndPointBatchOrderGet = "https://bter.com/api/1/private/getBatchOrder.do";
        public const string configBterRestEndPointMarketDepth = "https://bter.com/api/1/depth/ltc_cny";
        public const string configBterWSEndPoint = "wss://real.okcoin.com:10440/websocket/okcoinapi";
        public const string configBterPartner = "";
        public const string configBterSecretKey = "";



    }
}
