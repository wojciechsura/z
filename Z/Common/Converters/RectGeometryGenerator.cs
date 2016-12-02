using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Z.Common.Converters
{
    public class RectGeometryGenerator : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is double && values[1] is double && values[2] is CornerRadius)
            {
                double width = (double)values[0];
                double height = (double)values[1];
                double radius = ((CornerRadius)values[2]).TopLeft;

                return new RectangleGeometry()
                {
                    Rect = new System.Windows.Rect(0, 0, width, height),
                    RadiusX = radius,
                    RadiusY = radius
                };
            }
            else
                return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
