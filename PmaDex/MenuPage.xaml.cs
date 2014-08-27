using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Net.Http;
using System.Xml.Linq;
using System.Collections;
using System.IO.IsolatedStorage;
using System.Collections.ObjectModel;

namespace PmaDex
{
    public partial class OptionsPage : PhoneApplicationPage
    {

        ProgressIndicator progressIndicator = new ProgressIndicator() { IsVisible = true, IsIndeterminate = false, Text = "Consultando..." };

        private bool isInit { get; set; }

        public OptionsPage()
        {
            InitializeComponent();
            isInit = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("token"))
            {
                string token = NavigationContext.QueryString["token"];
                saveTokenToIsolatedStorage(token);

                if (!isInit)
                {
                    loadProjects(token);
                    this.dpkDate.Value = DateTime.Now;
                    isInit = true;
                }
            }
        }

        private static void saveTokenToIsolatedStorage(string token)
        {
            // save value
            IsolatedStorageSettings.ApplicationSettings["token"] = token;
        }

        private async void loadProjects(string token)
        {
            PmaServices pmaServices = new PmaServices();
            List<PmaProject> projectsList = await pmaServices.loadProjects(token);

            this.lpkProjects.ItemsSource = projectsList.ToArray();
        }

        private async void lpkProjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (sender as ListPicker).SelectedItem;
            PmaProject pmaProject = (PmaProject) item;
            if (pmaProject == null)
            {
                return;
            }

            string token = getTokenFromIsolatedStorage();

            string id = pmaProject.id;
            PmaServices pmaServices = new PmaServices();
            List<PmaActivity> activities = await pmaServices.loadActivities(token, id);

            this.lpkActivities.ItemsSource = activities.ToArray();


        }
        
        private string getMinutes()
        {
            DateTime effortDt = (DateTime)tpkEffort.Value;
            DateTime baseDt = new DateTime(effortDt.Year, effortDt.Month, effortDt.Day, 0, 0, 0, 0, effortDt.Kind);
            string minutes = (effortDt.Subtract(baseDt).TotalMinutes).ToString();
            return minutes;
        }

        private static string getTokenFromIsolatedStorage()
        {
            string token = IsolatedStorageSettings.ApplicationSettings.Contains("token")
                ? (string)IsolatedStorageSettings.ApplicationSettings["token"]
                : ""; // false is default value 
            return token;
        }

        private void setProgressIndicator(bool value)
        {
            SystemTray.ProgressIndicator.IsIndeterminate = value;
            SystemTray.ProgressIndicator.IsVisible = value;
        }

        
        private void btnSave_Click(object sender, EventArgs e)
        {
            string token = getTokenFromIsolatedStorage();

            var item = lpkActivities.SelectedItem;
            PmaActivity pmaActivity = (PmaActivity)item;
            if (pmaActivity == null)
            {
                MessageBox.Show("Não foram encontradas atividades");
                return;
            }

            //DateTime startDate = DateTime.Now;
            //String effort = ((DateTime)tpkEffort.Value).Hour.ToString() + ":" + ((DateTime)tpkEffort.Value).Minute.ToString();
            //String effort = String.Format("{0:HH mm}", (DateTime)tpkEffort.Value);
            //string minutes = (date2.Subtract(date1).TotalMinutes).ToString();
            //DateTime date1 = new DateTime(0, 1, 1, 22, 59, 0);
            //DateTime date2 = new DateTime(0, 1, 1, 22, 59, 0);
            //TimeSpan ts = new TimeSpan(10, 30, 0);
            string day = String.Format("{0:yyyy-MM-dd}", dpkDate.Value);

            string minutes = getMinutes();

            PmaServices pmaServices = new PmaServices();
            pmaServices.CreateTheOneAppointment(token, day, tpkStartHour.ValueString, tpkEndHour.ValueString,
                tpkRest.ValueString, minutes, pmaActivity.id);
        }

        private void btnList_Click(object sender, EventArgs e)
        {
            GoToListAppointmentsPage();
        }

        private void GoToListAppointmentsPage()
        {
            Dispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/ListAppointmentsPage.xaml", UriKind.Relative));
            });
        }

        /*
            DateTime departure = new DateTime(2010, 6, 12, 18, 32, 0);
            DateTime arrival = new DateTime(2010, 6, 13, 22, 47, 0);
            TimeSpan travelTime = arrival - departure;  
            Console.WriteLine("{0} - {1} = {2}", arrival, departure, travelTime);  
         */
        private void calculateEffortTip()
        {

            if (this.tpkStartHour == null || this.tpkEndHour == null || this.tpkRest == null)
            {
                return;
            }

            DateTime startHour = (DateTime)tpkStartHour.Value;
            DateTime endHour = (DateTime)tpkEndHour.Value;
            DateTime rest = (DateTime)tpkRest.Value;
            TimeSpan effort = endHour.Subtract(startHour);
            effort = effort.Subtract(new TimeSpan(rest.Hour, rest.Minute, 0));

            //this.txtEffortTip.Text = effort.ToString();
            DateTime effortDate = (DateTime)tpkEffort.Value;
            DateTime dateOnly = effortDate.Date;
            DateTime date = dateOnly + effort;
            this.tpkEffort.Value = date;
        }

        private void tpkStartHour_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            calculateEffortTip();
        }

        private void tpkEndHour_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            calculateEffortTip();
        }

        private void tpkRest_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            calculateEffortTip();
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            GoToConfigPage();
        }


        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((Pivot)sender).SelectedIndex)
            {
                case 0:
                    this.ApplicationBar = this.Resources["AppBarSimple"] as ApplicationBar;
                    break;

                case 1:
                    this.ApplicationBar = this.Resources["AppBarAdvanced"] as ApplicationBar;
                    break;
            }
        }

        private void btnListAdv_Click(object sender, EventArgs e)
        {
            GoToListAppointmentsPage();
        }

        private void btnAddAdv_Click(object sender, EventArgs e)
        {

        }

        private void btnSaveAdv_Click(object sender, EventArgs e)
        {

        }

        private void btnConfigAdv_Click(object sender, EventArgs e)
        {
            GoToConfigPage();
        }

        private void GoToConfigPage()
        {
            Dispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/ConfigPage.xaml", UriKind.Relative));
            });
        }

    }
}