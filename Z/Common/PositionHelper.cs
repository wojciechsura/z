using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Z.Common
{
    internal static class PositionHelper
    {
        internal static Point EvalRelativePosition(Rect winRect, System.Drawing.Rectangle screenSize)
        {
            double halfScreenHeight = screenSize.Height / 2;
            double halfScreenWidth = screenSize.Width / 2;

            double relativeLeft = (winRect.Left + winRect.Width / 2 < halfScreenWidth) ? winRect.Left : winRect.Left - screenSize.Width;
            double relativeTop = (winRect.Top + winRect.Height / 2 < halfScreenHeight) ? winRect.Top : winRect.Top - screenSize.Height;

            return new Point(relativeLeft, relativeTop);
        }

        internal static Point EvalAbsolutePosition(Point relativePoint, System.Drawing.Rectangle screenSize)
        {
            double left = relativePoint.X >= 0 ? relativePoint.X : relativePoint.X + screenSize.Width;
            double top = relativePoint.Y >= 0 ? relativePoint.Y : relativePoint.Y + screenSize.Height;

            return new Point(left, top);
        }
    }
}
