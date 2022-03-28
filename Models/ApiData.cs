using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynaframe3.Models
{
    public class Credentials
    {
        public string Email { get; set; }
        public string MotDePasse { get; set; }
    }

    public class UserResponse
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string MotDePasse { get; set; }
        public bool estAdmin { get; set; }
        public bool estActif { get; set; }
        public string Token { get; set; }
    }

    public class Fichiers
    {
        public long Id { get; set; }
        public string Nom { get; set; }
        public string Chemin { get; set; }
        public string CheminComplet { get; set; }
        public int Size { get; set; }
    }

    public class Entreprise
    {
        public long Id { get; set; }
        public string Nom { set; get; }
        public string numeroSiret { set; get; }
        public string numeroTelephone { set; get; }
        public string siteWeb { get; set; }
    }

    public class Raspberry
    {
        public long Id { get; set; }
        public string Modele { get; set; }
        public int Ram { get; set; }
    }

    public class Player
    {
        public long Id { get; set; }
        public string Nom { get; set; }
        public string Position { get; set; }
        public Guid Cle { get; set; }
        public bool Update { get; set; }
        public Entreprise Entreprise { get; set; }
        public Raspberry Raspberry { get; set; }
        public ICollection<Fichiers> Fichiers { get; set; }
    }
}
