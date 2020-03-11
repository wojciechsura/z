using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Z.Controls
{
    public class BaseSubWindow : Window
    {
        private bool reversed = false;

        protected virtual void OnReversedChanged()
        {

        }

        public bool Reversed
        {
            get => reversed;
            set 
            {
                if (reversed != value)
                {
                    reversed = value;
                    OnReversedChanged();
                }
            }
        }
    }
}
