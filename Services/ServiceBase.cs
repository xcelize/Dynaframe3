using Dynaframe3.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dynaframe3.Services
{
    public abstract class ServiceBase
    {
        protected readonly HttpClient httpClient;
        protected bool isAuth;
        protected UserResponse? userResponse;

        public ServiceBase()
        {
            httpClient = new HttpClient();
            Auth();
        }

        private void Auth()
        {
            Credentials credentials = new Credentials() { Email = AppSettings.Default.Email, MotDePasse=AppSettings.Default.MotDePasse };
            var response = httpClient.PostAsync("http://localhost:44471/api/Utilisateurs/auth", new StringContent(JsonConvert.SerializeObject(credentials), Encoding.UTF8, "application/json")).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                Logger.LogComment("[INFO] L'utilisateur à été trouvé");
                UserResponse userResponse = JsonConvert.DeserializeObject<UserResponse>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                this.userResponse = userResponse;
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", userResponse.Token);
                isAuth = true;
            }
        }
    }
}
