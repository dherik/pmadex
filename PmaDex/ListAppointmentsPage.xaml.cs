using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PmaDex.Util;

namespace PmaDex
{
    public partial class ListAppointmentsPage : PhoneApplicationPage
    {

        ProgressIndicator ProgressIndicator = new ProgressIndicator() { IsVisible = true, IsIndeterminate = false, Text = "Consultando..." };

        public ListAppointmentsPage()
        {
            InitializeComponent();
        }

        private async void PtnFind_Click(object sender, EventArgs e)
        {
            PmaServices pmaServices = new PmaServices();
            string startDate = dpkStartDate.FormatToDiaMesAno();
            string endDate = dpkEndDate.FormatToDiaMesAno();

            SystemTray.SetProgressIndicator(this, ProgressIndicator);
            setProgressIndicator(true);
            List<DailyAppointment> list = await pmaServices.FindDailyAppointments(TokenUtil.GetToken(), startDate, endDate);
            setProgressIndicator(false);

            llsDailyAppointment.ItemsSource = list.ToArray();
        }

        private void setProgressIndicator(bool value)
        {
            SystemTray.ProgressIndicator.IsIndeterminate = value;
            SystemTray.ProgressIndicator.IsVisible = value;
        }

        private void LlsDailyAppointment_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
                // var selected = (sender as MenuItem).DataContext as DailyAppointment;
                MessageBox.Show("Apontamento removido! (não implementado)");
            }
        }
    }
}