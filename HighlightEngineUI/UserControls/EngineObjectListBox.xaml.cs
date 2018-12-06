using HighlightEngineUI.Models;
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

namespace HighlightEngineUI
{
    /// <summary>
    /// Interaction logic for EngineObjectListBox.xaml
    /// </summary>
    public partial class EngineObjectListBox : UserControl
    {
        public EngineObjectListBox()
        {
            InitializeComponent();
        }

        public event Action<string> MouseDownEventFire;

        private string GetLabelContentFromPanel(object stackPanel)
        {
            string labelContent = null;
            StackPanel senderStackPanel = stackPanel as StackPanel;
            Label innerLabel = senderStackPanel.Children[1] as Label;
            labelContent = innerLabel.Content as string;
            return labelContent;
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string labelContent = GetLabelContentFromPanel(sender);  
            MouseDownEventFire?.Invoke(labelContent);
        }
    }
}
