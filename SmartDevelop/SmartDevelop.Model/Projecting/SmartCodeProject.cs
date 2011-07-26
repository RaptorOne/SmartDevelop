using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.DOM;
using System.CodeDom;
using SmartDevelop.Model.CodeLanguages;
using Archimedes.Patterns.Services;
using SmartDevelop.Model.CodeContexts;
using Archimedes.Patterns.Utils;
using Archimedes.Patterns;

namespace SmartDevelop.Model.Projecting
{
    /// <summary>
    /// Represents a smart Project
    /// </summary>
    public class SmartCodeProject : ProjectItem
    {
        #region Fields

        readonly CodeDOMService _domservice;
        readonly CodeLanguage _language;

        #endregion

        #region Constructor

        public SmartCodeProject(string name, CodeLanguage language) 
            : base(null) {

                ThrowUtil.ThrowIfNull(language);

            Name = name;
            _language = language;
            _domservice = language.CreateDOMService(this);
            
        }

        #endregion

        #region Properties

        public CodeDOMService DOMService {
            get { return _domservice; }
        }

        public CodeLanguage Language {
            get { return _language; }
        }


        /// <summary>
        /// Returns itself ;) 
        /// (children delegate their Project Get up to the parent root)
        /// </summary>
        public override SmartCodeProject Project {
            get {
                return this;
            }
        }

        string _name="";
        public override string Name {
            get { return _name; }
            set { _name = value; }
        }

        #endregion

        #region Event Handlers

        protected override void OnTokenizerUpdated(object sender, EventArgs<ProjectItemCode> codeProjectEventArgs) {
            _domservice.CompileTokenFile(codeProjectEventArgs.Value, _domservice.RootType);
            base.OnTokenizerUpdated(sender, codeProjectEventArgs);
        }

        #endregion
    }



}
