using IniParser;
using IniParser.Model;
using IniParser.Parser;
using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MuseJet.Common.Services
{
    public class ConfigService
    {
        public static string UserFilesDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MuseJet");
        private static string _configFilePath = Path.Combine(UserFilesDir, "config.ini");

        private IniData _iniData;
        public float Volume { get; set; }

        public ConfigService()
        {
            if (!Directory.Exists(UserFilesDir)) Directory.CreateDirectory(UserFilesDir);

            if (!File.Exists(_configFilePath))
            {
                FileIniDataParser iniFile = new();
                _iniData = new();
                _iniData.Sections.AddSection("Config");
                _iniData["Config"].AddKey("volume", 1.0F.ToString());
                iniFile.WriteFile(_configFilePath, _iniData);
                Volume = 1.0F;
            }
            else
            {
                FileIniDataParser iniFile = new();
                _iniData = iniFile.ReadFile(_configFilePath);
                string volume = _iniData["Config"]["volume"];
                Volume = float.Parse(volume);
            }
        }

        public void Save()
        {
            _iniData["Config"]["volume"] = Volume.ToString();
            FileIniDataParser iniFile = new();
            iniFile.WriteFile(_configFilePath, _iniData);
        }
    }
}
