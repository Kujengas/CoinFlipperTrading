using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinFlipperPro.Model;
using Newtonsoft.Json;

namespace CoinFlipperPro.DataAccess
{
    public class TickerDAL
    {


        public static string ConnectionString()
        {
            string selectedDB = ConfigurationManager.AppSettings["db"].ToString();
            string connectionString = ConfigurationManager.ConnectionStrings[selectedDB].ConnectionString;


            //  return "Server=Rico-NOTEBOOK01\\SQL2008R2;Database=CoinFlipperChina;Trusted_Connection=True;";
            //  return "Server=Rico-NOTEBOOK01\\SQL2008R2;Database=CoinFlipperChina;Trusted_Connection=True;";

            return connectionString;
        }

        public static void InsertTickerEntry(List<TickerEntry> lte)
        {

            using (var conn = new SqlConnection(ConnectionString()))
            using (var cmd = new SqlCommand("usp_TickerInsert", conn) { CommandType = CommandType.StoredProcedure })
            {
                foreach (TickerEntry te in lte)
                {
                    conn.Open();
                    cmd.Parameters.Add(new SqlParameter("@TickerBuy", te.Data.buy));
                    cmd.Parameters.Add(new SqlParameter("@TickerHigh", te.Data.high));
                    cmd.Parameters.Add(new SqlParameter("@TickerLast", te.Data.last));
                    cmd.Parameters.Add(new SqlParameter("@TickerLow", te.Data.low));
                    cmd.Parameters.Add(new SqlParameter("@TickerSell", te.Data.sell));
                    cmd.Parameters.Add(new SqlParameter("@TickerVol", te.Data.vol));
                    cmd.Parameters.Add(new SqlParameter("@PriceDate", te.GetTickerDateTime()));

                    cmd.ExecuteNonQuery();
                }

            }


        }

        public static void InsertTransaction(AccountStatus stat)
        {

            using (var conn = new SqlConnection(ConnectionString()))
            using (var cmd = new SqlCommand("usp_TransactionInsert", conn) { CommandType = CommandType.StoredProcedure })
            {

                conn.Open();
                cmd.Parameters.Add(new SqlParameter("@TansactionType", stat.TransactionType.ToString()));
                cmd.Parameters.Add(new SqlParameter("@Volume", stat.Volume));
                cmd.Parameters.Add(new SqlParameter("@StartingBalance", stat.StartingBalance));
                cmd.Parameters.Add(new SqlParameter("@LastPurchasePrice", stat.LastPurchasePrice));
                cmd.Parameters.Add(new SqlParameter("@LastSalePrice", stat.LastSalePrice));
                cmd.Parameters.Add(new SqlParameter("@CurrentBalance", stat.CurrentBalance));
                cmd.Parameters.Add(new SqlParameter("@TransactionSource", stat.Source.ToString()));
                cmd.Parameters.Add(new SqlParameter("@IsSimulator", true));

                cmd.ExecuteNonQuery();


            }


        }


        public static List<Ticker> GetTicker(int time, long tickerId)
        {
            bool isBackTest = false;
            int? startTick = null;
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(ConnectionString()))

            using (var cmd = new SqlCommand("usp_TickerGet", conn) { CommandType = CommandType.StoredProcedure })
            {

                conn.Open();
                cmd.Parameters.Add(new SqlParameter("@Hours", time));
                cmd.Parameters.Add(new SqlParameter("@BackTestTickerId", tickerId));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

            }


            return (from DataRow row in dt.Rows
                    select
                        new Ticker { offset = Convert.ToDecimal(row["Offset"]), offset12 = Convert.ToDecimal(row["Offset12"]), offset24 = Convert.ToDecimal(row["Offset24"]), avg = Convert.ToDecimal(row["TickerAvg"]), avg12 = Convert.ToDecimal(row["TickerAvg12"]), avg24 = Convert.ToDecimal(row["TickerAvg24"]), DataEntryDate = Convert.ToDateTime(row["DataEntryDate"]), priceId = Convert.ToInt64(row["PriceId"]), last = Convert.ToDecimal(row["TickerLast"]), buy = Convert.ToDecimal(row["TickerBuy"]), high = Convert.ToDecimal(row["TickerHigh"]), low = Convert.ToDecimal(row["TickerLow"]), sell = Convert.ToDecimal(row["TickerSell"]), vol = Convert.ToDecimal(row["TickerVol"]) }
             ).ToList<Ticker>();

        }

        public static MarketDepth GetMarketDepth()
        {
           
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(ConnectionString()))

            using (var cmd = new SqlCommand("usp_OrderBookGet", conn) { CommandType = CommandType.StoredProcedure })
            {

                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

            }


           var tmpMarketDepth=(ApiMarketDepth)JsonConvert.DeserializeObject<ApiMarketDepth>(dt.Rows[0]["OrderBookData"].ToString());
            return tmpMarketDepth.ToMarketDepth();

        }


        public static DataTable GetRecentSales(int time)
        {
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(ConnectionString()))
            using (var cmd = new SqlCommand("usp_RecentSalesGet", conn) { CommandType = CommandType.StoredProcedure })
            {

                conn.Open();
                cmd.Parameters.Add(new SqlParameter("@Hours", time));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

            }
            return dt;

        }




        public static Ticker GetLastTicker(long tickerId)
        {
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(ConnectionString()))
            using (var cmd = new SqlCommand("usp_LastTickerGet", conn) { CommandType = CommandType.StoredProcedure })
            {

                conn.Open();
                cmd.Parameters.Add(new SqlParameter("@BackTestTickerId", tickerId));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }


            return (from DataRow row in dt.Rows
                    select
                        new Ticker { offset = Convert.ToDecimal(row["Offset"]), offset12 = Convert.ToDecimal(row["Offset12"]), offset24 = Convert.ToDecimal(row["Offset24"]), avg = Convert.ToDecimal(row["TickerAvg"]), avg12 = Convert.ToDecimal(row["TickerAvg12"]), avg24 = Convert.ToDecimal(row["TickerAvg24"]), DataEntryDate = Convert.ToDateTime(row["DataEntryDate"]), priceId = Convert.ToInt64(row["PriceId"]), last = Convert.ToDecimal(row["TickerLast"]), buy = Convert.ToDecimal(row["TickerBuy"]), high = Convert.ToDecimal(row["TickerHigh"]), low = Convert.ToDecimal(row["TickerLow"]), sell = Convert.ToDecimal(row["TickerSell"]), vol = Convert.ToDecimal(row["TickerVol"]) }
            ).FirstOrDefault<Ticker>();

        }


        public static List<CandleStickEntity> GetEMAList(long tickerId)
        {

            var lst = new List<CandleStickEntity>();
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(ConnectionString()))
            using (var cmd = new SqlCommand("usp_TickerCloseByIntervalGet", conn) { CommandType = CommandType.StoredProcedure })
            {

                conn.Open();
                cmd.Parameters.Add(new SqlParameter("@Hours", 5));
                cmd.Parameters.Add(new SqlParameter("@Interval", 1));
                cmd.Parameters.Add(new SqlParameter("@BackTestTickerId", tickerId));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            lst = (from DataRow r in dt.Rows
                   select new CandleStickEntity { CloseDataEntryDate = Convert.ToDateTime(r["CloseDataEntryDate"]), AveragePrice = Convert.ToDouble(r["AveragePrice"]), ClosePrice = Convert.ToDouble(r["ClosePrice"]), HighPrice = Convert.ToDouble(r["HighPrice"]), LowPrice = Convert.ToDouble(r["LowPrice"]), OpenPrice = Convert.ToDouble(r["OpenPrice"]) }).ToList();



            return lst;


        }


        public static List<CandleStickEntity> GetEMAListInterval(long tickerId)
        {

            var lst = new List<CandleStickEntity>();
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(ConnectionString()))
            using (var cmd = new SqlCommand("usp_TickerCloseByIntervalGet", conn) { CommandType = CommandType.StoredProcedure })
            {

                conn.Open();
                cmd.Parameters.Add(new SqlParameter("@Hours", 7));
                cmd.Parameters.Add(new SqlParameter("@Interval", 15));
                cmd.Parameters.Add(new SqlParameter("@BackTestTickerId", tickerId));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            lst = (from DataRow r in dt.Rows
                   select new CandleStickEntity { CloseDataEntryDate = Convert.ToDateTime(r["CloseDataEntryDate"]), AveragePrice = Convert.ToDouble(r["AveragePrice"]), ClosePrice = Convert.ToDouble(r["ClosePrice"]), HighPrice = Convert.ToDouble(r["HighPrice"]), LowPrice = Convert.ToDouble(r["LowPrice"]), OpenPrice = Convert.ToDouble(r["OpenPrice"]) }).ToList();



            return lst;


        }



        public static List<TickerIndex> GetTickerIds()
        {

            var lst = new List<TickerIndex>();
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(ConnectionString()))
            using (var cmd = new SqlCommand("usp_TickerIdsGet", conn) { CommandType = CommandType.StoredProcedure })
            {

                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            lst = (from DataRow r in dt.Rows
                   select new TickerIndex { DataEntryDate = Convert.ToDateTime(r["DataEntryDate"]), TickerId = Convert.ToInt64(r["TickerId"]) }).ToList();



            return lst;


        }

        public static DataTable GetLastTransaction()
        {
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(ConnectionString()))
            using (var cmd = new SqlCommand("usp_LastTransactionGet", conn) { CommandType = CommandType.StoredProcedure })
            {

                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;

        }






        public static DataTable GetTransactions(int time)
        {
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(ConnectionString()))
            using (var cmd = new SqlCommand("usp_TransactionGet", conn) { CommandType = CommandType.StoredProcedure })
            {

                conn.Open();
                cmd.Parameters.Add(new SqlParameter("@Hours", time));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

            }
            return dt;

        }

        //public static List<TickerEntry> GetLastTickerEntity(long tickerId)
        //{

        //    var dt = GetLastTicker(tickerId);
        //    var entry = new List<TickerEntry>();
        //    var te = new TickerEntry
        //    {
        //        Date = (Int32)Convert.ToDateTime(dt.Rows[0]["DataEntryDate"]).ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
        //        Data = new Ticker { last = Convert.ToDecimal(dt.Rows[0]["TickerLast"]), buy = Convert.ToDecimal(dt.Rows[0]["TickerBuy"]), high = Convert.ToDecimal(dt.Rows[0]["TickerHigh"]), low = Convert.ToDecimal(dt.Rows[0]["TickerLow"]), sell = Convert.ToDecimal(dt.Rows[0]["TickerSell"]), vol = Convert.ToDecimal(dt.Rows[0]["TickerVol"]) }
        //    };
        //    entry.Add(te);
        //    return entry;
        //}

        public static List<FlipperCandlestick> GetMacdInterval(int hours, int interval, long tickerid)
        {
            return GetMacdInterval(hours, interval, 26, 12, 9, tickerid);
        }


        public static List<FlipperCandlestick> GetMacdInterval(int hours, int interval, int longTime, int shortTime, int signalTime, long tickerid)
        {
            var lst = new List<FlipperCandlestick>();
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(ConnectionString()))
            using (var cmd = new SqlCommand("usp_KlineMACDByIntervalGet", conn) { CommandType = CommandType.StoredProcedure })
           // using (var cmd = new SqlCommand("usp_MACDByIntervalGet", conn) { CommandType = CommandType.StoredProcedure })
            {

                conn.Open();
                cmd.Parameters.Add(new SqlParameter("@Hours ", hours));
                cmd.Parameters.Add(new SqlParameter("@Interval ", interval));
                cmd.Parameters.Add(new SqlParameter("@BackTestTickerId ", tickerid));
                cmd.Parameters.Add(new SqlParameter("@LongTime ", longTime));
                cmd.Parameters.Add(new SqlParameter("@ShortTime ", shortTime));
                cmd.Parameters.Add(new SqlParameter("@SignalTime", signalTime));
                cmd.Parameters.Add(new SqlParameter("@HighLowTime", 2));
                cmd.Parameters.Add(new SqlParameter("@RSITime", 14));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                lst = (from DataRow row in dt.Rows

                       select new FlipperCandlestick
                       {
                           PriceId = Convert.ToInt64(row["PriceId"]),
                           IntervalTime=row["IntervalTime"].ToString(),
                           //AveragePrice = Convert.ToDecimal(row["AveragePrice"]), 
                           AveragePrice = Convert.ToDecimal(row["Volume"]),
                           ClosePrice = Convert.ToDecimal(row["ClosePrice"]), 
                           IntervalDate = Convert.ToDateTime(row["IntervalDate"]),
                           OpenPrice = Convert.ToDecimal(row["OpenPrice"]),
                           LowPrice = Convert.ToDecimal(row["LowPrice"]),
                           HighPrice = Convert.ToDecimal(row["HighPrice"]),
                           CloseDataEntryDate = Convert.ToDateTime(row["CloseDataEntryDate"]),
                           SMALow = Convert.ToDecimal(row["SMALow"]),
                           SMAHigh = Convert.ToDecimal(row["SMAHigh"]),
                           SMAShort = Convert.ToDecimal(row["SMAShort"]),
                           SMALong = Convert.ToDecimal(row["SMALong"]),
                           SMASignal = Convert.ToDecimal(row["SMASignal"]),
                           Support1 = Convert.ToDecimal(row["Support1"]),
                           Resistance1 = Convert.ToDecimal(row["Resistance1"]),
                           Support2 = Convert.ToDecimal(row["Support2"]),
                           Resistance2 = Convert.ToDecimal(row["Resistance2"]),
                           Support3 = Convert.ToDecimal(row["Support3"]),
                           Resistance3 = Convert.ToDecimal(row["Resistance3"]),
                           MACD = Convert.ToDecimal(row["MACD"]),
                           Hist = Convert.ToDecimal(row["Hist"]),
                           StochRSI=Convert.ToDecimal(row["StochRSI"]),
                            CompareLongPrice = Convert.ToDecimal(row["SMACompareLong"]),
                            CompareShortPrice = Convert.ToDecimal(row["SMACompareShort"]),
                           Direction = row["Direction"].ToString(),
                           BBHigh = Convert.ToDecimal(row["BBHigh"]),
                           BBLow = Convert.ToDecimal(row["BBLow"]),
                           KeltnerUpper = Convert.ToDecimal(row["KeltnerUpper"]),
                           KeltnerLower = Convert.ToDecimal(row["KeltnerLower"])
                       }).ToList();
                
	  

            }
            return lst;
        }


    }
}
