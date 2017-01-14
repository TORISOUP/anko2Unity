using System;
using System.IO;
using System.Xml.Serialization;

namespace anko2unity
{

    /// <summary>
    /// 設定値クラス
    /// </summary>
    [Serializable]
    public class Config
    {
        public int LocationX { get; set; }
        public int LocationY { get; set; }

        public int Port { get; set; }

        public Config()
        {
            this.LocationX = 0;
            this.LocationY = 0;
            this.Port = 17305;
        }

        /// <summary>
        /// 設定値をファイルから読み込みます
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <returns></returns>
        public static Config Load(string path)
        {
            if (!File.Exists(path))
            {
                return new Config();
            }

            Config configData = null;
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    System.Xml.Serialization.XmlSerializer xs = new XmlSerializer(typeof(Config));
                    configData = xs.Deserialize(fs) as Config;
                }
            }
            catch { }

            return configData != null ? configData : new Config();
        }

        /// <summary>
        /// 設定値をファイルへ書き込みます
        /// </summary>
        /// <param name="path">ファイルパス</param>
        public void Save(string path)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer xs = new XmlSerializer(typeof(Config));
                    xs.Serialize(fs, this);
                }
            }
            catch { }
        }

    }

}
