using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Z.Extensions
{
    public class SourceDisplay
    {
        #region GetGroupBy dependency property

        public static string GetGroupByProperty(DependencyObject obj)
        {
            return (string)obj.GetValue(GroupByPropertyProperty);
        }

        public static void SetGroupByProperty(DependencyObject obj, string value)
        {
            obj.SetValue(GroupByPropertyProperty, value);
        }


        public static readonly DependencyProperty GroupByPropertyProperty =
            DependencyProperty.RegisterAttached("GroupByProperty", typeof(string), typeof(SourceDisplay), new PropertyMetadata(null, HandleGroupByChanged));

        private static void HandleGroupByChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is CollectionViewSource collectionViewSource))
                return;

            if (!(e.NewValue is string) && e.NewValue != null)
                return;

            var property = e.NewValue as string;

            collectionViewSource.GroupDescriptions.Clear();
            if (!string.IsNullOrEmpty(property))
                collectionViewSource.GroupDescriptions.Add(new PropertyGroupDescription(property));
        }

        #endregion
    }
}
