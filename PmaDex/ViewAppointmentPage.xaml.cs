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
using PmaDex.Util;

namespace PmaDex
{
    public partial class ViewAppointmentPage : PhoneApplicationPage
    {
        public ViewAppointmentPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DailyAppointment dailyAppointment = PhoneApplicationService.Current.State["dailyAppointment"] as DailyAppointment;
            LoadAppointment(dailyAppointment);
        }
        
        private async void LoadAppointment(DailyAppointment dailyAppointment)
        {
            this.txtStart.Text = dailyAppointment.Inicio;
            this.txtEnd.Text = dailyAppointment.Fim;
            this.txtInterval.Text = dailyAppointment.Intervalo;

            this.txtDate.Text = dailyAppointment.Data;
            
            PmaServices pma = new PmaServices();
            List<Appointment> app = await pma.FindAppointments(TokenUtil.GetToken(), dailyAppointment.Data);
            this.llsAppointment.ItemsSource = app.ToArray();
        }

    }
}