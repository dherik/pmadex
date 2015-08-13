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
    public partial class MenuPage : PhoneApplicationPage
    {
        ProgressIndicator progressIndicator = new ProgressIndicator() { IsVisible = true, IsIndeterminate = false, Text = "Consultando..." };

        private bool IsInit { get; set; }

        public MenuPage()
        {
            InitializeComponent();
            IsInit = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("token"))
            {
                string token = NavigationContext.QueryString["token"];
                TokenUtil.SaveToken(token);

                if (!IsInit)
                {
                    LoadProjects(token);
                    this.dpkDate.Value = DateTime.Now;
                    IsInit = true;
                }
            }

            if (PhoneApplicationService.Current.State.ContainsKey("pmaActivities"))
            {
                List<PmaActivity> activities = PhoneApplicationService.Current.State["pmaActivities"] as List<PmaActivity>;
                ListActivitiesAdvanced.ItemsSource = activities;
            }

        }

        private async void LoadProjects(string token)
        {
            PmaServices pmaServices = new PmaServices();
            List<PmaProject> projectsList = await pmaServices.LoadProjects(token);
            this.lpkProjects.ItemsSource = projectsList.ToArray();
        }

        private async void LpkProjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (sender as ListPicker).SelectedItem;
            PmaProject pmaProject = (PmaProject)item;
            if (pmaProject == null)
            {
                return;
            }

            string token = TokenUtil.GetToken();

            string id = pmaProject.Id;
            PmaServices pmaServices = new PmaServices();
            List<PmaActivity> activities = await pmaServices.LoadActivities(token, id);

            this.lpkActivities.ItemsSource = activities.ToArray();
        }
        
        private void SetProgressIndicator(bool value)
        {
            SystemTray.ProgressIndicator.IsIndeterminate = value;
            SystemTray.ProgressIndicator.IsVisible = value;
        }

        
        private void BtnSave_Click(object sender, EventArgs e)
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
            pmaServices.CreateTheOneAppointment(
                token, 
                day, 
                tpkStartHour.ValueString, 
                tpkEndHour.ValueString,
                tpkRest.ValueString, 
                effortInMinutes, 
                pmaActivity.Id);
        }

        private void BtnList_Click(object sender, EventArgs e)
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
        private void CalculateEffortTip()
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

        private void TpkStartHour_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            CalculateEffortTip();
        }

        private void TpkEndHour_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            CalculateEffortTip();
        }

        private void TpkRest_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            CalculateEffortTip();
        }

        private void BtnConfig_Click(object sender, EventArgs e)
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

        private void BtnListAdv_Click(object sender, EventArgs e)
        {
            GoToListAppointmentsPage();
        }

        private void BtnAddAdv_Click(object sender, EventArgs e)
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

        private async void BtnSaveAdv_Click(object sender, EventArgs e)
        {

            //criar CreateDayAppointment e salvar
            //criar CreateDayAppointment para cada atividade da lista


            //TODO validar se tem atividades adicionadas
            PmaServices pmaServices = new PmaServices();
            string response = await pmaServices.CreateDayAppointment(TokenUtil.GetToken(), dpkDateAdv.FormatToDiaMesAno(), tpkStartHourAdv.ValueString, tpkEndHourAdv.ValueString, tpkRestAdv.ValueString);
            //TODO tratar response

            //foreach (var atividade in ListActivitiesAdvanced)
            //{
                //response = await pmaServices.CreateAppointment(TokenUtil.GetToken(), dpkDateAdv.FormatToDiaMesAno(), atividade.id, atividade.Effort, atividade.Descricao);
                //TODO tratar response
            //}

            //foreach (object o in lstActivitiesAdv.Items)
            //{
            //    if(o is PmaActivity) 
            //    {
            //    }
            //}
        }

        private void BtnConfigAdv_Click(object sender, EventArgs e)
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

        private void LlsActivityAppointment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {

        }

    }

}