using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProCalc.NET;
using ProCalc.NET.Exceptions;
using System.Windows.Input;
using Z.Wpf.Types;
using Z.BusinessLogic.Events;
using System.Windows;
using Z.BusinessLogic.Services.EventBus;
using Z.BusinessLogic.Services.Config;
using Z.BusinessLogic.Services.AppWindows;
using Z.BusinessLogic.Services.Application;

namespace Z.BusinessLogic.ViewModels.ProCalc
{
    public class ProCalcViewModel : INotifyPropertyChanged, IEventListener<PositionChangedEvent>, IEventListener<ShuttingDownEvent>
    {
        private readonly IConfigurationService configurationService;
        private readonly IAppWindowService windowService;
        private readonly IApplicationController applicationController;
        private readonly ProCalcCore proCalcCore;
        private readonly IEventBus eventBus;
        private IProCalcWindowAccess access;

        private string result = "";
        private string binResult = "";
        private string octResult = "";
        private string hexResult = "";
        private string dmsResult = "";
        private string fractionResult = "";
        private string errorText;
        private string enteredText;
        private bool showHint;
        private bool suspendPositionChangeNotifications = false;

        private void ClearInput()
        {
            EnteredText = null;
            ErrorText = null;
            Result = "";
            BinResult = "";
            OctResult = "";
            HexResult = "";
            DmsResult = "";
            FractionResult = "";
        }

        private void InternalDismissWindow()
        {
            access.Hide();
        }

        private void InternalSummonWindow()
        {
            ClearInput();

            access.Show();
            ShowHint = true;
        }

        private void EnteredTextChanged()
        {
            ShowHint = false;
            ErrorText = null;
        }

        private bool IsInputEmpty()
        {
            return String.IsNullOrEmpty(enteredText);
        }

        private void OpenConfiguration()
        {
            ClearInput();
            access.OpenConfiguration();
        }

        private void Shutdown()
        {
            applicationController.Shutdown();
        }

        private void DoSwitchToZ()
        {
            windowService.ShowMainWindow();
        }

        private void DoSwitchToProCalc()
        {
            windowService.ShowProCalcWindow();
        }

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // IEventListener implementation --------------------------------------

        void IEventListener<ShuttingDownEvent>.Receive(ShuttingDownEvent @event)
        {
            

            configurationService.Configuration.ProCalcWindow.RelativePosition = access.RelativePosition;
        }

        void IEventListener<PositionChangedEvent>.Receive(PositionChangedEvent @event)
        {
            if (configurationService.Configuration.General.SynchronizeWindowPositions && 
                @event.Origin != this && 
                    ((int)@event.X != (int)access.Position.X || 
                    (int)@event.Y != (int)access.Position.Y))
            {
                try
                {
                    // Don't propagate this as an event
                    suspendPositionChangeNotifications = true;
                    access.Position = new Point(@event.X, @event.Y);
                }
                finally
                {
                    suspendPositionChangeNotifications = false;
                }
            }
        }

        // Public methods -----------------------------------------------------

        public ProCalcViewModel(IConfigurationService configurationService,
            IAppWindowService windowService,
            IApplicationController applicationController,
            IEventBus eventBus)
        {
            this.configurationService = configurationService;
            this.windowService = windowService;
            this.applicationController = applicationController;
            this.eventBus = eventBus;

            this.eventBus.Register((IEventListener<PositionChangedEvent>)this);
            this.eventBus.Register((IEventListener<ShuttingDownEvent>)this);

            proCalcCore = new ProCalcCore();

            ConfigurationCommand = new SimpleCommand((obj) => OpenConfiguration());
            CloseCommand = new SimpleCommand((obj) => Shutdown());
            SwitchToZCommand = new SimpleCommand((obj) => DoSwitchToZ());
            SwitchToProCalcCommand = new SimpleCommand((obj) => DoSwitchToProCalc());
        }

        public IProCalcWindowAccess ProCalcWindowAccess
        {
            set
            {
                if (access != null)
                    throw new InvalidOperationException("ProCalcWindowAccess can be set only once!");
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                access = value;
            }
        }

        public void Initialized()
        {
            access.RelativePosition = configurationService.Configuration.ProCalcWindow.RelativePosition;
        }

        public void Dismiss()
        {
            InternalDismissWindow();
        }

        public void Summon()
        {
            InternalSummonWindow();
        }

        public bool EscapePressed()
        {
            if (!IsInputEmpty())
            {
                ClearInput();
            }
            else
            {
                InternalDismissWindow();
            }

            return true;
        }

        public bool EnterPressed()
        {
            // Evaluate expression

            access.InputSelectAll();

            try
            {
                var compiled = proCalcCore.Compile(enteredText);
                var result = proCalcCore.Execute(compiled);

                Result = result.AsString;
                BinResult = result.AsBin;
                OctResult = result.AsOct;
                HexResult = result.AsHex;
                DmsResult = result.AsDMS;
                FractionResult = result.AsIntFraction;

                ErrorText = null;

                access.ShowResults();
            }
            catch (ProCalcException e)
            {
                Result = "";
                BinResult = "";
                OctResult = "";
                HexResult = "";
                DmsResult = "";
                FractionResult = "";

                if (e is ExpressionException expressionException)
                {
                    if (expressionException.Column >= 0)
                        access.CaretPosition = expressionException.Column;
                }

                if (e is UserException userException)
                {
                    ErrorText = userException.ErrorCode.ToString();
                }
                else
                {
                    ErrorText = e.Message;
                }
            }

            return true;
        }

        public void WindowLostFocus()
        {
            // HideWindow();
        }

        public string Result
        {
            get
            {
                return result;
            }
            set
            {
                result = value;
                OnPropertyChanged(nameof(Result));
            }
        }

        public string BinResult
        {
            get
            {
                return binResult;
            }
            set
            {
                binResult = value;
                OnPropertyChanged(nameof(BinResult));
            }
        }

        public string OctResult
        {
            get
            {
                return octResult;
            }
            set
            {
                octResult = value;
                OnPropertyChanged(nameof(OctResult));
            }
        }

        public void NotifyPositionChanged(int left, int top)
        {
            if (!suspendPositionChangeNotifications)
                eventBus.Send(new PositionChangedEvent(left, top, this));
        }

        public string HexResult
        {
            get
            {
                return hexResult;
            }
            set
            {
                hexResult = value;
                OnPropertyChanged(nameof(HexResult));
            }
        }

        public string DmsResult
        {
            get
            {
                return dmsResult;
            }
            set
            {
                dmsResult = value;
                OnPropertyChanged(nameof(DmsResult));
            }
        }

        public string FractionResult
        {
            get
            {
                return fractionResult;
            }
            set
            {
                fractionResult = value;
                OnPropertyChanged(nameof(FractionResult));
            }
        }

        public string EnteredText
        {
            get
            {
                return enteredText;
            }
            set
            {
                enteredText = value;
                EnteredTextChanged();
                OnPropertyChanged(nameof(EnteredText));
            }
        }

        public bool ShowHint
        {
            get
            {
                return showHint;
            }
            set
            {
                showHint = value;
                OnPropertyChanged(nameof(ShowHint));
            }
        }

        public string ErrorText
        {
            get
            {
                return errorText;
            }
            set
            {
                errorText = value;
                OnPropertyChanged(nameof(ErrorText));
            }
        }

        public ICommand ConfigurationCommand { get; private set; }

        public ICommand CloseCommand { get; private set; }

        public ICommand SwitchToZCommand { get; private set; }

        public ICommand SwitchToProCalcCommand { get; private set; }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
