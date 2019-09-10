using Microsoft.Xaml.Interactivity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WorkUwpApp.Behaviors
{
    public class MultiSelectBehavior : Behavior<ListViewBase>
    {
        private bool _selectionChangedInProgress = false;

        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
                                  "SelectedItems",
                                  typeof(ObservableCollection<object>),
                                  typeof(MultiSelectBehavior),
                                  new PropertyMetadata(new ObservableCollection<object>(), PropertyChangedCallback));

        public ObservableCollection<object> SelectedItems
        {
            get { return (ObservableCollection<object>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += OnSelectionChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_selectionChangedInProgress) return;
            _selectionChangedInProgress = true;
            foreach (var item in e.RemovedItems)
            {
                if (SelectedItems.Contains(item))
                {
                    SelectedItems.Remove(item);
                }
            }

            foreach (var item in e.AddedItems)
            {
                if (!SelectedItems.Contains(item))
                {
                    SelectedItems.Add(item);
                }
            }
            _selectionChangedInProgress = false;
        }
        //sender
        private static void PropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            NotifyCollectionChangedEventHandler handler = (s, e) => SelectedItemsChanged(sender, e);
            if (args.OldValue is ObservableCollection<object>)
            {
                (args.OldValue as ObservableCollection<object>).CollectionChanged -= handler;
            }

            if (args.NewValue is ObservableCollection<object>)
            {
                (args.NewValue as ObservableCollection<object>).CollectionChanged += handler;
            }
        }

        private static void SelectedItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender != null && sender is MultiSelectBehavior)   //adding "sender != null &&"
            {
                //var listViewBase = (sender as MultiSelectBehavior).AssociatedObject;
                //var listSelectedItems = listViewBase.SelectedItems;

                //null after second call of page
                var listSelectedItems = (sender as MultiSelectBehavior).AssociatedObject.SelectedItems;
                if (e.OldItems != null)
                {
                    foreach (var item in e.OldItems)
                    {
                        if (listSelectedItems.Contains(item))
                        {
                            listSelectedItems.Remove(item);
                        }
                    }
                }

                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems)
                    {
                        if (!listSelectedItems.Contains(item))
                        {
                            listSelectedItems.Add(item);
                        }
                    }
                }





            }
        }
    }
}
