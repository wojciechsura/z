using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Z.BusinessLogic.ViewModels.Base;

namespace Z.Common.Extensions
{
    public class TreeViewBehavior<TItem> : Behavior<TreeView>
        where TItem : HierarchicalViewModel<TItem>
    {
        private static TreeViewItem TreeViewItemFromItem(TreeView treeView, TItem selected)
        {
            if (selected == null)
                return null;

            // Build hierarchy
            List<TItem> hierarchy = new List<TItem>();
            var current = selected;
            while (current != null)
            {
                hierarchy.Add(current);
                current = current.Parent;
            }

            TreeViewItem item = (TreeViewItem)treeView.ItemContainerGenerator.ContainerFromItem(hierarchy.Last());

            if (item == null)
                return null;

            for (int i = hierarchy.Count - 2; i >= 0; i--)
            {
                item = (TreeViewItem)item.ItemContainerGenerator.ContainerFromItem(hierarchy[i]);
            }

            return item;
        }

        #region SelectedItem Property

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(TreeViewBehavior<TItem>), new UIPropertyMetadata(null, OnSelectedItemChanged));

        private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var item = TreeViewItemFromItem((sender as LauncherTreeViewBehavior).AssociatedObject, e.NewValue as TItem);
            if (item != null)
            {
                item.SetValue(TreeViewItem.IsSelectedProperty, true);
            }
        }

        #endregion

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.SelectedItemChanged += OnTreeViewSelectedItemChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (this.AssociatedObject != null)
            {
                this.AssociatedObject.SelectedItemChanged -= OnTreeViewSelectedItemChanged;
            }
        }

        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.SelectedItem = e.NewValue;
        }
    }
}
