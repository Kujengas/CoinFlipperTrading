using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinFlipperPro.Model
{
    public class OrderSet
    {


        public List<Order> orders { get; set; }
        public bool result { get; set; }

    }
    public class Order
    {
        public decimal amount { get; set; }
        public decimal avg_rate { get; set; }
        public long createDate { get; set; }
        public decimal deal_amount { get; set; }
        public int orders_id { get; set; }
        public decimal rate { get; set; }
        public int status { get; set; }
        public string symbol { get; set; }
        public string type { get; set; }
    }

}
