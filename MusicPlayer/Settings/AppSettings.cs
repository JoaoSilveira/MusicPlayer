using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MusicPlayer.Settings
{
    [Serializable]
    public class AppSettings
    {
        private static AppSettings instance = Load() ?? new AppSettings();

        public static AppSettings Instance
        {
            get => instance;
            set
            {
                instance = value;
                Save();
            }
        }

        public static string FileDirectory
        {
            get => instance.LastFileDirectory;
            set
            {
                instance.LastFileDirectory = value;
                Save();
            }
        }

        public static string RootDirectory
        {
            get => instance.LastRootDirectory;
            set
            {
                instance.LastRootDirectory = value;
                Save();
            }
        }

        public static double Volume
        {
            get => instance.LastVolume;
            set => instance.LastVolume = value;
        }

        public static string MediaSource
        {
            get => instance.LastMediaSource;
            set => instance.LastMediaSource = value;
        }

        public string LastMediaSource { get; set; } = null;

        public string LastFileDirectory { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

        public string LastRootDirectory { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

        public double LastVolume { get; set; } = .5;

        public static AppSettings Load()
        {
            try
            {
                using (var stream = File.OpenWrite("app_settings.xml"))
                {
                    return (AppSettings)new XmlSerializer(typeof(AppSettings)).Deserialize(stream);
                }
            }
            catch
            {
                return null;
            }
        }

        public static bool Save()
        {
            try
            {
                using (var stream = File.OpenWrite("app_settings.xml"))
                {
                    new XmlSerializer(typeof(AppSettings)).Serialize(stream, instance);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
