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
using PmaDex.Util;

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
                TokenUtil.SaveToken(token);

                if (!isInit)
                {
                    loadProjects(token);
                    this.dpkDate.Value = DateTime.Now;
                    isInit = true;
                }
            }

            if (PhoneApplicationService.Current.State.ContainsKey("pmaActivity"))
            {
                PmaActivity pmaActivity = PhoneApplicationService.Current.State["pmaActivity"] as PmaActivity;
                PhoneApplicationService.Current.State.Remove("pmaActivity");
                lstActivitiesAdv.Items.Add(pmaActivity);
            }

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

            string token = TokenUtil.GetToken();

            string id = pmaProject.Id;
            PmaServices pmaServices = new PmaServices();
            List<PmaActivity> activities = await pmaServices.loadActivities(token, id);

            this.lpkActivities.ItemsSource = activities.ToArray();
        }
        
        private void setProgressIndicator(bool value)
        {
            SystemTray.ProgressIndicator.IsIndeterminate = value;
            SystemTray.ProgressIndicator.IsVisible = value;
        }

        
        private void btnSave_Click(object sender, EventArgs e)
        {
            string token = TokenUtil.GetToken();

            var item = lpkActivities.SelectedItem;
            PmaActivity pmaActivity = (PmaActivity)item;
            if (pmaActivity == null)
            {
                MessageBox.Show("Não foram encontradas atividades");
                return;
            }

            string day = dpkDate.FormatToDiaMesAno();
            string effortInMinutes = tpkEffort.GetEffortInMinutes();

            PmaServices pmaServices = new PmaServices();
            pmaServices.CreateTheOneAppointment(token, day, tpkStartHour.ValueString, tpkEndHour.ValueString,
                tpkRest.ValueString, effortInMinutes, pmaActivity.id);
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
            //List<Appointment> appointments = new List<Appointment>();
            //appointments.Add(new Appointment
            //{
            //    Cliente = "cliente",
            //    Projeto = "projeto",
            //    Atividade = "atividade",
            //    AtividadeStatus = "atividade status",
            //    Descricao = "descricao",
            //    Esforco = "04:00"
            //});
            //this.llsActivityAppointment.ItemsSource = appointments.ToArray();

            Dispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/AddActivityAdvancedPage.xaml", UriKind.Relative));
            });

        }

        private async void btnSaveAdv_Click(object sender, EventArgs e)
        {

            //criar CreateDayAppointment e salvar
            //criar CreateDayAppointment para cada atividade da lista


            //TODO validar se tem atividades adicionadas
            PmaServices pmaServices = new PmaServices();
            string response = await pmaServices.createDayAppointment(TokenUtil.GetToken(), dpkDateAdv.FormatToDiaMesAno(), tpkStartHourAdv.ValueString, tpkEndHourAdv.ValueString, tpkRestAdv.ValueString);
            //TODO tratar response

            lstActivitiesAdv.Items.ToList().ForEach(async x =>
            {
                var atividade = x as PmaActivity;
                response = await pmaServices.CreateAppointment(TokenUtil.GetToken(), dpkDateAdv.FormatToDiaMesAno(), atividade.id, atividade.Effort, atividade.Descricao);
                //TODO tratar response
            });

            //foreach (object o in lstActivitiesAdv.Items)
            //{
            //    if(o is PmaActivity) 
            //    {
            //    }
            //}
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

        private void llsActivityAppointment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}