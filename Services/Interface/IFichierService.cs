using Dynaframe3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dynaframe3.Services
{
    public interface IFichierService
    {
        public Task<Player?> getPlayerById(long id);
        public Task downloadFile(Fichiers fichier);
        public Task<Player?> putPlayer(Player player);
        public bool InDownload { get; set; }
        public Task deleteFile(string chemin);

    }
}
