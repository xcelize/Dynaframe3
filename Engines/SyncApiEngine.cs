using Dynaframe3.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Dynaframe3
{
    public class SyncApiEngine
    {

        public bool isAuth = false;
        public UserResponse userResponse;
        public Player player;
        public bool JobWork = false;
        public int progress = 0;


        private HttpClient httpClient = new HttpClient();

        public SyncApiEngine()
        {
            Auth();
        }

        public async Task<bool> downloadFiles()
        {
            /*
             * Todo : 
             * - Ajouter dans les settings pour le guid
             * - Utilisation du GUID plutôt que de  l'id
             */
            JobWork = true;
            Guid guid = new Guid("f6a5820c-d6f0-4859-bd83-036f0bacfd80");
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userResponse.Token);
                var result = client.GetAsync("http://localhost:44471/api/Players/1").GetAwaiter().GetResult();
                if (result != null)
                {
                    player = JsonConvert.DeserializeObject<Player>(await result.Content.ReadAsStringAsync());
                    foreach(Fichiers fichier in player.Fichiers)
                    {
                        
                        var fichierStream = await client.GetAsync("http://localhost:44471/api/Fichiers/download/" + fichier.Id);
                        Logger.LogComment("[Info]: Nous téléchargons le fichier " + fichier.Nom);
                        using (var fs = new FileStream(@"C:\Users\Alexis\source\repos\Dynaframe3\web\uploads\test\" + fichier.Nom, FileMode.CreateNew))
                        {
                            FileInfo file = new FileInfo(@"C:\Users\Alexis\source\repos\Dynaframe3\web\uploads\test\" + fichier.Nom);
                            if (!file.Exists)
                            {
                                if (fichierStream.Content.Headers.ContentLength == fichier.Size)
                                    await fichierStream.Content.CopyToAsync(fs);         
                            }
                        }
                        
                    }
                    JobWork = false;
                    return true;
                }
                else
                {
                    JobWork = false;
                    return false;
                }
            }
        }

        private void Auth()
        {
            Credentials credentials = new Credentials { Email = "alexis.petraz@outlook.com", MotDePasse = "alexis" };
            var response = httpClient.PostAsync("http://localhost:44471/api/Utilisateurs/auth", new StringContent(JsonConvert.SerializeObject(credentials), Encoding.UTF8, "application/json")).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                userResponse = JsonConvert.DeserializeObject<UserResponse>(result);
                Logger.LogComment(result);
                isAuth = true;
                addHeaderSecurity();
            }
            else
            {
                Logger.LogComment("[Info]: La requête n'as pas pu aboutir, synchronisation impossible");
                throw new Exception();
            }
        }

        private void addHeaderSecurity()
        {
            if (isAuth && userResponse.estActif)
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", userResponse.Token);
            }
        }
    }
}
