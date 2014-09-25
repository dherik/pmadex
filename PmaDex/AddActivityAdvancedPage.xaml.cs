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
    public partial class AddActivityAdvancedPage : PhoneApplicationPage
    {
        public AddActivityAdvancedPage()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/MenuPage.xaml", UriKind.Relative));
            });
        }

        private void lpkProjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}