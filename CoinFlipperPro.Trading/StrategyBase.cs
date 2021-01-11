using CoinFlipperPro.DataAccess;
using CoinFlipperPro.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoinFlipperPro.Trading
{
    public abstract class StrategyBase : ITradingStrategy
    {
        protected decimal currentHighPrice;
        public virtual  Boolean IsStopLossInProgress { get; set; }

        public virtual Model.AccountStatus Buy(Model.FlipperDataModel fdm, Model.TradeDecision td, Model.ITradeApi tradeApi)
        {
            if (!IsStopLossInProgress && td.doTrade)
            {
               if (Convert.ToDouble(fdm.accountInfo.info.funds.freezed.cny) < 1 && Convert.ToDecimal(fdm.accountInfo.info.funds.free.ltc) - fdm.algoConfig.ReservedCoin < 1 && Convert.ToDecimal(fdm.accountInfo.info.funds.freezed.ltc) < 1)
                {
                    double buyPrice;
                    double freeFunds;

                    freeFunds = Convert.ToDouble(fdm.accountInfo.info.funds.free.cny);
                    decimal calculatedMarketPrice = fdm.depth.ActualMarketAsk(freeFunds.ConvertToDecimal()).Price;
                    if (td.price != null && td.price != 0)
                    {
                        buyPrice = Convert.ToDouble(td.price);
                    }
                    else
                    {
                        //Best
                        buyPrice = Convert.ToDouble((Math.Floor((fdm.macdIntervalFast[0].AveragePrice) * 100) / 100) + fdm.algoConfig.BuyThreshold);
                    }

                    decimal amountToBuy = Convert.ToDecimal(freeFunds) / Convert.ToDecimal(buyPrice);



                    if (td.useMarket)
                    {
                       
                        buyPrice = (calculatedMarketPrice > 0 ? calculatedMarketPrice.ConvertToDouble() : buyPrice);
                        fdm.lastOrderResult = tradeApi.Trade(freeFunds, buyPrice.ConvertToDecimal(), TradeType.BuyMarket, fdm.accountInfo);
                    }
                    else
                    {
                        fdm.lastOrderResult = tradeApi.Trade(buyPrice, amountToBuy, TradeType.Buy, fdm.accountInfo);
                    }

                    fdm.stats.LastPurchasePrice = Convert.ToDecimal(buyPrice);
                    fdm.stats.LastSalePrice = 0;
                    fdm.stats.StatDate = DateTime.Now;
                    fdm.stats.LastOrderDate = DateTime.Now;
                    fdm.stats.CurrentBalance = 0;
                    fdm.stats.Volume = fdm.algoConfig.ReservedCoin;
                    fdm.stats.Source = TransactionSource.LiveData;
                    fdm.stats.IsSimulator = false;
                    currentHighPrice = fdm.macdIntervalFast[0].CompareShortPrice;
                    TickerDAL.InsertTransaction(fdm.stats);
                    //     LoadAccountInfo(false);
                }
            }

            return fdm.stats;
        }

        public virtual Model.AccountStatus Sell(Model.FlipperDataModel fdm, Model.TradeDecision td, Model.ITradeApi tradeApi)
        {
            if (!IsStopLossInProgress && td.doTrade)
            {
                if (Convert.ToDouble(fdm.accountInfo.info.funds.freezed.cny) < 1 && Convert.ToDecimal(fdm.accountInfo.info.funds.free.cny) < 15 && Convert.ToDecimal(fdm.accountInfo.info.funds.freezed.ltc) < 1)
                {
                    // double sellPrice = Convert.ToDouble(stats.LastPurchasePrice) + Convert.ToDouble(algoConfig.FixedSellThreshold);
                    // Convert.ToDouble(t.Data.last);
                    double freeFunds = Convert.ToDouble(fdm.accountInfo.info.funds.free.cny);
                    decimal amountToSell = Convert.ToDecimal(fdm.accountInfo.info.funds.free.ltc) - fdm.algoConfig.ReservedCoin;


                    fdm.stats.StatDate = DateTime.Now;
                    fdm.stats.LastOrderDate = DateTime.Now;

                    double sellPrice;


                    if (td.useMarket)
                    {
                        fdm.lastOrderResult = tradeApi.Trade(null, amountToSell, TradeType.SellMarket, fdm.accountInfo);
                        fdm.stats.LastSalePrice = fdm.ticker.FirstOrDefault<Ticker>().buy;
                    }
                    else
                    {
                        #region sellPriceCalculations
                        if (td.price != null && td.price > 0)
                        {
                            sellPrice = Convert.ToDouble(td.price);
                        }
                        else
                        {
                            if (fdm.ticker.FirstOrDefault<Ticker>().last >= fdm.stats.LastPurchasePrice + fdm.algoConfig.FixedSellThreshold && !fdm.algoConfig.UseStrictSellThreshold)
                            {
                                sellPrice = Convert.ToDouble(fdm.ticker.FirstOrDefault<Ticker>().last);
                            }
                            else if (fdm.macdIntervalFast[1].SMAHigh > fdm.ticker.FirstOrDefault<Ticker>().last && !fdm.algoConfig.UseStrictSellThreshold)
                            {
                                sellPrice = Convert.ToDouble(fdm.macdIntervalFast[1].SMAHigh);
                            }
                            else
                            {
                                // if (!macdIntervalFast.isMacdPositive()) 
                                // {
                                //     sellPrice = Convert.ToDouble(stats.LastPurchasePrice + .01M);
                                //  }
                                if ((Math.Floor(fdm.macdIntervalFast[0].Resistance1 * 100) / 100) > fdm.stats.LastPurchasePrice)
                                {
                                    sellPrice = Convert.ToDouble(Math.Floor(fdm.macdIntervalFast[0].Resistance1 * 100) / 100);
                                }
                                else
                                {
                                    sellPrice = Convert.ToDouble(fdm.stats.LastPurchasePrice + fdm.algoConfig.FixedSellThreshold);
                                }

                            }
                        }
                        #endregion

                        fdm.lastOrderResult = tradeApi.Trade(sellPrice, amountToSell, TradeType.Sell, fdm.accountInfo);
                        fdm.stats.LastSalePrice = Convert.ToDecimal(sellPrice);
                    }

                    fdm.stats.CurrentBalance = 0;
                    fdm.stats.Volume = fdm.algoConfig.ReservedCoin;
                    fdm.stats.TransactionType = TransactionType.Sell;
                    fdm.stats.Source = TransactionSource.LiveData;
                    fdm.stats.IsSimulator = false;
                    TickerDAL.InsertTransaction(fdm.stats);
                    //  LoadAccountInfo(false);
                }



            }


            Ticker te = fdm.ticker.FirstOrDefault();


            return fdm.stats;
        }

        public virtual Model.AccountStatus StopLoss(Model.FlipperDataModel fdm, Model.TradeDecision td, Model.ITradeApi tradeApi)
        {
            if (td.doTrade || IsStopLossInProgress)
            {
                if (!IsStopLossInProgress)
                {
                    IsStopLossInProgress = true;
                    if (fdm.openOrders.orders.Count > 0)
                    {

                        foreach (Order o in fdm.openOrders.orders)
                        {
                            tradeApi.CancelOrder(o.orders_id);

                        }
                    }
                }
                if (Convert.ToDecimal(fdm.accountInfo.info.funds.free.ltc) - fdm.algoConfig.ReservedCoin > 1)
                {
                    tradeApi.Trade(null, Convert.ToDecimal(fdm.accountInfo.info.funds.free.ltc) - fdm.algoConfig.ReservedCoin, TradeType.SellMarket, fdm.accountInfo);
                }



                if ((Convert.ToDecimal(fdm.accountInfo.info.funds.freezed.ltc) < 1 || (fdm.stats.LastOrderDate < DateTime.Now.AddMinutes(-5))) && (Convert.ToDecimal(fdm.accountInfo.info.funds.free.ltc) - fdm.algoConfig.ReservedCoin) < 1 && Convert.ToDecimal(fdm.accountInfo.info.funds.freezed.cny) < 1 && IsStopLossInProgress)
                {
                    IsStopLossInProgress = false;
                    fdm.stats.CurrentBalance = 0;
                    fdm.stats.Volume = fdm.algoConfig.ReservedCoin;
                    fdm.stats.TransactionType = TransactionType.Sell;
                    fdm.stats.Source = TransactionSource.LiveData;
                    fdm.stats.IsSimulator = false;
                    TickerDAL.InsertTransaction(fdm.stats);
                }


                TickerDAL.InsertTransaction(fdm.stats);
            }



            return fdm.stats;
        }

        public virtual Model.TradeDecision ShouldBuy(Model.FlipperDataModel fdm)
        {
            return ShouldBuyImpl(fdm);
        }

        public virtual Model.TradeDecision ShouldSell(Model.FlipperDataModel fdm)
        {
            return ShouldSellImpl(fdm);
        }

        public virtual Model.TradeDecision ShouldStopLoss(Model.FlipperDataModel fdm)
        {
            int points = 0;

            Ticker te = fdm.ticker.FirstOrDefault();
            decimal calculatedMarketPrice = fdm.depth.ActualMarketBid(fdm.stats.Volume - fdm.algoConfig.ReservedCoin).Price;

            switch (fdm.algoConfig.StopLossMode)
            {

                case StopMode.Any:


                    if (fdm.openOrders.orders != null)
                    {
                        foreach (Order o in fdm.openOrders.orders)
                        {
                            if (o.rate * fdm.algoConfig.StopLossPercentage >= calculatedMarketPrice || fdm.stats.LastOrderDate <= DateTime.Now.AddMinutes(-fdm.algoConfig.StopLossTime))
                            {
                                points += 4;
                            }


                            if (calculatedMarketPrice * fdm.algoConfig.StopLossPercentage >= o.rate || fdm.stats.LastOrderDate <= DateTime.Now.AddMinutes(-fdm.algoConfig.StopLossTime))
                            {
                                points += 4;
                            }


                        }
                    }


                    if ((fdm.stats.LastPurchasePrice * fdm.algoConfig.StopLossPercentage >= calculatedMarketPrice || ((calculatedMarketPrice * fdm.algoConfig.StopLossPercentage) >= fdm.stats.LastPurchasePrice && fdm.stats.LastSalePrice == 0) || fdm.stats.LastOrderDate <= DateTime.Now.AddMinutes(-fdm.algoConfig.StopLossTime) || points >= 4) && ((fdm.stats.Volume > 1) || (fdm.stats.Volume < 1 && fdm.stats.CurrentBalance < 5)))
                    {
                        return (new TradeDecision { doTrade = true, useMarket = true });
                    }
                    else return (new TradeDecision { doTrade = false, useMarket = false });
                //   break;                                                    
                case StopMode.Both:
                    if (fdm.openOrders.orders != null)
                    {
                        foreach (Order o in fdm.openOrders.orders)
                        {
                            if (o.rate * fdm.algoConfig.StopLossPercentage >= calculatedMarketPrice && fdm.stats.LastOrderDate <= DateTime.Now.AddMinutes(-fdm.algoConfig.StopLossTime))
                            {
                                points += 4;
                            }


                            if (calculatedMarketPrice * fdm.algoConfig.StopLossPercentage >= Convert.ToDecimal(o.rate) && fdm.stats.LastOrderDate <= DateTime.Now.AddMinutes(-fdm.algoConfig.StopLossTime))
                            {
                                points += 4;
                            }

                        }
                    }

                    if (((fdm.stats.LastPurchasePrice * fdm.algoConfig.StopLossPercentage >= calculatedMarketPrice || ((calculatedMarketPrice * fdm.algoConfig.StopLossPercentage) >= fdm.stats.LastPurchasePrice && fdm.stats.LastSalePrice == 0)) && fdm.stats.LastOrderDate <= DateTime.Now.AddMinutes(-fdm.algoConfig.StopLossTime) || points >= 4) && ((fdm.stats.Volume > 1) || (fdm.stats.Volume < 1 && fdm.stats.CurrentBalance < 5)))
                    {
                        return (new TradeDecision { doTrade = true, useMarket = true });
                    }
                    else return (new TradeDecision { doTrade = false, useMarket = false });

                //   break;                                                 
                case StopMode.None:
                    return (new TradeDecision { doTrade = false, useMarket = false });
                //   break;                                                 
                case StopMode.Percentage:
                    if (fdm.openOrders.orders != null)
                    {
                        foreach (Order o in fdm.openOrders.orders)
                        {
                            if ((o.rate * fdm.algoConfig.StopLossPercentage) >= calculatedMarketPrice)
                            {
                                points += 4;
                            }


                            if (calculatedMarketPrice * fdm.algoConfig.StopLossPercentage >= o.rate)
                            {
                                points += 4;
                            }



                        }
                    }

                    if ((Convert.ToDecimal(fdm.stats.LastPurchasePrice * fdm.algoConfig.StopLossPercentage) >= calculatedMarketPrice || ((calculatedMarketPrice * fdm.algoConfig.StopLossPercentage) >= fdm.stats.LastPurchasePrice && fdm.stats.LastSalePrice == 0) || points >= 4) && ((fdm.stats.Volume > 1) || (fdm.stats.Volume < 1 && fdm.stats.CurrentBalance < 5)))
                    {
                        return (new TradeDecision { doTrade = true, useMarket = true });
                    }
                    else return (new TradeDecision { doTrade = false, useMarket = false });
                //  break;
                case StopMode.Time:

                    if (fdm.openOrders.orders != null)
                    {
                        foreach (Order o in fdm.openOrders.orders)
                        {
                            if (fdm.stats.LastOrderDate <= DateTime.Now.AddMinutes(-fdm.algoConfig.StopLossTime))
                            {
                                points += 4;
                            }
                        }
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
            //        if (Convert.ToDecimal(o.rate * .997) >= calculatedMarketPrice && stats.LastOrderDate <= DateTime.Now.AddMinutes(-10))
            //        {
            //            points += 4;
            //        }
            //    }
            //}


            //if (macd.isMacdPlummit() || macdIntervalFast.isMacdPlummit())
            //{
            //    points += 4;            
            //}


            //if ((Convert.ToDecimal(stats.LastPurchasePrice * .997M) >= calculatedMarketPrice) && (stats.Volume > 1) && stats.LastOrderDate <= DateTime.Now.AddMinutes(-10))
            //{
            //    points += 4;
            //}

            return (new TradeDecision { doTrade = false, useMarket = false }); //(points >= 4);



        }

        abstract protected Model.TradeDecision ShouldBuyImpl(Model.FlipperDataModel fdm);
     
         abstract protected Model.TradeDecision ShouldSellImpl(Model.FlipperDataModel fdm);

    }
}
