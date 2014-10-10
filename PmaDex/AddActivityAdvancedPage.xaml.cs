using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PmaDex.Util;

namespace PmaDex
{
    public partial class AddActivityAdvancedPage : PhoneApplicationPage
    {

        private bool isInit { get; set; }

        public AddActivityAdvancedPage()
        {
            InitializeComponent();
            isInit = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/MenuPage.xaml", UriKind.Relative));
            });
        }

        private async void lpkProjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (sender as ListPicker).SelectedItem;
            PmaProject pmaProject = (PmaProject) item;
            if (pmaProject == null)
            {
                return;
            }

            string token = TokenUtil.GetTokenFromIsolatedStorage();

            string id = pmaProject.Id;
            PmaServices pmaServices = new PmaServices();
            List<PmaActivity> activities = await pmaServices.loadActivities(token, id);

            this.lpkActivities.ItemsSource = activities.ToArray();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!isInit)
            {
                loadProjects(TokenUtil.GetTokenFromIsolatedStorage());
                isInit = true;
            }
        }

        private async void loadProjects(string token)
        {
            PmaServices pmaServices = new PmaServices();
            List<PmaProject> projectsList = await pmaServices.loadProjects(token);
            this.lpkProjects.ItemsSource = projectsList.ToArray();
        }
    }
}