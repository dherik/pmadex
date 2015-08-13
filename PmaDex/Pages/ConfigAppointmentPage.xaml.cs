namespace PmaDex.Pages
{
    using Microsoft.Phone.Controls;
    using System;
    using System.Windows;
    using System.IO.IsolatedStorage;

    public partial class ConfigAppointmentPage : PhoneApplicationPage
    {
        public ConfigAppointmentPage()
        {
            this.InitializeComponent();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (this.txtboxDescription.Text == string.Empty)
            {
                MessageBox.Show("Descrição não estar vazia");
            }

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            settings["description"] = this.txtboxDescription;
            settings.Save();
        }
    }
}