using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace UnoMultiselect.Shared
{
    // copied from Telerik Wasm
    public static class ParentOfTypeExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> enumerable)
        {
            return enumerable ?? Enumerable.Empty<T>();
        }

        public static IEnumerable EmptyIfNull(this IEnumerable enumerable)
        {
            return enumerable ?? Enumerable.Empty<object>();
        }

        public static T FindChildByType<T>(this DependencyObject element) where T : DependencyObject
        {
            return Enumerable.FirstOrDefault<T>(element.ChildrenOfType<T>());
        }

        public static IEnumerable<T> ChildrenOfType<T>(this DependencyObject element) where T : DependencyObject
        {
            return Enumerable.OfType<T>(element.GetChildrenRecursive());
        }

        private static IEnumerable<DependencyObject> GetChildrenRecursive(this DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(element, i);
                if (child != null)
                {
                    yield return child;
                    using (IEnumerator<DependencyObject> enumerator = child.GetChildrenRecursive().GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            DependencyObject current = enumerator.Current;
                            yield return current;
                        }
                    }
                }
            }
            yield break;
        }

        public static T GetVisualParent<T>(this DependencyObject element) where T : DependencyObject
        {
            return element.ParentOfType<T>();
        }

        public static T ParentOfType<T>(this DependencyObject element) where T : DependencyObject
        {
            if (element == null)
            {
                return default(T);
            }
            return Enumerable.FirstOrDefault<T>(Enumerable.OfType<T>(element.GetParents()));
        }

        public static IEnumerable<DependencyObject> GetParents(this DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            while ((element = element.GetParent()) != null)
            {
                yield return element;
            }
        }

        private static DependencyObject GetParent(this DependencyObject element)
        {
            DependencyObject dependencyObject = null;
            try
            {
                dependencyObject = VisualTreeHelper.GetParent(element);
            }
            catch (InvalidOperationException)
            {
                dependencyObject = null;
            }
            if (dependencyObject == null)
            {
                FrameworkElement frameworkElement = element as FrameworkElement;
                if (frameworkElement != null)
                {
                    dependencyObject = frameworkElement.Parent;
                }
            }
            return dependencyObject;
        }
    }
}
