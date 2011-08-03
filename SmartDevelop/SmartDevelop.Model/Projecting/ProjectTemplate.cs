using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartDevelop.Model.Projecting
{
    /// <summary>
    /// Represents a Project-Template, which can create a new project
    /// This class is abstract
    /// </summary>
    public abstract class ProjectTemplate
    {
        #region Fields

        string _name;
        string _description;
        protected CodeLanguages.CodeLanguage _language;

        #endregion

        public ProjectTemplate(string name, CodeLanguages.CodeLanguage language) {
            _name = name;
            _language = language;
        }

        #region Properties


        public CodeLanguages.CodeLanguage Language {
            get { return _language; }
        }

        /// <summary>
        /// Template Name
        /// </summary>
        public virtual string Name {
            get { return _name; }
        }

        /// <summary>
        /// Template Description
        /// </summary>
        public virtual string Description {
            get { return _description; }
            set { _description = value; }
        }

        public virtual string Image {
            get { return @"../Images/project-folder.ico"; }
        }

        #endregion

        /// <summary>
        /// Creates the new templated Project
        /// </summary>
        /// <param name="name">Project Name</param>
        /// <param name="location">Project Location</param>
        /// <returns></returns>
        public abstract SmartCodeProject Create(string displayname, string name, string location);
    }
}
