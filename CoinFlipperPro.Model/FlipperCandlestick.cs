using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinFlipperPro.Model
{
    public class FlipperCandlestick
    {
        public DateTime IntervalDate { get; set; }
        public decimal Hist { get; set; }
        public string Direction { get; set; }
        public string Squeeze { get {return ((BBLow > KeltnerLower && BBHigh<KeltnerUpper)?"On":"Off"); } }
        public decimal SMAShort { get; set; }
        public decimal SMALong { get; set; }
        public decimal ClosePrice { get; set; }
        public decimal OpenPrice { get; set; }
        public decimal LowPrice { get; set; }
        public decimal HighPrice { get; set; }
        public decimal AveragePrice { get; set; }
        public DateTime CloseDataEntryDate { get; set; }
        public decimal CompareLongPrice { get; set; }
        public decimal CompareShortPrice { get; set; }
        public decimal SMALow { get; set; }
        public decimal SMAHigh { get; set; }
        public decimal SMASignal { get; set; }
        public decimal MACD { get; set; }
        public decimal Support1 { get; set; }
        public decimal Resistance1 { get; set; }
        public decimal Support2 { get; set; }
        public decimal Resistance2 { get; set; }
        public decimal Support3 { get; set; }
        public decimal Resistance3 { get; set; }
        public decimal StochRSI { get; set; }
        public string IntervalTime { get; set; }
        public decimal BBHigh { get; set; }
        public decimal BBLow { get; set; }
        public decimal KeltnerUpper { get; set; }
        public decimal KeltnerLower { get; set; }
        public Int64 PriceId { get; set; }
    }
}
