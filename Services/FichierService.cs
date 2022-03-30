using Dynaframe3.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dynaframe3.Services
{
    public class FichierService : ServiceBase, IFichierService
    {
        private bool inDownload;

        public FichierService() : base()
        {
            inDownload = false;
        }

        public bool InDownload { get; set; }

        public async Task downloadFile(Fichiers fichier)
        {
            
            if (!isAuth)
            {
                Logger.LogComment("[WARNING]: L'utilisateur ne semble pas connecter");
                throw new Exception("L'utilisateur de ce player n'est pas connecté");
            }
            inDownload = true;
            var fichierResponse = await httpClient.GetAsync("http://localhost:44471/api/Fichiers/download/" + fichier.Id);
            using (var stream = await fichierResponse.Content.ReadAsStreamAsync())
            {
                Logger.LogComment("[Info]: Nous téléchargons le fichier " + fichier.Nom);
                FileInfo fileInfo = new FileInfo(Path.Combine(AppContext.BaseDirectory, @"web\uploads", fichier.Nom));
                using (var fs = fileInfo.OpenWrite())
                { 
                    await stream.CopyToAsync(fs);
                }
            }
            inDownload = false;
        }

        public async Task<Player?> getPlayerById(long id)
        {
            if (!isAuth)
            {
                Logger.LogComment("[WARNING]: L'utilisateur ne semble pas connecter");
                return null;
            }
            var response = await httpClient.GetAsync("http://localhost:44471/api/Players/" + id.ToString());
            if (response != null)
            {
                return JsonConvert.DeserializeObject<Player>(await response.Content.ReadAsStringAsync());
            }
            return null;
            
        }

        public async Task<Player?> putPlayer(Player player)
        {
            if (!isAuth)
            {
                Logger.LogComment("[WARNING]: L'utilisateur ne semble pas connecter");
                return null;
            }
            var playerDTO = new StringContent(JsonConvert.SerializeObject(player), Encoding.UTF8, "application/json");
            var response = await httpClient.PutAsync("http://localhost:44471/api/Players/" + player.Id.ToString(), new StringContent(JsonConvert.SerializeObject(player), Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                Player? playerResp = JsonConvert.DeserializeObject<Player>(await response.Content.ReadAsStringAsync()); 
                if (playerResp != null)
                {
                    return playerResp;
                }
            }
            return null;
        }
    
        public async Task deleteFile(string chemin)
        {
            FileInfo fichier = new FileInfo(chemin);
            if (fichier.Exists)
            {
                await Task.Run(() => { fichier.Delete(); });
            } 
        }
    
    }
}
