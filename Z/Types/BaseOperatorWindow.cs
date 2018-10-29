using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Z.Types
{
    public abstract class BaseOperatorWindow : Window
    {
        public abstract void Summon();
        public abstract void Dismiss();
    }
}
