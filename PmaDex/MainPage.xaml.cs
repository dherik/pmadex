﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PmaDex.Resources;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Net.NetworkInformation;

namespace PmaDex
{
    public partial class MainPage : PhoneApplicationPage
    {

        ProgressIndicator progressIndicator = new ProgressIndicator() { IsVisible = true, IsIndeterminate = false, Text = "Autenticando..." };

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            if (!IsolatedStorageSettings.ApplicationSettings.Contains("saveLogin"))
            {
                IsolatedStorageSettings.ApplicationSettings.Add("saveLogin", false);
                IsolatedStorageSettings.ApplicationSettings.Save();
            }

            if (!IsolatedStorageSettings.ApplicationSettings.Contains("user") && !IsolatedStorageSettings.ApplicationSettings.Contains("password"))
            {
                IsolatedStorageSettings.ApplicationSettings.Add("user", "");
                IsolatedStorageSettings.ApplicationSettings.Add("password", "");
                IsolatedStorageSettings.ApplicationSettings.Save();
            }

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            Boolean saveLogin = (bool)settings["saveLogin"];
            if (saveLogin)
            {
                this.user.Text = IsolatedStorageSettings.ApplicationSettings["user"] as string;
                this.pwBox.Password = IsolatedStorageSettings.ApplicationSettings["password"] as string;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
            
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            if (user.Text == "" || pwBox.Password == "") { MessageBox.Show("Preencha o usuário e a senha"); }

            bool isNetwork=NetworkInterface.GetIsNetworkAvailable();
            if (!isNetwork)
            {
                MessageBox.Show("Sem acesso à internet");
                return;
            }

            SystemTray.SetProgressIndicator(this, progressIndicator);
            setProgressIndicator(true);
            PmaServices pmaServices = new PmaServices();
            string response = await pmaServices.login(this.user.Text, this.pwBox.Password);
            getToken(response);
            if (!PmaXmlParser.isError(response))
            {
                saveLoginCredentials(this.user.Text, this.pwBox.Password);
            }

            setProgressIndicator(false);
        }

        private void saveLoginCredentials(string user, string password)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            Boolean saveLogin = (bool)settings["saveLogin"];

            if (saveLogin)
            {
                IsolatedStorageSettings.ApplicationSettings["user"] = user;
                IsolatedStorageSettings.ApplicationSettings["password"] = password;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
            
        }

        private void setProgressIndicator(bool value)
        {
            SystemTray.ProgressIndicator.IsIndeterminate = value;
            SystemTray.ProgressIndicator.IsVisible = value;
        }


        private void getToken(string response)
        {
            XDocument entry = XDocument.Parse(response);
            if (PmaXmlParser.isError(response))
            {
                //error
                string errorMessage = PmaXmlParser.getErrorMessage(response);
                MessageBox.Show("Ocorreu uma falha na requisição: " + errorMessage != null ? errorMessage : "desconhecida");
                return;
            }

            string token = (string)entry.Element("response").Element("content").Element("token");
            if (token != null && !token.Trim().Equals(""))
            {
                Dispatcher.BeginInvoke(() =>
                {
                    NavigationService.Navigate(new Uri("/MenuPage.xaml?token="+token, UriKind.Relative));
                });  
            }
            
        }

    }
}