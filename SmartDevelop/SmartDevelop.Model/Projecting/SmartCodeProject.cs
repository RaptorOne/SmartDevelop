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
using System.Threading.Tasks;

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

        SmartSolution _solution;

        #endregion

        public event EventHandler<EventArgs<ProjectItem>> RequestShowDocument;

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
        /// Gets/Sets the Solution to which this Project belongs
        /// </summary>
        public SmartSolution Solution {
            get { return _solution; }
            internal set { _solution = value; }
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

        #region Public Methods

        /// <summary>
        /// Shows the given document in the editor
        /// </summary>
        /// <param name="documentToShow"></param>
        internal void ShowDocument(ProjectItem documentToShow){
            OnRequestShowDocument(documentToShow);
        }

        #endregion

        #region Event Handlers

        protected override void OnTokenizerUpdated(object sender, EventArgs<ProjectItemCode> codeProjectEventArgs) {
            Task.Factory.StartNew(() => {
                    _domservice.CompileTokenFileAsync(codeProjectEventArgs.Value, null);
                });
            base.OnTokenizerUpdated(sender, codeProjectEventArgs);
        }

        protected virtual void OnRequestShowDocument(ProjectItem documentToShow) {
            if(RequestShowDocument != null)
                RequestShowDocument(this, new EventArgs<ProjectItem>(documentToShow));
        }

        #endregion
    }



}
