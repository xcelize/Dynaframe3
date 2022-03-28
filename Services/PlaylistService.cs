using Dynaframe3.Models;
using Dynaframe3.Services.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynaframe3.Services
{
    class PlaylistService : IPlaylistService
    {
        private List<PlaylistItem> _playlist;
        private int _index;

        public PlaylistService()
        {
            _playlist = new List<PlaylistItem>();
            _index = 0;
            Rebuild();
        }

        public PlaylistItem? Current()
        {
            Logger.LogComment("[INFO] le fichier actuel est " + _playlist[_index].MediaFile.Path);
            return _playlist.Count > 0 ? _playlist[_index] : null;
        }

        public void First()
        {
            _index = 0;
            Current();
        }

        public void Last()
        {
            _index = _playlist.Count - 1;
            Current();
        }

        public void Next()
        {
            if (_index + 1 < _playlist.Count)
                _index++;
            else
                First();
        }

        public void Previous()
        {
            if (_index - 1 > 0)
                _index--;
            else
                First();
        }

        public void Rebuild()
        {
            Logger.LogComment("[INFO]: La Playlist se construit");
            foreach (string dir in AppSettings.Default.SearchDirectories)
            {
                if (System.IO.Directory.Exists(dir))
                {
                    int compteur = 1;
                    foreach (string file in System.IO.Directory.GetFiles(dir))
                    {
                        PlayListItemType itemType = PlayListEngineHelper.GetPlayListItemTypeFromPath(file);
                        string type = "";
                        switch (itemType)
                        {
                            case PlayListItemType.Video:
                                type = "Video";
                                break;
                            case PlayListItemType.Image:
                                type = "Image";
                                break;
                            case PlayListItemType.AnimatedGif:
                                type = "Gif";
                                break;
                            case PlayListItemType.Invalid:
                                type = "Invalid";
                                break;
                        }
                        if (type != "Invalid")
                        {
                            MediaFile mediaFile = new MediaFile() { Id = compteur, Path = Path.Combine(dir, file), Type = type };
                            PlaylistItem item = new PlaylistItem(compteur, mediaFile);
                            _playlist.Add(item);
                        }
                    }
                }
            }
            if (AppSettings.Default.Shuffle)
            {
                Shuffle();
            }
        }

        private void Shuffle() 
        {
            var random = new Random();
            _playlist.OrderBy(item => random.Next());
        }

        public List<PlaylistItem> PlaylistItems
        {
            get => _playlist;
            set => _playlist = value;
        }
    }
}
