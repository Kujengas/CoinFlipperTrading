using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinFlipperPro.Model;
using CoinFlipperPro.DataAccess;

namespace CoinFlipperPro.Trading
{
    public class SimulationStategy : ITradingStrategy
    {
        public Boolean IsStopLossInProgress { get; set; }
        public AccountStatus Buy(FlipperDataModel fdm, TradeDecision td, ITradeApi tradeApi)
        {
            if (!IsStopLossInProgress && td.doTrade)
            {
                Ticker te = fdm.ticker.FirstOrDefault();
                AccountStatus stat = new AccountStatus
                {
                    CurrentBalance = 0,
                    LastPurchasePrice = te.last,
                    LastSalePrice = fdm.stats.LastSalePrice,
                    StartingBalance = fdm.stats.StartingBalance,
                    StatDate = DateTime.Now,
                    Volume = (fdm.stats.CurrentBalance / te.last),
                    Source = TransactionSource.Simulator,
                    IsSimulator = true,
                    TransactionType = TransactionType.Buy,
                    LastOrderDate = DateTime.Now
                };

                TickerDAL.InsertTransaction(stat);
                return stat;
            }
            else
            {
                return fdm.stats;

            }

        }

        public AccountStatus Sell(FlipperDataModel fdm, TradeDecision td, ITradeApi tradeApi)
        {
            if (td.doTrade)
            {
                Ticker te = fdm.ticker.FirstOrDefault();
                AccountStatus stat = new AccountStatus
                {
                    CurrentBalance = (fdm.stats.Volume * te.last),
                    LastPurchasePrice = fdm.stats.LastPurchasePrice,
                    LastSalePrice = te.last,
                    StartingBalance = fdm.stats.StartingBalance,
                    StatDate = DateTime.Now,
                    Volume = 0,
                    IsSimulator = true,
                    TransactionType = TransactionType.Sell,
                    Source = TransactionSource.Simulator,
                    LastOrderDate = DateTime.Now
                };
                TickerDAL.InsertTransaction(stat);
                return stat;
            }

            else
            {
                return fdm.stats;
            }
        }

        public AccountStatus StopLoss(FlipperDataModel fdm, TradeDecision td, ITradeApi tradeApi)
        {
            if (td.doTrade)
            {

                Ticker te = fdm.ticker.FirstOrDefault();
                AccountStatus stat = new AccountStatus
                {
                    CurrentBalance = (fdm.stats.Volume * te.last),
                    LastPurchasePrice = fdm.stats.LastPurchasePrice,
                    LastSalePrice = te.last,
                    StartingBalance = fdm.stats.StartingBalance,
                    StatDate = DateTime.Now,
                    Volume = 0,
                    IsSimulator = true,
                    TransactionType = TransactionType.Sell,
                    Source = TransactionSource.Simulator,
                    LastOrderDate = DateTime.Now
                };
                TickerDAL.InsertTransaction(stat);
                return stat;
            }
            else 
            {

                return fdm.stats;
            }
        }


        public TradeDecision ShouldBuy(FlipperDataModel fdm)
        {
            Ticker te = fdm.ticker.FirstOrDefault();
            //Decimal threshold = 0.03M;
            decimal saftey = ((te.high - te.low) * .8M) + te.low;
            decimal safteyLow = ((te.high - te.low) * .2M) + te.low;

            decimal avgHigh = fdm.macdIntervalSlow.CurrentHighPriceThresh();
            decimal avgLow = fdm.macdIntervalSlow.CurrentLowPriceThresh();

            Console.WriteLine("AvgLow:{0}  TickerLast:{1}  Difference:{2}   AvgHigh:{3}   Spread:{4}", avgLow, te.last, (te.last - avgLow), avgHigh, (avgHigh - avgLow));

            //if (macdInterval.isMacdGoingUp()||(macdInterval[0].Hist>0.01M)
            //    {
            //-----------------------------------------------------------------------------------------------------------
            //  if (macd.Count > 5 && (stats.StatDate < DateTime.Now.AddSeconds(-5) || te.last < stats.LastSalePrice))
            //  {
            //    if (macdInterval.isMacdGoingUp())
            //    {
            //        if (macd.isLastCandlePositive())
            //        {
            //            if (te.last < saftey)
            //            {
            //                int points = 0;

            //               // if (te.last < (average + algoConfig.BuyThreshold) || macdInterval.isPriceLow())
            //              //  {
            //                if (!macd.isMacdPlummit())
            //                {

            //                    //   if (macdInterval.isMacdPositive()) 

            //                    points += 3;





            //                    // points += 3;
            //                    // if (macd[0].Direction == MacdDirection.Up.ToString() && macd[1].Direction == MacdDirection.Up.ToString()) //&& macd[2].Direction.ToString() == MacdDirection.Down.ToString() && macd[3].Direction == MacdDirection.Down.ToString())
            //                    //     points += 3;
            //                    //  if (macd[1].Direction == MacdDirection.Up.ToString() && macd[2].Direction == MacdDirection.Up.ToString())
            //                    //      points += 3;
            //                    //   }

            //                }
            //                return (points >= 3 && stats.CurrentBalance > 1);
            //            }
            //        }
            //  //  }

            //    }

            //}
            //return false;
            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------


            int points = 0;
            if (fdm.macdIntervalSlow.isMacdGoingUp() && /*macdIntervalFast.isMacdPositive() && */fdm.macdIntervalFast.isMacdGoingUp())
            {
                points += 3;
            }

            //  if (macdIntervalSlow.isMacdPositive() && macdIntervalFast.isMacdPositive())
            //   {
            //       points += 3;
            //  }


            if (points >= 3 && fdm.stats.CurrentBalance > 10)
            {


                return (new TradeDecision { doTrade = true, useMarket = true });
            }
            else return (new TradeDecision { doTrade = false, useMarket = false });
            //    }
            //  else return false;

            // return ((te.last < (average + algoConfig.BuyThreshold)) && stats.CurrentBalance > 0);
        }

        public TradeDecision ShouldSell(FlipperDataModel fdm)
        {
            Ticker te = fdm.ticker.FirstOrDefault();
            //  if (stats.IsSimulator)Thiscommentedsojustrideitout
            //  {
            if (fdm.macd.Count > 5)
            {
                //   if (te.last > stats.LastPurchasePrice)
                // {
                int points = 0;
                //  if (macd[0].Direction == MacdDirection.Down.ToString() && macd[2].Direction == MacdDirection.Down.ToString() && te.last > stats.LastPurchasePrice)
                //     points += 4;
                //   if (te.last >= (average + algoConfig.SellThreshold) && te.last > stats.LastPurchasePrice)
                //       points += 4;
                ////  if (te.last >= stats.LastPurchasePrice + algoConfig.SellThreshold)
                //      points += 4;

                //  if (macd[1].Direction == MacdDirection.Down.ToString() && macd[2].Direction == MacdDirection.Up.ToString() && te.last > stats.LastPurchasePrice)
                //      points += 4;

                //--------MACD Exit------------------------------------------

                if (!fdm.macdIntervalFast.isMacdGoingUp() || te.last >= (fdm.stats.LastPurchasePrice + fdm.algoConfig.FixedSellThreshold) /*|| fdm.algoConfig.UseStrictSellThreshold*/)
                    points += 4;


                //---------------------------------------------------

                //---11/14/2014-------------------------------------------------------------------- 

                //if (te.last >= stats.LastPurchasePrice + algoConfig.FixedSellThreshold)
                //    points += 4;
                //if (!macdIntervalSlow.isMacdGoingUp() && te.last >= stats.LastPurchasePrice)
                //    points += 4;
                //---------------------------------------------------------------------

                //Stop loss code here
                //if (macd[0].Close < macd[0].Open && te.last >= (stats.LastPurchasePrice - 0.05M) && stats.StatDate.AddMinutes(2) <= DateTime.Now)
                //     points += 4;


                if (points >= 4 && fdm.stats.Volume > 10)
                {
                    return (new TradeDecision { doTrade = true, useMarket = true });
                }

                else return (new TradeDecision { doTrade = false, useMarket = false });
            }
            else return (new TradeDecision { doTrade = false, useMarket = false });
            //  }
            // else return false;
            //}
            //  else return false;
            //  }
            //  else
            //  {
            //      return true;
            //  }
            ////    return (((te.last > (average + algoConfig.SellThreshold)) && (stats.Volume > 0) && te.last > stats.LastPurchasePrice + algoConfig.SellThreshold) ||( (stats.Volume > 0) && te.last > stats.LastPurchasePrice + algoConfig.FixedSellThreshold));

        }

        public TradeDecision ShouldStopLoss(FlipperDataModel fdm)
        {
            int points = 0;

            Ticker te = fdm.ticker.FirstOrDefault();
            
            switch (fdm.algoConfig.StopLossMode)
            {

                case StopMode.Any:


                    if (fdm.stats.Volume > 1)
                    {
                        //  foreach (Order o in fdm.openOrders.orders)
                        //   {
                        if (fdm.stats.LastPurchasePrice * fdm.algoConfig.StopLossPercentage >= te.buy || fdm.stats.LastOrderDate <= DateTime.Now.AddMinutes(-fdm.algoConfig.StopLossTime))
                        {
                            points += 4;
                        }


                        if (te.buy * fdm.algoConfig.StopLossPercentage >= fdm.stats.LastPurchasePrice || fdm.stats.LastOrderDate <= DateTime.Now.AddMinutes(-fdm.algoConfig.StopLossTime))
                        {
                            points += 4;
                        }


                        //     }
                    }


                    if ((fdm.stats.LastPurchasePrice * fdm.algoConfig.StopLossPercentage >= te.buy || ((te.buy * fdm.algoConfig.StopLossPercentage) >= fdm.stats.LastPurchasePrice && fdm.stats.LastSalePrice == 0) || fdm.stats.LastOrderDate <= DateTime.Now.AddMinutes(-fdm.algoConfig.StopLossTime) || points >= 4) && ((fdm.stats.Volume > 1) || (fdm.stats.Volume < 1 && fdm.stats.CurrentBalance < 5)))
                    {
                        return (new TradeDecision { doTrade = true, useMarket = true });
                    }
                    else return (new TradeDecision { doTrade = false, useMarket = false });
                //   break;                                                    
                case StopMode.Both:
                    if (fdm.stats.Volume > 1)
                    {
                        //  foreach (Order o in fdm.openOrders.orders)
                        //  {
                        if (fdm.stats.LastPurchasePrice * fdm.algoConfig.StopLossPercentage >= te.buy && fdm.stats.LastOrderDate <= DateTime.Now.AddMinutes(-fdm.algoConfig.StopLossTime))
                        {
                            points += 4;
                        }


                        if (te.buy * fdm.algoConfig.StopLossPercentage >= Convert.ToDecimal(fdm.stats.LastPurchasePrice) && fdm.stats.LastOrderDate <= DateTime.Now.AddMinutes(-fdm.algoConfig.StopLossTime))
                        {
                            points += 4;
                        }

                        //   }
                    }

                    if (((fdm.stats.LastPurchasePrice * fdm.algoConfig.StopLossPercentage >= te.buy || ((te.buy * fdm.algoConfig.StopLossPercentage) >= fdm.stats.LastPurchasePrice && fdm.stats.LastSalePrice == 0)) && fdm.stats.LastOrderDate <= DateTime.Now.AddMinutes(-fdm.algoConfig.StopLossTime) || points >= 4) && ((fdm.stats.Volume > 1) || (fdm.stats.Volume < 1 && fdm.stats.CurrentBalance < 5)))
                    {
                        return (new TradeDecision { doTrade = true, useMarket = true });
                    }
                    else return (new TradeDecision { doTrade = false, useMarket = false });

                //   break;                                                 
                case StopMode.None:
                    return (new TradeDecision { doTrade = false, useMarket = false });
                //   break;                                                 
                case StopMode.Percentage:
                    if (fdm.stats.Volume > 1)
                    {
                        //  foreach (Order o in fdm.openOrders.orders)
                        //  {
                        if ((fdm.stats.LastPurchasePrice * fdm.algoConfig.StopLossPercentage) >= te.buy)
                        {
                            points += 4;
                        }


                        if (te.buy * fdm.algoConfig.StopLossPercentage >= fdm.stats.LastPurchasePrice)
                        {
                            points += 4;
                        }



                        //    }
                    }

                    if ((Convert.ToDecimal(fdm.stats.LastPurchasePrice * fdm.algoConfig.StopLossPercentage) >= te.buy || ((te.buy * fdm.algoConfig.StopLossPercentage) >= fdm.stats.LastPurchasePrice && fdm.stats.LastSalePrice == 0) || points >= 4) && ((fdm.stats.Volume > 1) || (fdm.stats.Volume < 1 && fdm.stats.CurrentBalance < 5)))
                    {
                        return (new TradeDecision { doTrade = true, useMarket = true });
                    }
                    else return (new TradeDecision { doTrade = false, useMarket = false });
                //  break;
                case StopMode.Time:

                    if (fdm.stats.Volume > 1)
                    {
                        //  foreach (Order o in fdm.openOrders.orders)
                        //   {
                        if (fdm.stats.LastOrderDate <= DateTime.Now.AddMinutes(-fdm.algoConfig.StopLossTime))
                        {
                            points += 4;
                        }
                        //    }
                    }

                    if ((fdm.stats.LastOrderDate <= DateTime.Now.AddMinutes(-fdm.algoConfig.StopLossTime) || points >= 4) && ((fdm.stats.Volume > 1) || (fdm.stats.Volume < 1 && fdm.stats.CurrentBalance < 5)))
                    {
                        return (new TradeDecision { doTrade = true, useMarket = true });
                    }
                    else return (new TradeDecision { doTrade = false, useMarket = false });
                //   break;
            }


            //if (openOrders.orders != null)
            //{
            //    foreach (Order o in openOrders.orders)
            //    {
            //        if (Convert.ToDecimal(o.rate * .997) >= te.buy && stats.LastOrderDate <= DateTime.Now.AddMinutes(-10))
            //        {
            //            points += 4;
            //        }
            //    }
            //}


            //if (macd.isMacdPlummit() || macdIntervalFast.isMacdPlummit())
            //{
            //    points += 4;            
            //}


            //if ((Convert.ToDecimal(stats.LastPurchasePrice * .997M) >= te.buy) && (stats.Volume > 1) && stats.LastOrderDate <= DateTime.Now.AddMinutes(-10))
            //{
            //    points += 4;
            //}

            return (new TradeDecision { doTrade = false, useMarket = false }); //(points >= 4);



            //   return (((te.last > (average + algoConfig.SellThreshold)) && (stats.Volume > 0) && te.last > stats.LastPurchasePrice + algoConfig.SellThreshold) ||( (stats.Volume > 0) && te.last > stats.LastPurchasePrice + algoConfig.FixedSellThreshold));

        }

    }
}

