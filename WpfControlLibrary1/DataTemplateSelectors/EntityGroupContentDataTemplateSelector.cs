using System.Windows;
using System.Windows.Controls;

namespace WpfControlLibrary1.DataTemplateSelectors
{
    public class EntityGroupContentDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate result = null;

            FrameworkElement element = container as FrameworkElement;
            string dataTemplateTypeItem = item as string;

            if (element != null && dataTemplateTypeItem != null)
            {
                result = element.FindResource(dataTemplateTypeItem) as DataTemplate;
            }

            return result;
        }
    }
}
