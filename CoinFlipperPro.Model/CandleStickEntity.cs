using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinFlipperPro.Model
{
   public class CandleStickEntity
    {
       public DateTime IntervalDate { get; set; }
      public  double AveragePrice { get; set; }
      public  double OpenPrice { get; set; }
      public  double ClosePrice { get; set; }
      public  double LowPrice { get; set; }
      public  double HighPrice { get; set; }
      public  DateTime CloseDataEntryDate { get; set; }

    }
}
