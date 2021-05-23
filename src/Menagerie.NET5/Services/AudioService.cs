using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace Menagerie.Services
{
    public class AudioService
    {
        #region Singleton

        private static AudioService _instance = new AudioService();

        public static AudioService Instance
        {
            get { return _instance ??= new AudioService(); }
        }

        #endregion

        private readonly SoundPlayer _notification1Player;
        private readonly SoundPlayer _notification2Player;
        private readonly SoundPlayer _clickPlayer;
        private readonly SoundPlayer _knockPlayer;

        private AudioService()
        {
            _notification1Player = new SoundPlayer("Assets/Audio/notif1.wav");
            _notification2Player = new SoundPlayer("Assets/Audio/notif2.wav");
            _clickPlayer = new SoundPlayer("Assets/Audio/click.wav");
            _knockPlayer = new SoundPlayer("Assets/Audio/knocking-on-door.wav");
        }

        public void PlayNotification1()
        {
            Task.Run(() => _notification1Player.PlaySync());
        }

        public void PlayNotification2()
        {
            Task.Run(() => _notification2Player.PlaySync());
        }

        public void PlayClick()
        {
            Task.Run(() => _clickPlayer.PlaySync());
        }

        public void PlayKnock()
        {
            Task.Run(() => _knockPlayer.PlaySync());
        }
    }
}