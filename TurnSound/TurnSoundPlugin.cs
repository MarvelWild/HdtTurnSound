using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Plugins;
using System;
using System.IO;
using System.Media;
using System.Reflection;
using System.Windows.Controls;

namespace HDT.Plugins.TurnSound
{
    public class TurnSoundPlugin : IPlugin
	{
        public string Author => "MW";
        public string ButtonText => "TurnSound";

        public string Description => "Turn sound";

        public MenuItem MenuItem => null;
        public string Name => "TurnSound";

        private SoundPlayer _snd;

        public void OnButtonPress() 
        { 
        }

        private bool _isLoaded = false;

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        private void Log(string text)
        {
            // File.AppendAllText(@"c:\tmp\log.txt", text + Environment.NewLine);
        }

        public void OnLoad() 
        {
            if (!_isLoaded)
            {

                string soundPath = Path.Combine(AssemblyDirectory, "turn.wav");

                Log(soundPath);

                _snd = new SoundPlayer(soundPath);


                GameEvents.OnGameStart.Add(OnGameStart);
                GameEvents.OnTurnStart.Add(OnTurnStart);
                _isLoaded = true;
            }
        }
        public void OnUnload()
        {
            //GameEvents.OnTurnStart.Remove -no method
        }
        public void OnUpdate() { }

        public static readonly Version AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
        public static readonly Version PluginVersion = new Version(AssemblyVersion.Major, AssemblyVersion.Minor, AssemblyVersion.Build);
        public Version Version => PluginVersion;


        private DateTime _lastPlay = DateTime.MinValue;

        private void OnGameStart()
        {
            PlaySound();
        }
        private void OnTurnStart(ActivePlayer player)
        {
            if (ActivePlayer.Player != player) return;

            PlaySound();
        }

        private void PlaySound()
        {
            var now = DateTime.Now;
            var ms_since_play = (now - _lastPlay).TotalMilliseconds;

            // fix multiple fires on single turn start
            if (ms_since_play < 1000)
                return;

            _snd.Play();
            _lastPlay = now;
        }
    }
}
