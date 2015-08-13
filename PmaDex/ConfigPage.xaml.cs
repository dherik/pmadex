using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace PmaDex
{
    public partial class ConfigPage : PhoneApplicationPage
    {
        public ConfigPage()
        {
            InitializeComponent();
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/Pages/ConfigLoginPage.xaml", UriKind.Relative));
            });
        }

        private void lstConfigOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lstBox = sender as ListBox;
            ListBoxItem lbi = lstBox.SelectedItem as ListBoxItem;
            if (lbi != null && lbi.Name == "login")
            {
                Dispatcher.BeginInvoke(() =>
                {
                    NavigationService.Navigate(new Uri("/Pages/ConfigLoginPage.xaml", UriKind.Relative));
                });
            }

            if (lbi != null && lbi.Name == "apontamento")
            {
                Dispatcher.BeginInvoke(() =>
                {
                    NavigationService.Navigate(new Uri("/Pages/ConfigAppointmentPage.xaml", UriKind.Relative));
                });
            }

            lstBox.SelectedIndex = -1;
        }
    }
}