using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinFlipperPro.Trading
{
    public static class TradingStrategyFactory
    {
        public static ITradingStrategy GetTradingStrategy(string name)
        {
            switch (name)
            {
                case "Default":
                    return new DefaultStrategy();
                case "Simulation":
                    return new SimulationStategy();
                case "FastOverSlow":
                    return new FastOverSlowStrategy();
                case "RideTheMacd":
                    return new RideTheMacdStrategy();
                default:
                    return new DefaultStrategy();
            }

        }
    }
}
