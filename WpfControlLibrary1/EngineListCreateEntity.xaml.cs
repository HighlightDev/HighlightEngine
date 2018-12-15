﻿using System;
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
    /// Interaction logic for EngineListCreateEntity.xaml
    /// </summary>
    public partial class EngineListCreateEntity : UserControl
    {
        public Action<object, string> MainContent_mouseDownAction;

        public EngineListCreateEntity()
        {
            InitializeComponent();
        }

        private void scrollviewer_1_MouseEnter(object sender, MouseEventArgs e)
        {
            var scrollViewer = (sender as ScrollViewer);
            scrollViewer?.Focus();
        }

        private void stackpanel_mainContent_MouseDown(object sender, MouseButtonEventArgs e)
        {
            StackPanel eventSender = sender as StackPanel;
            if (eventSender != null)
            {
                string eventSenderName = eventSender.Name;
                MainContent_mouseDownAction?.Invoke(eventSender, eventSenderName);
            }

            StackPanel senderStackPanel = sender as StackPanel;
            object prop = senderStackPanel.FindName("stackpanel_templatedContent");
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
    }
}
