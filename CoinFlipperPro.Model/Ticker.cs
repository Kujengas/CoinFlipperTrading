using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinFlipperPro.Model
{

      public class Ticker
      {
          public long priceId { get; set; }
          public DateTime DataEntryDate { get; set; }
          public decimal offset { get; set; }
          public decimal offset12 { get; set; }
          public decimal offset24 { get; set; }
          public decimal avg { get; set; }
          public decimal avg12 { get; set; }
          public decimal avg24 { get; set; }
          public decimal buy { get; set; }
          public decimal high { get; set; }
          public decimal last { get; set; }
          public decimal low { get; set; }
          public decimal sell { get; set; }
          public decimal vol { get; set; }



      }
    
}
