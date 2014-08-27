using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;

namespace PmaDex
{
    public partial class ListAppointmentsPage : PhoneApplicationPage
    {

        ProgressIndicator progressIndicator = new ProgressIndicator() { IsVisible = true, IsIndeterminate = false, Text = "Consultando..." };

        public ListAppointmentsPage()
        {
            InitializeComponent();
        }

        private async void btnFind_Click(object sender, EventArgs e)
        {
            PmaServices pmaServices = new PmaServices();
            string startDate = String.Format("{0:yyyy-MM-dd}", dpkStartDate.Value);
            string endDate = String.Format("{0:yyyy-MM-dd}", dpkEndDate.Value);

            SystemTray.SetProgressIndicator(this, progressIndicator);
            setProgressIndicator(true);
            List<DailyAppointment> list = await pmaServices.findDailyAppointments(getTokenFromIsolatedStorage(), startDate, endDate);
            setProgressIndicator(false);

            llsDailyAppointment.ItemsSource = list.ToArray();
        }

        private void setProgressIndicator(bool value)
        {
            SystemTray.ProgressIndicator.IsIndeterminate = value;
            SystemTray.ProgressIndicator.IsVisible = value;
        }

        private static string getTokenFromIsolatedStorage()
        {
            string token = IsolatedStorageSettings.ApplicationSettings.Contains("token")
                ? (string)IsolatedStorageSettings.ApplicationSettings["token"]
                : ""; // false is default value 
            return token;
        }

        private void llsDailyAppointment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (sender as LongListSelector).SelectedItem;
            DailyAppointment dailyAppointment = (DailyAppointment)item;
            if (dailyAppointment == null)
            {
                return;
            }

            PhoneApplicationService.Current.State["dailyAppointment"] = dailyAppointment;
            Dispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/ViewAppointmentPage.xaml", UriKind.Relative));
            });

        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Você deseja realmente remover o apontamento?", "Remover apontamento?", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                //var selected = (sender as MenuItem).DataContext as DailyAppointment;
                MessageBox.Show("Apontamento removido! (não implementado)");
            }
            
        }
    }
}