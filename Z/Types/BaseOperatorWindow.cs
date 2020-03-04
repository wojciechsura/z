using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Z.Common;

namespace Z.Types
{
    public abstract class BaseOperatorWindow : Window
    {
        protected readonly WindowInteropHelper windowInteropHelper;

        protected void SetRelativePosition(Point value)
        {
            var screen = System.Windows.Forms.Screen.FromHandle(windowInteropHelper.Handle);

            var pos = PositionHelper.EvalAbsolutePosition(value, screen.WorkingArea);

            Left = pos.X;
            Top = pos.Y;
        }

        protected Point EvalRelativePosition()
        {
            var screen = System.Windows.Forms.Screen.FromHandle(windowInteropHelper.Handle);
            var winRect = new Rect(this.Left, this.Top, this.Width, this.Height);
            return PositionHelper.EvalRelativePosition(winRect, screen.WorkingArea);
        }

        public BaseOperatorWindow()
        {
            windowInteropHelper = new WindowInteropHelper(this);
        }

        public abstract void Summon();
        public abstract void Dismiss();
    }
}
