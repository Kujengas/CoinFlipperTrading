using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinFlipperPro.Model
{
    public class Asset
    {
        public string net { get; set; }
        public string total { get; set; }
    }

    public class Borrow
    {
        public string btc { get; set; }
        public string cny { get; set; }
        public string ltc { get; set; }
    }

    public class Free
    {
        public string btc { get; set; }
        public string cny { get; set; }
        public string ltc { get; set; }
    }

    public class Freezed
    {
        public string btc { get; set; }
        public string cny { get; set; }
        public string ltc { get; set; }
    }

    public class UnionFund
    {
        public string btc { get; set; }
        public string ltc { get; set; }
    }

    public class Funds
    {
        public Asset asset { get; set; }
        public Borrow borrow { get; set; }
        public Free free { get; set; }
        public Freezed freezed { get; set; }
        public UnionFund unionFund { get; set; }
    }

    public class Info
    {
        public Funds funds { get; set; }
    }

    public class ApiAccountInfo
    {
        public Info info { get; set; }
        public bool result { get; set; }
    }
}
