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
    public partial class ConfigAppointmentPage : PhoneApplicationPage
    {
        public ConfigAppointmentPage()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            if (this.txtboxDescription.Text == "")
            {
                MessageBox.Show("Descrição não estar vazia");
            }

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            settings["description"] = this.txtboxDescription;
            settings.Save();
        }
    }
}