using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinFlipperPro.Model;


namespace CoinFlipperPro.Api
{
   public static class TradingApiFactory
    {
       public static ITradeApi 
           GetTradingApi(string name) 
       {

           return new OkCoinTradeApi();
       
       }
    }
}
