using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archimedes.Patterns.Serializing;

namespace SmartDevelop.AHK.AHKv1
{
    [Serializable]
    public class AHKSettings
    {
        public string InterpreterPath = @"C:\Program Files (x86)\AutoHotkey\AutoHotkey.exe";

        public AHKSettings() { }
        public AHKSettings(string path) { SettingsSerialisationPath = path; }

        [NonSerialized()]
        internal string SettingsSerialisationPath;

        public void Save() {
            SerializerHelper.SerializeObjectToFile(this, SettingsSerialisationPath);
        }
    }
}
