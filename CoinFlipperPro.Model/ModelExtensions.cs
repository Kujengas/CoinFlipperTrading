using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinFlipperPro.Model
{
    public static class ModelExtensions
    {
        public static double ConvertToDouble(this decimal value) 
        {
            return Convert.ToDouble(value);
        }


        public static decimal ConvertToDecimal(this double value)
        {
            return Convert.ToDecimal(value);
        }

    }
}
