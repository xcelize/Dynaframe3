using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynaframe3.Models
{
    public class PlaylistItem
    {
        private int _position;
        private MediaFile _mediaFile;

        public PlaylistItem(int pPosition, MediaFile mediaFile)
        {
            _position = pPosition;
            _mediaFile = mediaFile;
        }

        public int Position
        {
            get => _position;
            set => _position = value;
        }

        public MediaFile MediaFile
        {
            get => _mediaFile;
            set => _mediaFile = value;
        }
    }
}
