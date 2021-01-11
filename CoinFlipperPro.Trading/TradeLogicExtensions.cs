using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinFlipperPro.Model;

namespace CoinFlipperPro.Trading
{
  public static  class TradeLogicExtensions
    {

      private static decimal rateOfChangeFactor = .002M;
      public static bool isMacdGoingUp(this List<FlipperCandlestick> lst) 
      {
         return (lst[0].Direction == MacdDirection.Up.ToString() && lst[1].Direction == MacdDirection.Up.ToString()); //|| (lst[1].Direction == MacdDirection.Up.ToString() && lst[2].Direction == MacdDirection.Up.ToString());
      }
      public static bool isMacdPlummit(this List<FlipperCandlestick> lst)
      {
          if (lst.Count > 2)
          {
              return (lst[0].Direction == MacdDirection.Down.ToString() && lst[1].Direction == MacdDirection.Down.ToString() && lst[1].Direction == MacdDirection.Down.ToString());// && lst[2].Direction == MacdDirection.Up.ToString());
          }
          else
          {
              return false;
          }
      }

      public static bool isMacdPositive(this List<FlipperCandlestick> lst)
      {
          return (lst[0].Hist > 0);
      }

      public static bool isLastCandlePositive(this List<FlipperCandlestick> lst)
      {
          return (lst[1].ClosePrice < lst[1].OpenPrice);
      }

      public static bool isCandlePositive(this List<FlipperCandlestick> lst)
      {
          return (lst[0].ClosePrice < lst[0].OpenPrice);
      }

      public static bool isShortSMAOnTop(this List<FlipperCandlestick> lst)
      {
          return (lst[0].CompareShortPrice > lst[0].CompareLongPrice + .01M);
      }


      public static bool isShortSMAGoingUp(this List<FlipperCandlestick> lst)
      {
          return IsGoingUp(lst[0].CompareShortPrice,lst[3].CompareShortPrice,4);
      }
      public static bool isShortSMAGoingDown(this List<FlipperCandlestick> lst)
      {
          return IsGoingDown(lst[0].CompareShortPrice, lst[3].CompareShortPrice, 4);
      }
      public static bool isShortSMAFlat(this List<FlipperCandlestick> lst)
      {
          return IsFlat(lst[0].CompareShortPrice, lst[3].CompareShortPrice, 4);
      }


      public static bool isLongSMAGoingUp(this List<FlipperCandlestick> lst)
      {
          return IsGoingUp(lst[0].CompareLongPrice, lst[3].CompareLongPrice, 4);
      }
      public static bool isLongSMAGoingDown(this List<FlipperCandlestick> lst)
      {
          return IsGoingDown(lst[0].CompareLongPrice, lst[3].CompareLongPrice, 4);
      }
      public static bool isLongSMAFlat(this List<FlipperCandlestick> lst)
      {
          return IsFlat(lst[0].CompareLongPrice, lst[3].CompareLongPrice, 4);
      }



      private static bool IsGoingUp(decimal currentValue, decimal oldValue, int count) 
      {
          return (((currentValue - oldValue) / count) > rateOfChangeFactor);
      
      }

      private static bool IsGoingDown(decimal currentValue, decimal oldValue, int count)
      {
          return (((currentValue - oldValue) / count) < (rateOfChangeFactor * -1));

      }

      private static bool IsFlat(decimal currentValue, decimal oldValue, int count)
      {
          return (((currentValue - oldValue) / count) > -.005M && ((currentValue - oldValue) / count) < rateOfChangeFactor);

      }



      public static bool isPriceLow(this List<FlipperCandlestick> lst)
      {

          decimal diff = lst[1].SMAHigh - lst[1].SMALow;

          decimal avgLow = lst[1].SMALow + ((diff * .65M) / 2);

                   
          
          return (lst[0].ClosePrice < avgLow);
      }


      public static bool isPriceHigh(this List<FlipperCandlestick> lst)
      {

          decimal diff = lst[1].SMAHigh - lst[1].SMALow;
    

          decimal avgHigh = lst[1].SMAHigh - ((diff * .65M) / 2);


          return (lst[0].ClosePrice > avgHigh);
      }


      public static decimal CurrentLowPriceThresh(this List<FlipperCandlestick> lst)
      {

          decimal diff = lst[1].SMAHigh - lst[1].SMALow;

          decimal avgLow = lst[1].SMALow + ((diff * .65M) / 2);



          return avgLow;
      }


      public static decimal CurrentHighPriceThresh(this List<FlipperCandlestick> lst)
      {

          decimal diff = lst[1].SMAHigh - lst[1].SMALow;


          decimal avgHigh = lst[1].SMAHigh - ((diff * .65M) / 2);


          return avgHigh;
      }

      public static decimal MacdAveragePrice(this List<FlipperCandlestick> lst)
      {
          return lst.Average(x => x.ClosePrice);
      }

      public static OrderPair ActualMarketAsk(this MarketDepth depth, decimal money) 
      {
          var currentRow=0;
          var currentPrice = depth.Asks[currentRow].Price;
          var currentAmount = depth.Asks[currentRow].Amount;
          List<OrderPair> tmpOrderSet = new List<OrderPair>();
            while (money > 0) {

                if (money > currentPrice * currentAmount)
                {
                    tmpOrderSet.Add(depth.Asks[currentRow]);
                    money -= currentPrice * currentAmount;
                }
                else 
                {
                    var newAmount = money / currentPrice;
                    tmpOrderSet.Add(new OrderPair { Amount = newAmount, Price = currentPrice });
                    money = 0;
                }

               currentRow++; 
               currentPrice = depth.Asks[currentRow].Price;
               currentAmount = depth.Asks[currentRow].Amount;
            
            }
            decimal totalShares = 0;
            decimal totalCost = 0;

            foreach (OrderPair p in tmpOrderSet)
            {
                totalShares += p.Amount;
                totalCost += p.Price * p.Amount;
            }

            return new OrderPair { Amount = totalShares, Price = totalCost / totalShares };
      
      
      }

      public static OrderPair ActualMarketBid(this MarketDepth depth, decimal amount)
      {
          var currentRow = 0;
          var currentPrice = depth.Bids[currentRow].Price;
          var currentAmount = depth.Bids[currentRow].Amount;
          List<OrderPair> tmpOrderSet = new List<OrderPair>();
          while (amount > 0)
          {

              if (amount >  currentAmount)
              {
                  tmpOrderSet.Add(depth.Asks[currentRow]);
                  amount -= currentAmount;
              }
              else
              {
                
                  tmpOrderSet.Add(new OrderPair { Amount = amount, Price = currentPrice });
                  amount = 0;
              }

              currentRow++;
              currentPrice = depth.Bids[currentRow].Price;
              currentAmount = depth.Bids[currentRow].Amount;

          }
          decimal totalShares = 0;
          decimal totalCost = 0;

          foreach (OrderPair p in tmpOrderSet)
          {
              totalShares += p.Amount;
              totalCost += p.Price * p.Amount;
          }

          decimal linqAmount = (from OrderPair pr in tmpOrderSet select pr.Amount).Sum();

          decimal linqCost = (from OrderPair pr in tmpOrderSet select pr.Amount*pr.Price).Sum();
          
          
          
          return new OrderPair { Amount = totalShares, Price = totalCost / totalShares };


      }

    }
}
