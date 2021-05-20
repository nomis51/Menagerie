using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace Menagerie.Services {
    public class AudioService {
        #region Singleton
        private static AudioService _instance = new AudioService();
        public static AudioService Instance {
            get {
                if (_instance == null) {
                    _instance = new AudioService();
                }

                return _instance;
            }
        }
        #endregion

        private SoundPlayer _notif1Player;
        private SoundPlayer _notif2Player;
        private SoundPlayer _clickPlayer;
        private SoundPlayer _knockPlayer;

        private AudioService()
        {
            _notif1Player = new SoundPlayer("Assets/Audio/notif1.wav");
            _notif2Player = new SoundPlayer("Assets/Audio/notif2.wav");
            _clickPlayer = new SoundPlayer("Assets/Audio/click.wav");
            _knockPlayer = new SoundPlayer("Assets/Audio/knocking-on-door.wav");
        }

        public void PlayNotif1() {
            Task.Run(() => _notif1Player.PlaySync());
        }

        public void PlayNotif2() {
            Task.Run(() => _notif2Player.PlaySync());
        }

        public void PlayClick() {
            Task.Run(() => _clickPlayer.PlaySync());
        }

        public void PlayKnock() {
            Task.Run(() => _knockPlayer.PlaySync());
        }
    }
}
