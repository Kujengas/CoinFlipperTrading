using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinFlipperPro.Model
{
  public  class ApiOrderResult
    {
            public bool result { get; set; }
            public int order_id { get; set; }
            public int errorCode { get; set; }
    }
}
