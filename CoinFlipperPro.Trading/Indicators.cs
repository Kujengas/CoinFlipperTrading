using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinFlipperPro.Model;
using TicTacTec.TA.Library;

namespace CoinFlipperPro.Trading
{
    public static class Indicators
    {

        /// <summary>
        /// Takes a List of DateTime, double (date, closing price) and returns a List of DateTime, double
        /// where double is the MACD (12,26) value. 
        /// Caller must ensure minimum number of data points, or this method will throw.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<Macd> CalculateMACD(List<CandleStickEntity> data)
        {
            decimal emaShort, emaLong, yesterdayEMAShort, yesterdayEMALong, yesterdayEMASignal, emaSignal, yesterdayHist;
            int longPeriod, shortPeriod, signalPeriod;

            longPeriod =26;// 10;
            shortPeriod = 12;//3;
            signalPeriod = 9;// 16;

            if (data.Count > longPeriod)
            {
                // Compute simple moving averages to start with for the yesterdayEMA values.
                decimal sum = 0.0;
                for (int i = 0; i < shortPeriod; i++)
                {
                    sum += data[i].ClosePrice;
                }

                yesterdayEMAShort = sum / shortPeriod;

                sum = 0.0;

                for (int i = 0; i < longPeriod; i++)
                {
                    sum += data[i].ClosePrice;
                }

                yesterdayEMALong = sum / longPeriod;



                for (int i = 0; i < signalPeriod; i++)
                {
                    sum += data[i].ClosePrice;
                }

                //   yesterdayEMA9 = sum / 9.0;

                List<Macd> macdValues = new List<Macd>();



                // For MACD(12.Long), we need to calculate a 12-day EMA and a Long-day EMA, then subtract the Long-day
                // from the 12-day. 
                for (int i = 0; i < data.Count; i++)
                {
                    // Call the EMA calculation for each EMA.
                    emaShort = CalculateEMA(data[i].ClosePrice, shortPeriod, yesterdayEMAShort);
                    emaLong = CalculateEMA(data[i].ClosePrice, longPeriod, yesterdayEMALong);
                    //  ema9 = CalculateEMA(data[i].Item2, 9, yesterdayEMA9);


                    // Store the result for this data point.
                    macdValues.Add(new Macd { PriceDate = data[i].CloseDataEntryDate, EMAShort = emaShort.ConvertToDecimal(), EMALong = emaLong.ConvertToDecimal(), Difference = (emaShort - emaLong).ConvertToDecimal(), EMASignal = 0, Histogram = 0, Close = data[i].ClosePrice.ConvertToDecimal(), High = data[i].HighPrice.ConvertToDecimal(), Low = data[i].LowPrice.ConvertToDecimal(), Open = data[i].OpenPrice.ConvertToDecimal(), Average = data[i].AveragePrice.ConvertToDecimal() });

                    // Update yesterdayEMA.
                    yesterdayEMAShort = emaShort;
                    yesterdayEMALong = emaLong;
                    //  yesterdayEMA9 = ema9;
                }


                sum = 0.0M;
                for (int i = 0; i < signalPeriod; i++)
                {
                    sum += Convert.ToDouble(macdValues[i].Difference);
                }

                yesterdayEMASignal = sum / signalPeriod;
                yesterdayHist = 0.0M;
                decimal hist = 0.0M;

                for (int i = 0; i < macdValues.Count; i++)
                {
                    // Call the EMA calculation for each EMA.

                    emaSignal = Convert.ToDouble(CalculateEMA(Convert.ToDouble(macdValues[i].Difference), signalPeriod, yesterdayEMASignal));


                    // Store the result for this data point.
                    macdValues[i].EMASignal = emaSignal;
                    hist = macdValues[i].Difference - emaSignal;
                    macdValues[i].Histogram = hist;

                    if (i == 0)
                    {
                        macdValues[i].Direction = MacdDirection.None.ToString();
                    }
                    else
                    {
                        if (hist > yesterdayHist)
                        {
                            macdValues[i].Direction = MacdDirection.Up.ToString();
                        }
                        else if (hist < yesterdayHist)
                        {
                            macdValues[i].Direction = MacdDirection.Down.ToString();
                        }
                        else if (hist == yesterdayHist)
                        {
                            macdValues[i].Direction = MacdDirection.Sideways.ToString();
                        }
                    }


                    yesterdayHist = hist;
                    yesterdayEMASignal = emaSignal;
                }
                List<string> lst = getpattern(macdValues);

                for (int i = 0; i < lst.Count;i++)
                {
                    if (!String.IsNullOrEmpty(lst[i])) 
                    {
                        macdValues[i].CandlePattern = lst[i];
                    
                    }
                }



                    return macdValues.OrderByDescending(x => x.PriceDate).ToList();
            }
            else return new List<Macd>();

        }

        private static double CalculateEMA(double todaysPrice, double numberOfDays, double EMAYesterday)
        {
            double alpha = 2.0 / (numberOfDays + 1);

            return EMAYesterday + alpha * (todaysPrice - EMAYesterday);
        }

        private static List<string> getpattern(List<Macd> macd)
        {

            double[] open = (from Macd m in macd select Convert.ToDouble(m.Open)).ToArray<double>();
            double[] close = (from Macd m in macd select Convert.ToDouble(m.Close)).ToArray<double>();
            double[] high = (from Macd m in macd select Convert.ToDouble(m.High)).ToArray<double>();
            double[] low = (from Macd m in macd select Convert.ToDouble(m.Low)).ToArray<double>();
            List<String> patterns = (from Macd m in macd select m.CandlePattern).ToList<string>();
            
            int beginIndex, endIndex;
            int[] isPattern = new int[macd.Count];
            List<string> lst = new List<string>();
            TicTacTec.TA.Library.Core.CdlHammer(0, macd.Count - 1, open, high, low, close, out beginIndex, out endIndex, isPattern);
            for (int i=0; i < isPattern.Count();i++ )
            {
                if (isPattern[i] != 0)
                    patterns[i] = (!String.IsNullOrEmpty(patterns[i])) ? patterns[i] + ",Hammer(" + isPattern[i] + ")" : "Hammer(" + isPattern[i] + ")";
            }
            Array.Clear(isPattern,0, isPattern.Length);

            TicTacTec.TA.Library.Core.CdlHangingMan(0, macd.Count - 1, open, high, low, close, out beginIndex, out endIndex, isPattern);
            for (int i=0; i < isPattern.Count(); i++)
            {
                if (isPattern[i] != 0)
                    patterns[i] = (!String.IsNullOrEmpty(patterns[i])) ? patterns[i] + ",Hanging Man(" + isPattern[i] + ")" : "Hanging Man(" + isPattern[i] + ")";
            }
            Array.Clear(isPattern, 0, isPattern.Length);


            TicTacTec.TA.Library.Core.CdlDojiStar(0, macd.Count - 1, open, high, low, close, out beginIndex, out endIndex, isPattern);
            for (int i = 0; i < isPattern.Count(); i++)
            {
                if (isPattern[i] != 0)
                    patterns[i] = (!String.IsNullOrEmpty(patterns[i])) ? patterns[i] + ",Doji Star(" + isPattern[i] + ")" : "Doji Star(" + isPattern[i] + ")";
            }
            Array.Clear(isPattern, 0, isPattern.Length);

            TicTacTec.TA.Library.Core.CdlDoji(0, macd.Count - 1, open, high, low, close, out beginIndex, out endIndex, isPattern);
            for (int i = 0; i < isPattern.Count(); i++)
            {
                if (isPattern[i] != 0)
                    patterns[i] = (!String.IsNullOrEmpty(patterns[i])) ? patterns[i] + ",Doji(" + isPattern[i] + ")" : "Doji(" + isPattern[i] + ")";
            }
            Array.Clear(isPattern, 0, isPattern.Length);

            TicTacTec.TA.Library.Core.CdlInvertedHammer(0, macd.Count - 1, open, high, low, close, out beginIndex, out endIndex, isPattern);
            for (int i = 0; i < isPattern.Count(); i++)
            {
                if (isPattern[i] != 0)
                    patterns[i] = (!String.IsNullOrEmpty(patterns[i])) ? patterns[i] + ",Inverted Hammer(" + isPattern[i] + ")" : "Inverted Hammer(" + isPattern[i] + ")";
            }
            Array.Clear(isPattern, 0, isPattern.Length);


            TicTacTec.TA.Library.Core.CdlBeltHold(0, macd.Count - 1, open, high, low, close, out beginIndex, out endIndex, isPattern);
            for (int i = 0; i < isPattern.Count(); i++)
            {
                if (isPattern[i] != 0)
                    patterns[i] = (!String.IsNullOrEmpty(patterns[i])) ? patterns[i] + ",Belt hold line (" + isPattern[i] + ")" : "Belt hold line(" + isPattern[i] + ")";
            }
            Array.Clear(isPattern, 0, isPattern.Length);

            TicTacTec.TA.Library.Core.CdlShootingStar(0, macd.Count - 1, open, high, low, close, out beginIndex, out endIndex, isPattern);
            for (int i = 0; i < isPattern.Count(); i++)
            {
                if (isPattern[i] != 0)
                    patterns[i] = (!String.IsNullOrEmpty(patterns[i])) ? patterns[i] + ",Shooting Star(" + isPattern[i] + ")" : "Shooting Star(" + isPattern[i] + ")";
            }
            Array.Clear(isPattern, 0, isPattern.Length);

            TicTacTec.TA.Library.Core.CdlEngulfing(0, macd.Count - 1, open, high, low, close, out beginIndex, out endIndex, isPattern);
            for (int i = 0; i < isPattern.Count(); i++)
            {
                if (isPattern[i] != 0)
                    patterns[i] = (!String.IsNullOrEmpty(patterns[i])) ? patterns[i] + ",Engulfing(" + isPattern[i] + ")" : "Engulfing(" + isPattern[i] + ")";
            }
            Array.Clear(isPattern, 0, isPattern.Length);

          


            return patterns;



        }


    }
}
