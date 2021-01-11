using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinFlipperPro.Model
{
       public class TickerEntry
        {
            public TickerEntry() { }
            public long Date { get; set; }
            public Ticker Data { get; set; }

            public string channel { get; set; }
            public DateTime GetTickerDateTime()
            {
                System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                return dtDateTime.AddSeconds(Date).ToLocalTime();
            }
        }

   
}
