using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Markup;
using UnoMultiselect.Shared;

//using TextSearch = Telerik.Windows.Controls.TextSearch;

namespace TheHub.Wasm.Controls.CheckableCombo
{
    /// <summary>
    /// MultiSelect ComboBox
    /// </summary>
    public class MultiSelectComboBox : ComboBox, INotifyPropertyChanged
    {
        #region Events

        /// <summary>
        /// Est appelé quand une propriété change
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of MultiSelectComboBox
        /// </summary>
        public MultiSelectComboBox()
        {
            //            ClearSelectionButtonVisibility = Visibility.Collapsed;

#if !__WASM__
//            string xaml =
//                @"<DataTemplate 
//                xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
//                xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
//                xmlns:local=""local:TheHub.Wasm.Controls.CheckableCombo"">
//                <TextBlock TextWrapping=""Wrap"" local:MultiSelectComboBoxService.SelectionBoxLoaded=""True"" />
//                </DataTemplate>";
//            
//            var selectionBoxTemplate = (DataTemplate)XamlReader.Load(xaml);
//            SelectionBoxTemplate = selectionBoxTemplate;
////            EmptySelectionBoxTemplate = selectionBoxTemplate;
//
//            // this is in StyleWorkarounds
//            string xaml =
//                @"<DataTemplate 
//                xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
//                xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""    
//                xmlns:local=""using:TheHub.Wasm.Controls.CheckableCombo"">
//                <CheckBox local:MultiSelectComboBoxService.ComboBoxItemLoaded=""True""
//                    IsChecked=""{Binding Path=(local:MultiSelectComboBoxService.IsChecked), Mode=TwoWay, RelativeSource={RelativeSource Self}}"" />
//                </DataTemplate>";
//            ItemTemplate = (DataTemplate)XamlReader.Load(xaml);
#endif
            SelectedItems = new ObservableCollection<object>();
            ContentPresenterVisible = Visibility.Collapsed;
            //            IsDropDownTabNavigationEnabled = false;
            Debug.WriteLine("multiselect created");
//            ItemTemplateSelector = new MultiSelectItemTemplateSelector();
//            IsItemItsOwnContainerOverride = false;
        }

        #endregion

        #region Propriétés

        /// <summary>
        /// DisplayBindingPath Property
        /// </summary>
        public static readonly DependencyProperty DisplayBindingPathProperty = DependencyProperty.Register(
           "DisplayBindingPath", typeof(string), typeof(MultiSelectComboBox), new PropertyMetadata(null, (obj, e) =>
           {
//               TextSearch.SetTextPath(obj, e.NewValue as string);
           }));
        /// <summary>
        /// Gets or sets the display member path (we can't reuse DisplayMemberPath property)
        /// </summary>
        public string DisplayBindingPath
        {
            get { return GetValue(DisplayBindingPathProperty) as string; }
            set { SetValue(DisplayBindingPathProperty, value); }
        }

        // this property allows the manual firing of the propertychanged event so that changes to the contents can be picked up by the service
        // Maybe there is a way to manually fire the same for the dependency property below, but I'm on the train and dont have an internet collection
        // to look it up.
        public ObservableCollection<object> SelectedItemsContents
        {
            get { return SelectedItems; }
        }

        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
            "SelectedItems", typeof(ObservableCollection<object>), typeof(MultiSelectComboBox), new PropertyMetadata(null, (obj, e) =>
            {
                var combo = obj as MultiSelectComboBox;
                combo.HookNewValues(e.OldValue as ObservableCollection<object>, e.NewValue as ObservableCollection<object>);
            }));

        public ObservableCollection<object> SelectedItems
        {
            get { return GetValue(SelectedItemsProperty) as ObservableCollection<object>; }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public static readonly DependencyProperty SelectionBoxTemplateProperty = DependencyProperty.Register(
            "SelectionBoxTemplate", typeof(DataTemplate), typeof(MultiSelectComboBox), new PropertyMetadata(default(DataTemplate), (o, args) =>
            {
                var cm = (MultiSelectComboBox)o;
//                ((MultiSelectItemTemplateSelector)cm.ItemTemplateSelector).SelectionBoxTemplate = cm.SelectionBoxTemplate;
            }));

        public static readonly DependencyProperty ContentPresenterVisibleProperty = DependencyProperty.Register(
            "ContentPresenterVisible", typeof(Visibility), typeof(MultiSelectComboBox), new PropertyMetadata(default(Visibility)));

        public Visibility ContentPresenterVisible
        {
            get { return (Visibility) GetValue(ContentPresenterVisibleProperty); }
            set { SetValue(ContentPresenterVisibleProperty, value); }
        }

        public DataTemplate SelectionBoxTemplate
        {
            get { return (DataTemplate) GetValue(SelectionBoxTemplateProperty); }
            set { SetValue(SelectionBoxTemplateProperty, value); }
        }

        public static readonly DependencyProperty PopupItemTemplateProperty = DependencyProperty.Register(
            "PopupItemTemplate", typeof(DataTemplate), typeof(MultiSelectComboBox), new PropertyMetadata(default(DataTemplate), (o, args) =>
            {
                var cm = (MultiSelectComboBox) o;
//                ((MultiSelectItemTemplateSelector) cm.ItemTemplateSelector).PopupTemplate = cm.PopupItemTemplate;
            }));

        public DataTemplate PopupItemTemplate
        {
            get { return (DataTemplate)GetValue(PopupItemTemplateProperty); }
            set { SetValue(PopupItemTemplateProperty, value); }
        }

        ObservableCollection<object> selectedValues;

        ObservableCollection<object> hookingValues;

        /// <summary>
        /// Gets the selected values
        /// </summary>
        public ObservableCollection<object> SelectedValues
        {
            get
            {
                if (selectedValues == null)
                {
                    selectedValues = new ObservableCollection<object>();
                    selectedValues.CollectionChanged += SelectedValuesCollectionChanged;
                }
                return selectedValues;
            }
        }

        public static readonly DependencyProperty SelectedDisplayNamesProperty = DependencyProperty.Register(
            "SelectedDisplayNames", typeof(string), typeof(MultiSelectComboBox), new PropertyMetadata(default(string)));

        public string SelectedDisplayNames
        {
            get { return (string) GetValue(SelectedDisplayNamesProperty); }
            set { SetValue(SelectedDisplayNamesProperty, value); }
        }

        void HookNewValues(ObservableCollection<object> oldCollection, ObservableCollection<object> newCollection)
        {
            if (oldCollection != null)
            {
                oldCollection.CollectionChanged -= SelectedItemsCollectionChanged;
            }
            if (newCollection != null)
            {
                newCollection.CollectionChanged += SelectedItemsCollectionChanged;
            }
            hookingValues = newCollection;
            SelectedItemsCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            hookingValues = null;
        }
        //
        //		public ObservableCollection<object> SelectedValues
        //		{
        //			get { return GetValue(SelectedValuesProperty) as ObservableCollection<object>; }
        //			set { SetValue(SelectedValuesProperty, value); }
        //		}

        #endregion

        #region Methods

        /// <summary>
        /// Called when the Items property changed
        /// </summary>
        /// <param name="e">change informations</param>
        protected override void OnItemsChanged(object e)
        {
            base.OnItemsChanged(e);

            int idx;
            var selectedItems = SelectedItems;
//            var te = e as NotifyCollectionChangedEventArgs; // ItemsChangedEventArgs maybe?
//            switch (te.Action)
//            {
//                case NotifyCollectionChangedAction.Add:
//                case NotifyCollectionChangedAction.Replace:
//                case NotifyCollectionChangedAction.Reset:
                    var items = Items;
                    foreach (object value in SelectedValues)
                    {
                        foreach (object item in items.EmptyIfNull())
                        {
                            if (GetSelectedValue(item).Equals(value) && !selectedItems.Contains(item))
                            {
                                selectedItems.Add(item);
                            }
                        }
                    }
//                    break;
//                case NotifyCollectionChangedAction.Remove:
//                    foreach (object item in te.OldItems)
//                    {
//                        idx = selectedItems.IndexOf(item);
//                        if (idx >= 0)
//                        {
//                            selectedItems.RemoveAt(idx);
//                        }
//                    }
//                    break;
//            }
        }

        private void RemoveCollectionChangedEvents()
        {
            SelectedItemsUpdateSuspended = true;
            if (SelectedItems != null) SelectedItems.CollectionChanged -= SelectedItemsCollectionChanged;
            SelectedValues.CollectionChanged -= SelectedValuesCollectionChanged;
        }

        private void AddCollectionChangedEvents()
        {
            if (SelectedItems != null) SelectedItems.CollectionChanged += SelectedItemsCollectionChanged;
            SelectedValues.CollectionChanged += SelectedValuesCollectionChanged;
            SelectedItemsUpdateSuspended = false;
        }

        private void SelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (SelectedValuePath != null)
            {
                RemoveCollectionChangedEvents();
                try
                {
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            AddSelectedValues(e.NewItems);
                            break;
                        case NotifyCollectionChangedAction.Remove:
                            RemoveSelectedValues(e.OldItems);
                            break;
                        case NotifyCollectionChangedAction.Replace:
                            RemoveSelectedValues(e.OldItems);
                            AddSelectedValues(e.NewItems);
                            break;
                        case NotifyCollectionChangedAction.Reset:
                            SelectedValues.Clear();
                            foreach (object item in Items)
                            {
                                UpdateSelectedItem(item, false);
                            }
                            AddSelectedValues(hookingValues);
                            break;
                    }
                }
                finally
                {
                    AddCollectionChangedEvents();
                }
                UpdateSource();
            }
            SelectedDisplayNames = Convert();
        }

        private void RemoveSelectedValues(IList items)
        {
            foreach (var item in items)
            {
                SelectedValues.Remove(GetSelectedValue(item));
                UpdateSelectedItem(item, false);
            }
        }

        private void AddSelectedValues(IList items)
        {
            if (items != null)
            {
                object selectedValue;
                foreach (var item in items)
                {
                    selectedValue = GetSelectedValue(item);
                    if (!SelectedValues.Contains(selectedValue))
                    {
                        SelectedValues.Add(selectedValue);
                    }
                    UpdateSelectedItem(item, true);
                }
            }
            UpdateComboBoxSelectedItem();
        }

        void UpdateComboBoxSelectedItem()
        {
            if (SelectedItems.IsNullOrEmpty())
            {
                SelectedIndex = -1;
            }
            else SelectedItem = SelectedItems.First();
        }

        object GetSelectedValue(object item)
        {
            if (item == null) return null;
            if (SelectedValuePath == ".") return item;
            return DataControlHelper.GetPropertyInfo(item.GetType(), SelectedValuePath)?.GetValue(item, null);
        }

        private void SelectedValuesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RemoveCollectionChangedEvents();
            try
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        AddSelectedItems(e.NewItems);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        RemoveSelectedItems(e.OldItems);
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        RemoveSelectedItems(e.OldItems);
                        AddSelectedItems(e.NewItems);
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        var selectedItems = SelectedItems.ToList();
                        SelectedItems.Clear();
                        foreach (object item in selectedItems)
                        {
                            UpdateSelectedItem(item, false);
                        }
                        AddSelectedItems(e.NewItems);
                        break;
                }
            }
            finally
            {
                AddCollectionChangedEvents();
            }
            SelectedDisplayNames = Convert();
        }

        public string Convert()
        {
            Debug.WriteLine("converting");
            if (String.IsNullOrWhiteSpace(DisplayBindingPath) || SelectedItems == null)
            {
                return String.Empty;
            }

            PropertyInfo propertyInfo;
            return string.Join(", ", SelectedItems.Select(item =>
            {
                if (DisplayBindingPath == ".") return item;
                propertyInfo = DataControlHelper.GetPropertyInfo(item.GetType(), DisplayBindingPath);
                if (propertyInfo == null)
                {
                    return String.Empty;
                }
                return propertyInfo.GetValue(item, null);
            }).ToArray());
        }

        public void UpdateSource()
        {
            var binding = GetBindingExpression(SelectedItemsProperty);
            if(binding != null) binding.UpdateSource();
        }

        private void RemoveSelectedItems(IList values)
        {
            object item;
            foreach (var value in values)
            {
                item = SelectedItems.FirstOrDefault(e => GetSelectedValue(e).Equals(value));
                if (item != null)
                {
                    SelectedItems.Remove(item);
                    UpdateSelectedItem(item, false);
                }
            }
        }

        private void AddSelectedItems(IList values)
        {
            if (values != null)
            {
                object item;
                foreach (var value in values)
                {
                    item = Items.FirstOrDefault(e => GetSelectedValue(e).Equals(value));
                    if (item != null)
                    {
                        SelectedItems.Add(item);
                        UpdateSelectedItem(item, true);
                    }
                }
            }
        }

        private void UpdateSelectedItem(object item, bool select)
        {
            var obj = ContainerFromItem(item);
            if (obj != null)
            {
                var cb = obj.FindChildByType<CheckBox>();
                if (cb != null && cb.IsChecked != select)
                {
                    cb.IsChecked = select;
                }
            }
        }

        public bool SelectedItemsUpdateSuspended { get; set; }

        public void UpdateSelectedContainer(ToggleButton checkBox, MultiSelectComboBoxItem container, bool select)
        {
            foreach (var si in SelectedItems)
            {
                var x = ContainerFromItem(si);
                Debug.WriteLine(x);
            }
            var obj = ItemFromContainer(checkBox.GetVisualParent<MultiSelectComboBoxItem>());
            if (obj != null)
            {
                checkBox.IsChecked = SelectedItems.Any(si => si == obj);
            }
        }


        /// <summary>
        /// Create a new ComboBox item
        /// </summary>
        /// <returns>a new ComboBox item</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MultiSelectComboBoxItem(this);
        }

//        protected override void OnKeyDown(KeyEventArgs e)
//        {
//            base.OnKeyDown(e);
//            IsDropDownOpen = true;
//        }

        #endregion

//        protected override bool HandleKeyDown(Key systemKey, int platformKeyCode)
//        {
//            if (systemKey == Key.Space)
//            {
//                foreach (var item in Items)
//                {
//                    var container = ItemContainerGenerator.ContainerFromItem(item) as RadComboBoxItem;
//                    if (container != null && container.IsHighlighted)
//                    {
//                        var cb = container.FindChildByType<CheckBox>();
//                        if (cb != null)
//                        {
//                            cb.IsChecked = !cb.IsChecked;
//                        }
//                    }
//                }
//            }
//            if (systemKey == Key.Escape && IsDropDownOpen)
//            {
//                SelectedValues.Clear();
//                if (oldValues != null)
//                {
//                    foreach (var v in oldValues)
//                    {
//                        SelectedValues.Add(v);
//                    }
//                }
//            }
//
//            return base.HandleKeyDown(systemKey, platformKeyCode);
//        }

        List<object> oldValues;
        protected override void OnDropDownOpened(object e)
        {
            Debug.WriteLine("drop down opened");
            base.OnDropDownOpened(e);
            oldValues = new List<object>(SelectedValues);
        }
    }

//    public class MultiSelectItemTemplateSelector : DataTemplateSelector
//    {
//        public DataTemplate PopupTemplate { get; set; }
//        public DataTemplate SelectionBoxTemplate { get; set; }
//
//        protected override DataTemplate SelectTemplateCore(object item)
//        {
//            return base.SelectTemplateCore(item);
//        }
//
//        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
//        {
//            if (container is ContentPresenter) return SelectionBoxTemplate;
//            return PopupTemplate;
//        }
//    }
}