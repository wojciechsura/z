using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Z.Common;

namespace Z.Types
{
    public abstract class BaseOperatorWindow : Window
    {
        protected readonly WindowInteropHelper windowInteropHelper;

        private Point GetPointsToPixelsConversion(Visual visual)
        {
            Matrix matrix;
            var source = PresentationSource.FromVisual(visual);
            if (source != null)
            {
                matrix = source.CompositionTarget.TransformToDevice;
            }
            else
            {
                using (var src = new HwndSource(new HwndSourceParameters()))
                {
                    matrix = src.CompositionTarget.TransformToDevice;
                }
            }

            return new Point(matrix.M11, matrix.M22);
        }

        private Size GetLogicalScreenSize()
        {
            var screen = System.Windows.Forms.Screen.FromHandle(windowInteropHelper.Handle);
            var conversion = GetPointsToPixelsConversion(this);
            Size screenSize = new Size(screen.WorkingArea.Width / conversion.X, screen.WorkingArea.Height / conversion.Y);
            return screenSize;
        }

        protected void SetRelativePosition(Point value)
        {
            Size screenSize = GetLogicalScreenSize();
            var pos = PositionHelper.EvalAbsolutePosition(value, screenSize);

            Left = pos.X;
            Top = pos.Y;
        }

        protected Point EvalRelativePosition()
        {
            Size screenSize = GetLogicalScreenSize();
            var winRect = new Rect(this.Left, this.Top, this.Width, this.Height);

            return PositionHelper.EvalRelativePosition(winRect, screenSize);
        }

        public BaseOperatorWindow()
        {
            windowInteropHelper = new WindowInteropHelper(this);
        }

        public abstract void Summon();
        public abstract void Dismiss();
    }
}
