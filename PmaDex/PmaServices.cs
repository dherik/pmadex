using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace PmaDex
{
    public class PmaServices
    {

        private const string UrlListarProjetos = "https://dextranet.dextra.com.br/pma/services/listar_projetos";
        private const string UrlLogin = "https://dextranet.dextra.com.br/pma/services/obter_token";
        private const string UrlListarAtividades = "https://dextranet.dextra.com.br/pma/services/listar_atividades";
        private const string UrlCriarApontamentoDiario = "https://dextranet.dextra.com.br/pma/services/criar_apontamento_diario";
        private const string UrlCriarApontamento = "https://dextranet.dextra.com.br/pma/services/criar_apontamento";
        private const string UrlListarApontamentosDiarios = "https://dextranet.dextra.com.br/pma/services/listar_apontamentos_diarios";
        private const string UrlListarApontamentos = "https://dextranet.dextra.com.br/pma/services/listar_apontamentos";
        // https://dextranet.dextra.com.br/pma/registros/destroy?atividade=6827&data=2014-07-29
        // https://dextranet.dextra.com.br/pma/registros/destroy_ad/138159


        public async Task<List<PmaProject>> LoadProjects(string token)
        {
            var values = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("token", token) };
            var httpClient = new HttpClient(new HttpClientHandler());
            HttpResponseMessage response = await httpClient.PostAsync(UrlListarProjetos, new FormUrlEncodedContent(values));
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            List<PmaProject> projectsList = HidrateListPmaProject(responseString);

            return projectsList;
        }

        public async Task<string> Login(string username, string password)
        {
            var values = new List<KeyValuePair<string, string>> { 
                new KeyValuePair<string, string>("username", username), 
                new KeyValuePair<string, string>("password", password) 
            };
            var httpClient = new HttpClient(new HttpClientHandler());
            HttpResponseMessage response = await httpClient.PostAsync(UrlLogin, new FormUrlEncodedContent(values));
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            return responseString;
        }

        public async Task<List<PmaActivity>> LoadActivities(string token, string idProject)
        {
            var values = new List<KeyValuePair<string, string>> { 
                new KeyValuePair<string, string>("token", token), 
                new KeyValuePair<string, string>("projeto", idProject) 
            };
            var httpClient = new HttpClient(new HttpClientHandler());
            HttpResponseMessage response = await httpClient.PostAsync(UrlListarAtividades, new FormUrlEncodedContent(values));
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            List<PmaActivity> activities = HidrateListPmaActivity(responseString);

            return activities;
        }

        private static List<PmaActivity> HidrateListPmaActivity(string response)
        {
            return ExtractActivitiesFromResponse(response);
        }

        private static List<PmaActivity> ExtractActivitiesFromResponse(string response)
        {
            List<PmaActivity> activities = new List<PmaActivity>();
            XDocument entry = XDocument.Parse(response);
            entry.Descendants("atividade").ToList().ForEach(xe =>
            {
                activities.Add(new PmaActivity
                {
                    Id = (string)xe.Element("id"),
                    NomeAtividade = (string)xe.Element("nome"),
                    NomeCliente = (string)xe.Element("cliente"),
                    NomeProjeto = (string)xe.Element("projeto")
                });
            });
            activities.OrderBy(pmaActivity => pmaActivity.NomeAtividade);
            return activities;
        }


        private static List<PmaProject> HidrateListPmaProject(string response)
        {
            XDocument entry = XDocument.Parse(response);
            List<PmaProject> projects = new List<PmaProject>();

            entry.Descendants("projeto").ToList().ForEach(xe =>
            {
                projects.Add(new PmaProject
                {
                    Id = (string)xe.Element("id"),
                    NomeCliente = (string)xe.Element("cliente"),
                    NomeProjeto = (string)xe.Element("nome")
                });
                
            });

            projects.OrderBy(pmaProject => pmaProject.NomeCliente);
            return projects;
        }

        public async void CreateTheOneAppointment(string token, string day, string startHour, string endHour, string restHour, string effort, string idActivity)
        {
            string response = await this.CreateDayAppointment(token, day, startHour, endHour, restHour);
            if (PmaXmlParser.IsError(response))
            {
                MessageBox.Show("Erro ao criar apontamento diário. Motivo: " + PmaXmlParser.GetErrorMessage(response));
                return;
            }

            response = await this.CreateAppointment(token, day, idActivity, effort);
            if (PmaXmlParser.IsError(response))
            {
                MessageBox.Show("Erro ao criar apontamento para atividade. Motivo: " + PmaXmlParser.GetErrorMessage(response));
                return;
            }

            MessageBox.Show("Apontamento criado com sucesso!");
        }

        // Parameters: token, data(yyyy-mm-dd), inicio(HH:MM), intervalo(HH:MM), fim(HH:MM) 
        public async Task<string> CreateDayAppointment(string token, string day, string startHour, string endHour, string restHour)
        {
            var values = new List<KeyValuePair<string, string>> { 
                new KeyValuePair<string, string>("token", token), 
                new KeyValuePair<string, string>("data", day),
                new KeyValuePair<string, string>("inicio", startHour),
                new KeyValuePair<string, string>("intervalo", restHour),
                new KeyValuePair<string, string>("fim", endHour)
            };
            var httpClient = new HttpClient(new HttpClientHandler());
            HttpResponseMessage response = await httpClient.PostAsync(UrlCriarApontamentoDiario, new FormUrlEncodedContent(values));
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;

        }

        // token, data(yyyy-mm-dd), atividadeId, atividadeStatus("working" ou "concluded"), esforco(min), descricao 
        private async Task<string> CreateAppointment(string token, string day, string idActivity, string effort)
        {
            string description = IsolatedStorageSettings.ApplicationSettings["description"] as string;
            return await this.CreateAppointment(token, day, idActivity, effort, description);
        }

        public async Task<string> CreateAppointment(string token, string day, string idActivity, string effort, string description)
        {
            var values = new List<KeyValuePair<string, string>> { 
                new KeyValuePair<string, string>("token", token), 
                new KeyValuePair<string, string>("data", day),
                new KeyValuePair<string, string>("atividadeId", idActivity),
                new KeyValuePair<string, string>("atividadeStatus", "concluded"),
                new KeyValuePair<string, string>("esforco", effort),
                new KeyValuePair<string, string>("descricao", description)
            };
            var httpClient = new HttpClient(new HttpClientHandler());
            HttpResponseMessage response = await httpClient.PostAsync(UrlCriarApontamento, new FormUrlEncodedContent(values));
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }

        public void FindDailyAppointments()
        {

        }

        // token, dataInicial(yyyy-mm-dd), dataFinal(yyyy-mm-dd) 
        public async Task<List<DailyAppointment>> FindDailyAppointments(string token, string startDate, string endDate)
        {
            var values = new List<KeyValuePair<string, string>> { 
                new KeyValuePair<string, string>("token", token), 
                new KeyValuePair<string, string>("dataInicial", startDate),
                new KeyValuePair<string, string>("dataFinal", endDate)            
            };

            var httpClient = new HttpClient(new HttpClientHandler());
            HttpResponseMessage response = await httpClient.PostAsync(UrlListarApontamentosDiarios, new FormUrlEncodedContent(values));
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            List<DailyAppointment> list = this.HidrateListDailyAppointment(responseString);
            return list;

        }

        private List<DailyAppointment> HidrateListDailyAppointment(string response)
        {
            return ExtractDailyAppointmentsFromResponse(response);
        }

        private static List<DailyAppointment> ExtractDailyAppointmentsFromResponse(string response)
        {
            List<DailyAppointment> dailyAppointments = new List<DailyAppointment>();

            XDocument entry = XDocument.Parse(response);
            entry.Descendants("apontamentoDiario").ToList().ForEach(xe =>
            {
                dailyAppointments.Add(new DailyAppointment
                {
                    Data = (string)xe.Element("data"),
                    Inicio = (string)xe.Element("inicio"),
                    Fim = (string)xe.Element("fim"),
                    Intervalo = (string)xe.Element("intervalo")
                });
            });
            return dailyAppointments;
        }

        // token, data(yyyy-mm-dd) 
        public async Task<List<Appointment>> FindAppointments(string token, string data)
        {
            var values = new List<KeyValuePair<string, string>> { 
                new KeyValuePair<string, string>("token", token), 
                new KeyValuePair<string, string>("data", data)
            };

            var httpClient = new HttpClient(new HttpClientHandler());
            HttpResponseMessage response = await httpClient.PostAsync(UrlListarApontamentos, new FormUrlEncodedContent(values));
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            List<Appointment> list = this.HidrateListAppointment(responseString);
            return list;
        }

        private List<Appointment> HidrateListAppointment(string response)
        {
            XDocument entry = XDocument.Parse(response);
            List<Appointment> appointments = new List<Appointment>();
            foreach (XElement xe in entry.Descendants("apontamento"))
            {
                appointments.Add(new Appointment
                {
                    Cliente = (string)xe.Element("cliente"),
                    Projeto = (string)xe.Element("projeto"),
                    Atividade = (string)xe.Element("atividade"),
                    AtividadeStatus = (string)xe.Element("atividadeStatus"),
                    Descricao = (string)xe.Element("descricao"),
                    Esforco = (string)xe.Element("esforco")
                });
            }

            return appointments;
        }

    }
}
