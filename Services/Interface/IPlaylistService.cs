using Dynaframe3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynaframe3.Services.Interface
{
    public interface IPlaylistService
    {
        public void Next();
        public void Previous();
        public void First();
        public void Last();
        public void Rebuild();
        public PlaylistItem? Current();
    }
}
