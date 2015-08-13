namespace PmaDex
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using PmaDex.Util;

    public partial class AddActivityAdvancedPage : PhoneApplicationPage
    {
        public AddActivityAdvancedPage()
        {
            this.InitializeComponent();
            this.IsInit = false;
        }

        private bool IsInit { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!this.IsInit)
            {
                this.LoadProjects(TokenUtil.GetToken());
                this.IsInit = true;
            }
        }

        private PmaActivity GetSelectedActivity()
        {
            var item = lpkActivities.SelectedItem;
            PmaActivity pmaActivity = (PmaActivity)item;
            if (pmaActivity != null)
            {
                return pmaActivity;
            }

            return null;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // TODO passar atividade para lista de atividade da tela anterior
            var activity = this.GetSelectedActivity();
            if (activity == null)
            {
                MessageBox.Show("Não foram encontradas atividades");
                return;
            }

            var pmaActivity = new PmaActivity
            {
                Id = activity.Id,
                NomeProjeto = activity.NomeProjeto,
                NomeCliente = activity.NomeCliente,
                Effort = tpkEffortAdv.GetEffortInMinutes(),
                Descricao = txtboxDescription.Text
            };

            Dispatcher.BeginInvoke(() =>
            {
                List<PmaActivity> activities;
                if (PhoneApplicationService.Current.State.ContainsKey("pmaActivities")) 
                {
                    activities = PhoneApplicationService.Current.State["pmaActivities"] as List<PmaActivity>;
                }
                else
                {
                    activities = new List<PmaActivity>();
                }

                activities.Add(pmaActivity);
                PhoneApplicationService.Current.State["pmaActivities"] = activities;

                NavigationService.Navigate(new Uri("/MenuPage.xaml", UriKind.Relative));
            });
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

        private async void LoadProjects(string token)
        {
            PmaServices pmaServices = new PmaServices();
            List<PmaProject> projectsList = await pmaServices.LoadProjects(token);
            this.lpkProjects.ItemsSource = projectsList.ToArray();
        }
    }
}