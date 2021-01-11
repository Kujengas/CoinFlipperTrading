using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoinFlipperPro.Model
{
    public class AccountStatus
    {

      public DateTime StatDate { get; set; }
      public  decimal Volume { get; set; }
      public decimal CurrentBalance { get; set; }
      public decimal StartingBalance { get; set; }
      public decimal LastPurchasePrice { get; set; }
      public decimal LastSalePrice { get; set; }

      public TransactionType TransactionType { get; set; }

      public TransactionSource Source { get; set; }
      public bool IsSimulator { get; set; }
      public DateTime LastOrderDate { get; set; }
    }
}
