using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinFlipperPro.Model
{
    public class Macd
    {

        public DateTime PriceDate { get; set; }

        public decimal Histogram { get; set; }
        public string Direction { get; set; }
        public decimal Average { get; set; }
        public string CandlePattern { get; set; }
        public decimal Open { get; set; }

        public decimal Close { get; set; }
        public decimal Low { get; set; }

        public decimal High { get; set; }

        public decimal EMAShort { get; set; }
        public decimal EMALong { get; set; }
        public decimal Difference { get; set; }
        public decimal EMASignal { get; set; }

    }
}
