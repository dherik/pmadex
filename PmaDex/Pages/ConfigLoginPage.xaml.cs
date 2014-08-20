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

namespace PmaDex.Pages
{
    public partial class ConfigLoginPage : PhoneApplicationPage
    {
        public ConfigLoginPage()
        {
            InitializeComponent();

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            Boolean saveLogin = (bool)settings["saveLogin"];
            this.togSaveLogin.IsChecked = saveLogin;
        }

        private void togSaveLogin_Checked(object sender, RoutedEventArgs e)
        {
            this.togSaveLogin.Content = "Ativado";
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            settings["saveLogin"] = true;
            settings.Save();
        }

        private void togSaveLogin_Unchecked(object sender, RoutedEventArgs e)
        {
            this.togSaveLogin.Content = "Desativado";
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            settings["saveLogin"] = false;

            if (IsolatedStorageSettings.ApplicationSettings.Contains("user") && IsolatedStorageSettings.ApplicationSettings.Contains("password"))
            {
                IsolatedStorageSettings.ApplicationSettings.Remove("user");
                IsolatedStorageSettings.ApplicationSettings.Remove("password");
            }

            settings.Save();
        }
    }
}