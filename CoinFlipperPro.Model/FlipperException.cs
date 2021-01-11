using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinFlipperPro.Model
{
 public   class FlipperException
    {
        public DateTime ExceptionDate { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }

    }
}
