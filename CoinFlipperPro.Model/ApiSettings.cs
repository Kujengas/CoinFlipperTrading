using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinFlipperPro.Model
{
    public class ApiSettings
    {
        public string getMarketDepthUri { get; set; }
        public string secretKey { get; set; }
        public string partnerId { get; set; }
        public string tradeUri { get; set; }
        public string accountUri { get; set; }
        public string batchOrderUri { get; set; }
        public string orderHistoryUri { get; set; }
        public string getOrderUri { get; set; }
        public string getBatchOrderUri { get; set; }
        public string getCancelOrderUri { get; set; }

    }
}




