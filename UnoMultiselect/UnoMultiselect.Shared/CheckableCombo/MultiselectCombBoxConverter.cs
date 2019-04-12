using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Windows.UI.Xaml.Data;

namespace TheHub.Silverlight.Controls.CheckableCombo
{
    public class MultiSelectComboBoxConverter : IValueConverter
    {
        #region Méthodes

        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            string displayMemberPath = parameter as string;
            if (String.IsNullOrWhiteSpace(displayMemberPath) || value == null)
            {
                return String.Empty;
            }

            PropertyInfo propertyInfo;
            return string.Join(", ", (value as IEnumerable<object>).Select(item =>
            {
                if (displayMemberPath == ".") return item;
                propertyInfo = DataControlHelper.GetPropertyInfo(item.GetType(), displayMemberPath);
                if (propertyInfo == null)
                {
                    return String.Empty;
                }
                return propertyInfo.GetValue(item, null);
            }).ToArray());
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}