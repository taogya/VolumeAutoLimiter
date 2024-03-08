using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace VolumeAutoLimiter.Models
{

    [Serializable]
    [XmlRoot("Settings")]
    public class Settings
    {
        public static string DefaultPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.xml");

        /// <summary>
        /// 設定値が存在するか
        /// </summary>
        public static bool Exists() => Exists(DefaultPath);

        /// <summary>
        /// 設定値が存在するか
        /// </summary>
        public static bool Exists(string path) => File.Exists(path);

        /// <summary>
        /// 設定値を読み込む
        /// </summary>
        public static Settings Load() => Load(DefaultPath);

        /// <summary>
        /// 設定値を読み込む
        /// </summary>
        public static Settings Load(string path)
        {
            var settings = new Settings();
            // ファイルが存在する場合は読み込む
            if (File.Exists(path))
            {
                var reader = new XmlSerializer(typeof(Settings));
                using var file = new StreamReader(path);
                settings = (Settings)reader.Deserialize(file);
            }
            return settings;
        }

        /// <summary>
        /// 設定値を保存する
        /// </summary>
        /// <param name="settings"></param>
        public static void Save(Settings settings) => Save(settings, DefaultPath);

        /// <summary>
        /// 設定値を保存する
        /// </summary>
        public static void Save(Settings settings, string path)
        {
            var serializer = new XmlSerializer(typeof(Settings));
            using var writer = new StreamWriter(path);
            serializer.Serialize(writer, settings);
        }

        /// <summary>
        /// 設定値を保存する
        /// </summary>
        public void Save() => Save(this);

        /// <summary>
        /// 設定値を保存する
        /// </summary>
        public void Save(string path) => Save(this, path);

        public Settings() { }

        /// <summary>
        /// 音量
        /// </summary>
        [XmlElement("Volume")]
        public int Volume { get; set; }
    }
}
