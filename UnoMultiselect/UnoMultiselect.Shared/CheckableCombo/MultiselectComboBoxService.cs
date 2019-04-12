using System.Collections;
using System.Windows;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using UnoMultiselect.Shared;

namespace TheHub.Silverlight.Controls.CheckableCombo
{
    /// <summary>
    /// Service for the MultiSelect comboBox
    /// </summary>
    public static class MultiSelectComboBoxService
    {
        /// <summary>
        /// IsChecked property
        /// </summary>
        public static DependencyProperty IsCheckedProperty = DependencyProperty.RegisterAttached("IsChecked",
            typeof(bool), typeof(MultiSelectComboBoxService), new PropertyMetadata(false, (obj, e) =>
            {
                MultiSelectComboBoxItem comboBoxItem = obj.GetVisualParent<MultiSelectComboBoxItem>();
                if (comboBoxItem != null)
                {
                    MultiSelectComboBox comboBox = comboBoxItem.ParentComboBox;
                    if (comboBox.SelectedItemsUpdateSuspended) return;
                    var selectedItems = (IList)comboBox.SelectedItems;
                    if (selectedItems == null) return;
                    object item = comboBoxItem.DataContext;
                    if ((bool)e.NewValue)
                    {
                        if (!selectedItems.Contains(item))
                        {
                            selectedItems.Add(item);
                        }
                    }
                    else
                    {
                        selectedItems.Remove(item);
                    }
                }
            }));

        /// <summary>
        /// Gets a value indicating if the object is checked or not
        /// </summary>
        /// <param name="obj">DependencyObject</param>
        /// <returns>a value indicating if the object is checked or not</returns>
        public static bool GetIsChecked(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsCheckedProperty);
        }

        /// <summary>
        /// Sets a value indicating if the object is checked or not
        /// </summary>
        /// <param name="obj">DependencyObject</param>
        /// <param name="value">the value indicating if the object is checked or not</param>
        public static void SetIsChecked(DependencyObject obj, bool value)
        {
            obj.SetValue(IsCheckedProperty, value);
        }

        /// <summary>
        /// SelectionBoxLoaded property called on SelectionBox load
        /// </summary>
        public static DependencyProperty SelectionBoxLoadedProperty = DependencyProperty.RegisterAttached("SelectionBoxLoaded",
            typeof(bool), typeof(MultiSelectComboBoxService), new PropertyMetadata(false, (obj, e) =>
            {
                TextBlock targetElement = obj as TextBlock;
                if (targetElement != null)
                {
                    targetElement.Loaded += new RoutedEventHandler(targetElement_Loaded);
                }
            }));

        private static void targetElement_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock targetElement = (TextBlock)sender;
            targetElement.Loaded -= new RoutedEventHandler(targetElement_Loaded);
            MultiSelectComboBox comboBox = targetElement.GetVisualParent<MultiSelectComboBox>();
            if (comboBox != null)
            {
                targetElement.SetBinding(TextBlock.TextProperty, new Binding()
                {
                    Path = "SelectedItemsContents",
                    Converter = new MultiSelectComboBoxConverter(),
                    Source = comboBox,
                    ConverterParameter = comboBox.DisplayBindingPath
                });
            }
        }

        /// <summary>
        /// Gets the value indicating if the object is loaded or not
        /// </summary>
        /// <param name="obj">DependencyObject</param>
        /// <returns>the value indicating if the object is loaded or not</returns>
        public static bool GetSelectionBoxLoaded(DependencyObject obj)
        {
            return (bool)obj.GetValue(SelectionBoxLoadedProperty);
        }

        /// <summary>
        /// Sets the value indicating if the object is loaded or not
        /// </summary>
        /// <param name="obj">DependencyObject</param>
        /// <param name="value">the value indicating if the object is loaded or not</param>
        public static void SetSelectionBoxLoaded(DependencyObject obj, bool value)
        {
            obj.SetValue(SelectionBoxLoadedProperty, value);
        }

        /// <summary>
        /// ComboBoxItemLoaded called on ComboBoxItem load
        /// </summary>
        public static DependencyProperty ComboBoxItemLoadedProperty = DependencyProperty.RegisterAttached("ComboBoxItemLoaded",
            typeof(bool), typeof(MultiSelectComboBoxService), new PropertyMetadata(false, (obj, e) =>
            {
                CheckBox targetElement = obj as CheckBox;
                if (targetElement != null)
                {
                    targetElement.Loaded += new RoutedEventHandler(comboBoxItem_Loaded);
                    targetElement.SetBinding(MultiSelectComboBoxService.DataContextProperty, new Binding());
                }
            }));

        private static void comboBoxItem_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            MultiSelectComboBox comboBox = GetComboBox(element);
            if (comboBox != null)
            {
                element.SetBinding(CheckBox.ContentProperty, new Binding
                                                             {
                                                                 Path = comboBox.DisplayBindingPath
                                                             });
                comboBox.UpdateSelectedContainer(element as ToggleButton, element.GetVisualParent<MultiSelectComboBoxItem>(), true);
            }
        }

        /// <summary>
        ///Gets the value indicating if the item is loaded or not
        /// </summary>
        /// <param name="obj">DependencyObject</param>
        /// <returns>the value indicating if the item is loaded or not</returns>
        public static bool GetComboBoxItemLoaded(DependencyObject obj)
        {
            return (bool)obj.GetValue(ComboBoxItemLoadedProperty);
        }

        /// <summary>
        /// Sets the value indicating if the item is loaded or not
        /// </summary>
        /// <param name="obj">DependencyObject</param>
        /// <param name="value">the value indicating if the item is loaded or not</param>
        public static void SetComboBoxItemLoaded(DependencyObject obj, bool value)
        {
            obj.SetValue(ComboBoxItemLoadedProperty, value);
        }

        private static MultiSelectComboBox GetComboBox(DependencyObject targetElement)
        {
            MultiSelectComboBoxItem item = targetElement.GetVisualParent<MultiSelectComboBoxItem>();
            if (item != null)
            {
                return item.ParentComboBox;
            }
            return null;
        }

        private static DependencyProperty DataContextProperty = DependencyProperty.RegisterAttached("DataContext",
            typeof(object), typeof(MultiSelectComboBoxService), new PropertyMetadata(null, (obj, e) =>
            {
                CheckBox checkBox = (CheckBox)obj;
                MultiSelectComboBox comboBox = GetComboBox(checkBox);
                if (comboBox != null && comboBox.SelectedItems != null)
                {
                    checkBox.IsChecked = comboBox.SelectedItems.Contains(checkBox.DataContext);
                }
            }));
        private static object GetDataContext(DependencyObject obj)
        {
            return obj.GetValue(DataContextProperty);
        }
        private static void SetDataContext(DependencyObject obj, object value)
        {
            obj.SetValue(DataContextProperty, value);
        }
    }
}
