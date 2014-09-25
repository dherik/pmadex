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
    public partial class ViewAppointmentPage : PhoneApplicationPage
    {

        //private bool isInit { get; set; }

        public ViewAppointmentPage()
        {
            InitializeComponent();
            //isInit = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DailyAppointment dailyAppointment = PhoneApplicationService.Current.State["dailyAppointment"] as DailyAppointment;
            loadAppointment(dailyAppointment);
        }

        
        private async void loadAppointment(DailyAppointment dailyAppointment)
        {
            //this.txtResume.Text = dailyAppointment.resume;
            this.txtStart.Text = dailyAppointment.Inicio;
            this.txtEnd.Text = dailyAppointment.Fim;
            this.txtInterval.Text = dailyAppointment.Intervalo;

            this.txtDate.Text = dailyAppointment.Data;
            
            PmaServices pma = new PmaServices();
            List<Appointment> app = await pma.findAppointments(getTokenFromIsolatedStorage(), dailyAppointment.Data);
            this.llsAppointment.ItemsSource = app.ToArray();
        }

        private static string getTokenFromIsolatedStorage()
        {
            string token = IsolatedStorageSettings.ApplicationSettings.Contains("token")
                ? (string)IsolatedStorageSettings.ApplicationSettings["token"]
                : ""; // false is default value 
            return token;
        }
    }
}