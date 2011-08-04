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

        public event EventHandler SettingsChanged;

        public string InterpreterPath = @"C:\Program Files (x86)\AutoHotkey\AutoHotkey.exe";
        public string LocalLibName = "Lib";
        public string StdLibName = "Lib";
        public string HelpFilePath = @"C:\Program Files (x86)\AutoHotkey\AutoHotkey.chm";

        public AHKSettings() { }
        public AHKSettings(string path) { SettingsSerialisationPath = path; }

        [NonSerialized()]
        internal string SettingsSerialisationPath;
        

        public void Save() {
            SerializerHelper.SerializeObjectToFile(this, SettingsSerialisationPath);
            OnSettingsChanged();
        }

        protected virtual void OnSettingsChanged() {
            if(SettingsChanged != null)
                SettingsChanged(this, EventArgs.Empty);
        }
    }
}
