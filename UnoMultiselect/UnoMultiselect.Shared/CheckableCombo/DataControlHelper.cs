using System;
using System.Reflection;

namespace TheHub.Silverlight.Controls.CheckableCombo
{
    public static class DataControlHelper
    {
        public static PropertyInfo GetPropertyInfo(Type objectType, string bindingPath)
        {
            if (bindingPath == null)
            {
                return null;
            }

            PropertyInfo propertyInfo = null;
            foreach (string path in bindingPath.Split('.'))
            {
                propertyInfo = objectType.GetProperty(path);
                if (propertyInfo == null)
                {
                    break;
                }
                objectType = propertyInfo.PropertyType;
            }
            return propertyInfo;
        }
    }
}
