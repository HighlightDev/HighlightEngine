using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfControlLibrary1
{
    /// <summary>
    /// Interaction logic for EngineEntityListBox.xaml
    /// </summary>
    public partial class EngineEntityListBox : UserControl
    {
        public EngineEntityListBox()
        {
            InitializeComponent();
        }

        #region User Control acquisition methods

        public event Action<string> EntryStackPanelMouseDownEventFire;
        public event Action<string> ModelLoadButtonMouseDownEventFire;

        public Border GetEntryBorder()
        {
            return FindName("border") as Border;
        }

        public ScrollViewer GetScrollViewer()
        {
            return FindName("scrollViewer") as ScrollViewer;
        }

        public Image GetImage()
        {
            // for now
            return null;
        }

        #endregion

        private string GetLabelContentFromPanel(StackPanel stackPanel)
        {
            string labelContent = null;
            Label innerLabel = stackPanel.Children[1] as Label;
            labelContent = innerLabel.Content as string;
            return labelContent;
        }

        private void selectProperyButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string name = (sender as ContentControl).Name.ToString();
            ModelLoadButtonMouseDownEventFire?.Invoke(name);
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            StackPanel senderStackPanel = sender as StackPanel;
            string labelContent = GetLabelContentFromPanel(senderStackPanel);
            EntryStackPanelMouseDownEventFire?.Invoke(labelContent);

            object prop = senderStackPanel.FindName("PropsStackPanel");
            var propsStack = prop as StackPanel;
            if (propsStack.Visibility == Visibility.Collapsed)
            {
                propsStack.Visibility = Visibility.Visible;
            }
            else
            {
                propsStack.Visibility = Visibility.Collapsed;
            }
        }

        private void ScrollViewer_MouseEnter(object sender, MouseEventArgs e)
        {
            scrollViewer.Focus();
        }
    }
}
