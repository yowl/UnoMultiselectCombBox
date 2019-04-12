using Windows.UI.Xaml.Controls;

namespace TheHub.Silverlight.Controls.CheckableCombo
{
    /// <summary>
    /// Item for MultiSelectComboBox
    /// </summary>
    public class MultiSelectComboBoxItem : ComboBoxItem
    {
        #region Constructor

        /// <summary>
        ///  Initializes a new instance of MultiSelectComboBoxItem class
        /// </summary>
        /// <param name="parent">parent comboBox</param>
        public MultiSelectComboBoxItem(MultiSelectComboBox parent)
        {
            ParentComboBox = parent;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the parent comboBox
        /// </summary>
        public MultiSelectComboBox ParentComboBox
        {
            get;
            private set;
        }

        #endregion
    }
}
