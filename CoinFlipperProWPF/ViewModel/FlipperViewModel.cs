using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Threading;
using CoinFlipperPro;
using CoinFlipperPro.Model;
using CoinFlipperPro.DataAccess;
using System.Collections.ObjectModel;
using CoinFlipperPro.Trading;
using CoinFlipperPro.Api;

namespace CoinFlipperProWPF.ViewModel
{
    class FlipperViewModel : INotifyPropertyChanged
    {
        #region Private Fields
        private int duration;
        private int accountDuration;
        private int macdChartInterval;
        private string summarytext;
        private DispatcherTimer timer = null;
        private DispatcherTimer accounttimer = null;
        private CoinFlipperPro.Model.FlipperDataModel fdm = new FlipperDataModel();
        private ObservableCollection<FlipperCandlestick> macd = new ObservableCollection<FlipperCandlestick>();
        private ObservableCollection<FlipperCandlestick> macdIntervalSlow = new ObservableCollection<FlipperCandlestick>();
        private ObservableCollection<FlipperCandlestick> macdIntervalFast = new ObservableCollection<FlipperCandlestick>();
        private ObservableCollection<Ticker> ticker = new ObservableCollection<Ticker>();
        private ObservableCollection<FlipperException> errors = new ObservableCollection<FlipperException>();
        private DateTime lastGridUpdate;
        private bool isAutoRefreshGridEnabled;
        private bool isLiveApi;
        private bool isLogicRunning;
        private bool isAlgoOn;
        private ITradingStrategy tradingStrategy;
        private ITradingStrategy simulationStrategy;
        private ITradeApi tradingApi;
        private ITradeApi simulationApi;
        #endregion

        #region Constructor
        public FlipperViewModel()
        {
            try
            {
                this.Duration = 1000;
                this.AccountDuration = 30000;
                this.StartTimerCommand = new RelayCommand(this.StartTimer);
                this.StopTimerCommand = new RelayCommand(this.StopTimer);
                this.TurnAlgoOffCommand = new RelayCommand(this.TurnAlgoOff);
                this.TurnAlgoOnCommand = new RelayCommand(this.TurnAlgoOn);
                this.ToggleLiveApiCommand = new RelayCommand(this.ToggleLiveApi);
                this.ResetAlgoConfigCommand = new RelayCommand(this.ResetAlgoConfig);
                ResetAlgoConfig();
                Fdm.macd = new List<FlipperCandlestick>();
                Fdm.macdIntervalFast = new List<FlipperCandlestick>();
                Fdm.macdIntervalSlow = new List<FlipperCandlestick>();
                Fdm.ticker = new List<Ticker>();
                lastGridUpdate = DateTime.Now;
                this.Fdm.stats = new AccountStatus { StartingBalance = 1000, CurrentBalance = 1000 };
                this.IsAutoRefreshGridEnabled = true;
                UpdateSummaryText();
            }
            catch (Exception ex)
            {
                AddError(ex, "ViewModelConstructor");
            }
        }



        #endregion

        #region Properties

        public FlipperDataModel Fdm
        {

            get
            {
                return this.fdm;
            }
            set
            {
                this.fdm = value;
                RaisePropertyChanged("Fdm");
            }

        }

        public int Duration
        {
            get
            {
                return this.duration;
            }
            set
            {
                this.duration = value;
                RaisePropertyChanged("Duration");
            }
        }

        public int AccountDuration
        {
            get
            {
                return this.accountDuration;
            }
            set
            {
                this.accountDuration = value;
                RaisePropertyChanged("AccountDuration");
            }
        }

        public string SummaryText
        {
            get
            {
                return this.summarytext;
            }
            set
            {
                this.summarytext = value;
                RaisePropertyChanged("SummaryText");
            }
        }

        public bool IsAutoRefreshGridEnabled
        {
            get
            {
                return this.isAutoRefreshGridEnabled;
            }
            set
            {
                this.isAutoRefreshGridEnabled = value;
                RaisePropertyChanged("IsAutoRefreshGridEnabled");
            }
        }

        public bool IsLiveApi
        {
            get
            {
                return this.isLiveApi;
            }
            set
            {
                this.isLiveApi = value;
                RaisePropertyChanged("IsLiveApi");
            }
        }

        public bool IsAlgoOn
        {
            get
            {
                return this.isAlgoOn;
            }
            set
            {
                this.isAlgoOn = value;
                RaisePropertyChanged("IsAlgoOn");
                RaisePropertyChanged("IsAlgoOnButtonEnabled");
                RaisePropertyChanged("IsAlgoOffButtonEnabled");
            }
        }

        public bool IsAlgoOnButtonEnabled
        {
            get { return !this.IsAlgoOn; }
        }

        public bool IsAlgoOffButtonEnabled
        {
            get { return this.IsAlgoOn; }
        }

        public ObservableCollection<FlipperCandlestick> Macd
        {
            get
            {
                return this.macd;
            }
            set
            {
                this.macd = value;
                RaisePropertyChanged("Macd");
            }
        }


        public int MacdChartInterval
        {
            get
            {
                return this.macdChartInterval;
            }
            set
            {
                this.macdChartInterval = value;
                RaisePropertyChanged("MacdChartInterval");
            }
           
        }


        public ObservableCollection<FlipperCandlestick> MacdIntervalFast
        {
            get
            {
                return this.macdIntervalFast;
            }
            set
            {
                this.macdIntervalFast = value;
                RaisePropertyChanged("MacdIntervalFast");
            }
        }

        public ObservableCollection<FlipperException> Errors
        {
            get
            {
                return this.errors;
            }
            set
            {
                this.errors = value;
                RaisePropertyChanged("Errors");
            }
        }

        public ObservableCollection<FlipperCandlestick> MacdIntervalSlow
        {
            get
            {
                return this.macdIntervalSlow;
            }
            set
            {
                this.macdIntervalSlow = value;
                RaisePropertyChanged("MacdIntervalSlow");
            }
        }

        public ObservableCollection<Ticker> Ticker
        {
            get
            {
                return this.ticker;
            }
            set
            {
                this.ticker = value;
                RaisePropertyChanged("Ticker");
            }
        }

        public ICommand StartTimerCommand
        {
            get;
            set;
        }

        public ICommand StopTimerCommand
        {
            get;
            set;
        }


        public ICommand TurnAlgoOnCommand
        {
            get;
            set;
        }

        public ICommand TurnAlgoOffCommand
        {
            get;
            set;
        }

        public ICommand ResetAlgoConfigCommand
        {
            get;
            set;
        }
        public ICommand ToggleLiveApiCommand
        {
            get;
            set;
        }
        #endregion

        #region Methods
        public void StartTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(this.Duration);
            timer.Tick += new EventHandler(TimerTick);
            timer.Start();

            accounttimer = new DispatcherTimer();
            accounttimer.Interval = TimeSpan.FromMilliseconds(this.AccountDuration);
            accounttimer.Tick += new EventHandler(AccountTimerTick);
            accounttimer.Start();

        }
        public void StopTimer()
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }
            if (accounttimer != null)
            {
                accounttimer.Stop();
                accounttimer = null;
            }
        }

        public void TurnAlgoOn()
        {            try {
            this.tradingStrategy = TradingStrategyFactory.GetTradingStrategy("RideTheMacd");

            if (this.tradingApi == null)
            {
                this.tradingApi = TradingApiFactory.GetTradingApi("");
            }
            this.simulationStrategy = TradingStrategyFactory.GetTradingStrategy("Simulation");
            this.simulationApi = TradingApiFactory.GetTradingApi("");

            this.IsAlgoOn = true;
        }
        catch (Exception ex)
        {
            AddError(ex, "TurnAlgoOn");
        }
        }

        public void TurnAlgoOff()
        {
            this.IsAlgoOn = false;

        }

        public void ToggleLiveApi()
        {
            if (IsLiveApi) 
            {
                if (tradingApi == null) {
                    this.tradingApi = TradingApiFactory.GetTradingApi(""); 
                }
                GetAccontInfo();
            }
        }

        private void UpdateSummaryText()
        {
            try { 
            StringBuilder accountDisplayData = new StringBuilder();
            accountDisplayData.AppendLine("---Account Status-------------------------------------------------------------------------------------------");
            accountDisplayData.Append(string.Format("Last Transaction Time:{0} ", Fdm.stats.StatDate.ToLocalTime().ToShortTimeString()));
            accountDisplayData.Append(Environment.NewLine);
            accountDisplayData.Append(string.Format("Starting Balance:{0:$0.##} ", Fdm.stats.StartingBalance));
            accountDisplayData.Append(string.Format("Current Balance:{0:$0.##} ", Fdm.stats.CurrentBalance));
            accountDisplayData.Append(string.Format("Current Volume:{0:0.##} ", Fdm.stats.Volume));
            accountDisplayData.Append(Environment.NewLine);
            accountDisplayData.Append(string.Format("Last Purchase Price:{0:$0.##} ", Fdm.stats.LastPurchasePrice));
            accountDisplayData.Append(string.Format("Last Sale Price:{0:$0.##} ", Fdm.stats.LastSalePrice));
            accountDisplayData.Append(Environment.NewLine);
            accountDisplayData.Append(string.Format("TrueAsk:{0:$0.##} ", Fdm.depth.ActualMarketAsk(Fdm.stats.CurrentBalance).Price));
            accountDisplayData.Append(string.Format(" TrueBid:{0:$0.##} ", Fdm.depth.ActualMarketBid(Fdm.depth.ActualMarketAsk(Fdm.stats.CurrentBalance).Amount).Price));
            accountDisplayData.Append(Environment.NewLine);
            accountDisplayData.AppendLine("---Ticker Status-------------------------------------------------------------------------------------------");


            if (Fdm.macdIntervalFast.Count > 1 /*&& latestTicker.Count > 0*/)
            {
                double sma = Convert.ToDouble(Fdm.macdIntervalFast.Average(x => x.ClosePrice));
                double close = Convert.ToDouble(Fdm.macdIntervalFast[0].ClosePrice);
                double high = Convert.ToDouble(Fdm.macdIntervalFast[1].SMAHigh);
                double low = Convert.ToDouble(Fdm.macdIntervalFast[1].SMALow);
                double bid = Convert.ToDouble(Fdm.ticker.FirstOrDefault().buy);
                double ask = Convert.ToDouble(Fdm.ticker.FirstOrDefault().sell);
                double avg = Convert.ToDouble(Fdm.macdIntervalFast.MacdAveragePrice());
                // double avg = Convert.ToDecimal(dt.Rows[0]["TickerAvg24"]);
                double proposed = Convert.ToDouble((Math.Floor(((macdIntervalFast[0].AveragePrice + macdIntervalFast[1].AveragePrice + macd[0].AveragePrice + macd[1].AveragePrice) / 4) * 100) / 100) + Fdm.algoConfig.BuyThreshold);

                accountDisplayData.AppendLine(string.Format("Tick Date:{0} ", Fdm.ticker.FirstOrDefault().DataEntryDate.ToString()));
               
                accountDisplayData.Append(string.Format("Last:{0:$0.##} ", close));
                accountDisplayData.Append(string.Format("Bid:{0:$0.##} ", bid));
                 accountDisplayData.Append(string.Format("Ask:{0:$0.##} ", ask));
                
                accountDisplayData.Append(string.Format("Avg Low:{0:$0.##} ", low));
                accountDisplayData.Append(string.Format("Avg High:{0:$0.##} ", high));
                accountDisplayData.Append(string.Format("SMA:{0:$0.##} ", sma));
                accountDisplayData.Append(string.Format("SMA Offset:{0:0.##} ", close - sma));
                accountDisplayData.Append(string.Format("Avg24 :{0:$0.##} ", avg));
                accountDisplayData.Append(string.Format("Avg24 Offset:{0:0.##} ", close - avg));

                accountDisplayData.Append(string.Format("Calculated Bid Price:{0:$0.##} ", proposed));
            }
            this.SummaryText = accountDisplayData.ToString();


            //lblCurrentBalance.Text = String.Format("{0:$0.##}", stats.CurrentBalance);
            //lblCurrentVol.Text = String.Format("{0:0.##}", stats.Volume);
            //lblLastPurchase.Text = String.Format("{0:$0.##}", stats.LastPurchasePrice);
            //lblLastSale.Text = String.Format("{0:$0.##}", stats.LastSalePrice);
            //lblLastTransaction.Text = stats.StatDate.ToLocalTime().ToShortTimeString();
            //LblStartingLabel.Text = String.Format("{0:$0.##}", stats.StartingBalance);

            /*  if (isLiveTrading)
              {
                  lblAvailibleCoin.Text = accountInfo.info.funds.free.ltc;
                  lblAvalibleFunds.Text = accountInfo.info.funds.free.cny;
                  lblFrozenCoin.Text = accountInfo.info.funds.freezed.ltc;
                  lblFrozenFunds.Text = accountInfo.info.funds.freezed.cny;



              }*/
            }
            catch (Exception ex)
            {
                AddError(ex, "UpdateSummaryText");
            }
        }

        private void LoadData()
        {
            try
            {
                Fdm.macd = TickerDAL.GetMacdInterval(4, 1, -1);
                
                Fdm.macdIntervalFast = TickerDAL.GetMacdInterval(24, Fdm.algoConfig.MacdIntervalTime, 19, 6, 9, -1);
                Fdm.macdIntervalSlow = TickerDAL.GetMacdInterval(24, Fdm.algoConfig.MacdIntervalTime, -1);
                Fdm.depth = TickerDAL.GetMarketDepth();
                Fdm.ticker = TickerDAL.GetTicker(4, -1);
              
                if (IsAutoRefreshGridEnabled && lastGridUpdate < DateTime.Now.AddSeconds(-5))
                {
                    Macd = new ObservableCollection<FlipperCandlestick>(Fdm.macd);
                    MacdIntervalFast = new ObservableCollection<FlipperCandlestick>(Fdm.macdIntervalFast);
                    MacdIntervalSlow = new ObservableCollection<FlipperCandlestick>(Fdm.macdIntervalSlow);
                    MacdChartInterval = 4;
                    Ticker = new ObservableCollection<Ticker>(Fdm.ticker);
                    lastGridUpdate = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                AddError(ex, "LoadData");
            }

        }

        private void RunTradeLogic()
        {
            try { 

            if (IsAlgoOn)
            {
                ITradingStrategy activeStrategy;
                ITradeApi activeApi;
                if (IsLiveApi)
                {
                    activeStrategy = tradingStrategy;
                    activeApi = tradingApi;

                    activeStrategy.Buy(Fdm, activeStrategy.ShouldBuy(Fdm), activeApi);
                    activeStrategy.Sell(Fdm, activeStrategy.ShouldSell(Fdm), activeApi);
                    activeStrategy.StopLoss(Fdm, activeStrategy.ShouldStopLoss(Fdm), activeApi);
                }
                else
                {
                    activeStrategy = simulationStrategy;
                    activeApi = simulationApi;

                    Fdm.stats = activeStrategy.Buy(Fdm, activeStrategy.ShouldBuy(Fdm), activeApi);
                    Fdm.stats = activeStrategy.Sell(Fdm, activeStrategy.ShouldSell(Fdm), activeApi);
                    Fdm.stats = activeStrategy.StopLoss(Fdm, activeStrategy.ShouldStopLoss(Fdm), activeApi);
                }



            }
            }
            catch (Exception ex)
            {
                AddError(ex, "RunTradeLogic");
            }
        }
        private void LoadOpenOrders()
        {
            try
            {
                Fdm.openOrders = tradingApi.GetUnfulfilledOrders(-1);
            }
            catch (Exception ex)
            {
                AddError(ex, "LoadOpenOrders");
            }
        }
        private void GetAccontInfo()
        {
            try
            {
                Fdm.accountInfo = tradingApi.GetAccountinfo();
                Fdm.stats.CurrentBalance = Convert.ToDecimal(Fdm.accountInfo.info.funds.free.cny);
                Fdm.stats.IsSimulator = false;
                Fdm.stats.Source = TransactionSource.LiveData;
                Fdm.stats.Volume = Convert.ToDecimal(Fdm.accountInfo.info.funds.free.ltc);
                
                if (Fdm.lastOrderResult != null)
                {
                    int id = Fdm.lastOrderResult.order_id;
                    //TradeApi.
                }
                LoadOpenOrders();
                UpdateSummaryText();
                RaisePropertyChanged("Fdm");
            }
            catch (Exception ex)
            {
                AddError(ex, "GetAccountInfo");
            }
        }

        public void ResetAlgoConfig()
        {
            this.Fdm.algoConfig = new AlgoConfiguration
            {
                BuyThreshold = 0.02M,
                FixedSellThreshold = 0.02M,
                MacdIntervalTime = 7,
                SellThreshold = 0.02M,
                StopLossPercentage = .995M,
                StopLossTime = 15,
                UseStrictSellThreshold = true,
                StopLossMode = StopMode.Any
            };

            TurnAlgoOff();
            RaisePropertyChanged("Fdm");
        }

        private void TimerTick(object send, EventArgs e)
        {
            if (!isLogicRunning)
            {
                isLogicRunning = true;
               // GetAccontInfo();
                LoadData();
                UpdateSummaryText();
                RunTradeLogic();
                isLogicRunning = false;
            }



        }

        public void AddError(Exception ex, string methodName)
        {
            this.Errors.Add(new FlipperException { Message = ex.Message, ExceptionDate = DateTime.Now, Source = methodName });
            RaisePropertyChanged("Errors");
        }

        private void AccountTimerTick(object send, EventArgs e)
        {
            if (!isLogicRunning)
            {
                GetAccontInfo();
            }



        }
        #endregion

        #region NotifyPropertyChanged Methods

        public event PropertyChangedEventHandler PropertyChanged;


        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}







