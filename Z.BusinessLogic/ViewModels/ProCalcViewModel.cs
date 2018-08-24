using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.ViewModels.Interfaces;
using ProCalc.NET;
using ProCalc.NET.Exceptions;
using Z.BusinessLogic.Services.Interfaces;

namespace Z.BusinessLogic.ViewModels
{
    public class ProCalcViewModel : INotifyPropertyChanged
    {
        private IProCalcWindowAccess access;
        private IConfigurationService configurationService;
        private ProCalcCore proCalcCore;

        private string result = "";
        private string binResult = "";
        private string octResult = "";
        private string hexResult = "";
        private string dmsResult = "";
        private string fractionResult = "";
        private string errorText;
        private string enteredText;
        private bool showHint;

        private void ClearInput()
        {
            EnteredText = null;
            ErrorText = null;
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

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ProCalcViewModel(IConfigurationService configurationService)
        {
            this.configurationService = configurationService;

            proCalcCore = new ProCalcCore();
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
            access.Position = configurationService.Configuration.MainWindow.Position;
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

            try
            {
                var compiled = proCalcCore.Compile(enteredText);
                var result = proCalcCore.Execute(compiled);

                Result = result.AsString();
                BinResult = result.AsBin();
                OctResult = result.AsOct();
                HexResult = result.AsHex();
                DmsResult = result.AsDMS();
                FractionResult = result.AsIntFraction();

                ErrorText = null;
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
                    if (expressionException.ExpressionPos >= 0)
                        access.CaretPosition = expressionException.ExpressionPos;                    
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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
