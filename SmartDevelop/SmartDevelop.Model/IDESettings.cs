using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archimedes.Patterns.Serializing;

namespace SmartDevelop.Model
{
    /// <summary>
    /// GLOBAL IDE Settings & DATA
    /// </summary>
    [Serializable]
    public class IDESettings
    {
        public event EventHandler SettingsChanged;

        public IDESettings() { }
        public IDESettings(string path) { SettingsSerialisationPath = path; }


        public List<string> Recent = new List<string>();


        #region Internal Helpers

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

        #endregion
    }
}
