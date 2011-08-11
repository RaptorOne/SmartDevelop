using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;
using Archimedes.Patterns;
using System.IO;
using Archimedes.Patterns.Serializing;

namespace SmartDevelop.Model
{
    /// <summary>
    /// Represents the IDE, which means the top level object
    /// Singleton
    /// </summary>
    public class IDE : Singleton<IDE>
    {
        SmartSolution _currentSolution;
        IDESettings _settings;
        string _settingsFilePath = Path.Combine(AppSettingsFolder, "settingsIDE.xml");
        readonly RecentList _recent;

        public event EventHandler<EventArgs<SmartSolution>> CurrentSolutionChanged;
        public event EventHandler<EventArgs<string>> RequestHandleFileOpen;

        /// <summary>
        /// Avoid instancing by throwing ide error ;)
        /// </summary>
        [Obsolete("Singleton!", true)]
        public IDE() {
            if(File.Exists(_settingsFilePath)){
                _settings = SerializerHelper.DeserializeObjectFromFile<IDESettings>(_settingsFilePath);
                _settings.SettingsSerialisationPath = _settingsFilePath;
            }else{
                // load default settings
                _settings = new IDESettings(_settingsFilePath);
                _settings.Save();
            }
            _recent = new RecentList(_settings.Recent);
        }

        static string appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SmartDevelop");

        /// <summary>
        /// Gets the Applications Settings/Data Folder
        /// </summary>
        public static string AppSettingsFolder {
            get {
                return appData;
            }
        }

        public RecentList Recent {
            get { return _recent; }
        }

        /// <summary>
        /// Gets/Sets the current Solution
        /// </summary>
        public SmartSolution CurrentSolution {
            get { return _currentSolution; }
            set {

                if(_currentSolution != null)
                    _currentSolution.ProjectAdded -= OnProjectAdded;

                _currentSolution = value;

                if(_currentSolution != null)
                    _currentSolution.ProjectAdded += OnProjectAdded;

                OnCurrentSolutionChanged(value);
            }
        }

        protected void OnCurrentSolutionChanged(SmartSolution newSolution) {
            if(CurrentSolutionChanged != null)
                CurrentSolutionChanged(this, new EventArgs<SmartSolution>(newSolution));

            if(newSolution != null) {
                foreach(var p in newSolution.GetProjects())
                    UpdateRecentList(p);
            }
        }


        public void OnProjectAdded(object sender, ProjectEventArgs e) {
            UpdateRecentList(e.Project);
        }

        void UpdateRecentList(SmartCodeProject project) {
            _recent.AddLatest(project.FilePath);
            _settings.Recent = _recent.GetRecents();
            _settings.Save();
        }

        /// <summary>
        /// Get the current solution
        /// </summary>
        /// <returns></returns>
        public static SmartSolution GetSolution() {
               return IDE.Instance.CurrentSolution;
        }

        public void OpenFile(string filePath) {
            OnRequestHandleFileOpen(filePath);
        }

        protected virtual void OnRequestHandleFileOpen(string filePath) {
            if(RequestHandleFileOpen != null)
                RequestHandleFileOpen(this, new EventArgs<string>(filePath));
        }
    }
}
