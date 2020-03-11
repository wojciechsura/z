using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Z.Controls
{
    public class ReversibleStackPanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            double width = 0.0, height = 0.0;

            for (int i = 0; i < InternalChildren.Count; i++)
            {
                UIElement child = InternalChildren[i];

                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                Size desiredSize = child.DesiredSize;

                if (child is FrameworkElement frameworkElement)
                    desiredSize = new Size(desiredSize.Width + frameworkElement.Margin.Left + frameworkElement.Margin.Right,
                        desiredSize.Height + frameworkElement.Margin.Top + frameworkElement.Margin.Bottom);

                width = Math.Max(width, desiredSize.Width);
                height += desiredSize.Height;
            }

            return new Size(width, height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double heights = 0.0;

            for (int i = 0; i < InternalChildren.Count; i++)
            {
                UIElement child = InternalChildren[i];

                Thickness margin = new Thickness(0);

                if (child is FrameworkElement frameworkElement)
                    margin = frameworkElement.Margin;

                if (Reverse)
                {
                    double y = finalSize.Height - heights - margin.Bottom - child.DesiredSize.Height;
                    double x = margin.Left;

                    child.Arrange(new Rect(x, y, child.DesiredSize.Width, child.DesiredSize.Height));
                }
                else
                {
                    double y = heights + margin.Top;
                    double x = margin.Left;

                    child.Arrange(new Rect(x, y, child.DesiredSize.Width, child.DesiredSize.Height));
                }

                heights += margin.Top + child.DesiredSize.Height + margin.Bottom;
            }

            return base.ArrangeOverride(finalSize);
        }

        #region Reverse dependency property

        public bool Reverse
        {
            get { return (bool)GetValue(ReverseProperty); }
            set { SetValue(ReverseProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Reverse.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReverseProperty =
            DependencyProperty.Register("Reverse", typeof(bool), typeof(ReversibleStackPanel), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange));

        #endregion
    }
}
