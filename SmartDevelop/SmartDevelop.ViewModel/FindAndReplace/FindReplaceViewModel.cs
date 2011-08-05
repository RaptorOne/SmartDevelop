using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Input;
using System.Collections;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit;
using Archimedes.Patterns.WPF.ViewModels;
using Archimedes.Patterns.WPF.Commands;
using Archimedes.Services.WPF.WorkBenchServices;
using Archimedes.Patterns.Services;
using Archimedes.Services.WPF.WorkBenchServices.MessageBox;
using SmartDevelop.Model.Projecting;
using SmartDevelop.ViewModel.DocumentFiles;
using SmartDevelop.Model;

namespace SmartDevelop.ViewModel.FindAndReplace
{
        public class FindReplaceViewModel : WorkspaceViewModel
        {
            IWorkBenchService _workbenchService = ServiceLocator.Instance.Resolve<IWorkBenchService>();
            IEnumerable<ProjectItemCodeDocument> _alldocuments;
            IEnumerator<ProjectItemCodeDocument> _alldocumentsEnumerator;

            #region Constructor

            public FindReplaceViewModel() {
                ReplacementText = "";
                SearchIn = SearchScope.CurrentDocument;
                ShowSearchIn = true;

                if(IDE.Instance.CurrentSolution != null && IDE.Instance.CurrentSolution.ActiveProject != null){
                    //_alldocuments = IDE.Instance.CurrentSolution
                    _alldocuments = IDE.Instance.CurrentSolution.ActiveProject.CodeDocuments;
                }
            }

            #endregion

            #region Public Properties

            IValueConverter _converter;
            /// <summary>
            /// Objects in the Editors list that do not implement the IEditor interface are converted to IEditor using this converter.
            /// </summary>
            public IValueConverter InterfaceConverter {
                get { return _converter; }
                set { _converter = value; }
            }


            string _textToFind;
            public string TextToFind {
                get { return _textToFind; }
                set { _textToFind = value; }
            }

            public string ReplacementText { get; set; }

            public bool UseWildcards {
                get;
                set;
            }

            public bool SearchUp {
                get;
                set;
            }


            public bool CaseSensitive {
                get;
                set;
            }


            public bool UseRegEx {
                get;
                set;
            }


            public bool WholeWord {
                get;
                set;
            }

            bool _acceptsReturn;
            public bool AcceptsReturn {
                get { return _acceptsReturn; }
                set { _acceptsReturn = value; }
            }
         


            SearchScope _searchIn = SearchScope.CurrentDocument;

            public SearchScope SearchIn {
                get { return _searchIn; }
                set { 
                    _searchIn = value;
                    OnPropertyChanged(() => SearchIn);
                }
            }

            public List<SearchScope> AllScopes {
                get {
                    return new List<SearchScope>() { SearchScope.CurrentDocument, SearchScope.AllDocuments };
                }
            }
         

            /// <summary>
            /// Determines whether to display the Search in combo box
            /// </summary>
            public bool ShowSearchIn {
                get;
                set;
            }


            #endregion


            object _currentEditor;
            public object CurrentEditor {
                get {
                    return _currentEditor;
                }
                set {
                    _currentEditor = value;
                }
            }

            public ProjectItemCodeDocument CurrentDocument {
                get;
                set;
            }

            protected void SetCurrentDocumentAsEditor(ProjectItemCodeDocument doc) {
                var docVM =  CodeFileViewModel.Create(doc);
                CurrentEditor = docVM;
                docVM.ShowCommand.Execute(null);
            }

            #region Helper Methods

            IEditor GetCurrentEditor() {

                if(CurrentDocument != null)
                    SetCurrentDocumentAsEditor(CurrentDocument);
                if(CurrentEditor == null)
                    return null;
                if(CurrentEditor is IEditor)
                    return CurrentEditor as IEditor;
                if(InterfaceConverter == null)
                    return null;

                return InterfaceConverter.Convert(CurrentEditor, typeof(IEditor), null, CultureInfo.CurrentCulture) as IEditor;
            }


            ProjectItemCodeDocument GetNextDocument() {

                if(SearchIn == SearchScope.CurrentDocument)
                    return CurrentDocument;

                if(_alldocumentsEnumerator == null) {
                    if(_alldocuments != null && _alldocuments.Any()) {
                        _alldocumentsEnumerator = _alldocuments.GetEnumerator();
                        _alldocumentsEnumerator.Reset();
                    }
                }

                if(_alldocumentsEnumerator != null) {
                    if(!_alldocumentsEnumerator.MoveNext()) {
                        _alldocumentsEnumerator.Reset();
                        _alldocumentsEnumerator.MoveNext();
                    }
                    CurrentDocument = _alldocumentsEnumerator.Current;
                    return _alldocumentsEnumerator.Current;
                } else
                    return CurrentDocument;
            }

            /// <summary>
            /// Constructs a regular expression according to the currently selected search parameters.
            /// </summary>
            /// <param name="ForceLeftToRight"></param>
            /// <returns>The regular expression.</returns>
            public Regex GetRegEx(bool ForceLeftToRight = false) {
                Regex r;
                RegexOptions o = RegexOptions.None;
                if(SearchUp && !ForceLeftToRight)
                    o = o | RegexOptions.RightToLeft;
                if(!CaseSensitive)
                    o = o | RegexOptions.IgnoreCase;

                if(UseRegEx)
                    r = new Regex(TextToFind, o);
                else {
                    string s = Regex.Escape(TextToFind);
                    if(UseWildcards)
                        s = s.Replace("\\*", ".*").Replace("\\?", ".");
                    if(WholeWord)
                        s = "\\W" + s + "\\W";
                    r = new Regex(s, o);
                }

                return r;
            }

            public void ReplaceAll(bool AskBefore = true) {
                IEditor CE = GetCurrentEditor();
                if(CE == null) return;

                if(!AskBefore || _workbenchService.MessageBox("Do you really want to replace all occurences of '" + TextToFind + "' with '" + ReplacementText + "'?",
                     "Replace all", MessageBoxType.Question, MessageBoxWPFButton.YesNo) == DialogWPFResult.Yes ) {
                    object InitialEditor = CurrentEditor;
                    bool igonreCurrent = false;
                    // loop through all editors, until we are back at the starting editor                
                    do {
                        Regex r = GetRegEx(true);   // force left to right, otherwise indices are screwed up
                        if(!igonreCurrent) {
                            int offset = 0;
                            CE.BeginChange();
                            foreach(Match m in r.Matches(CE.Text)) {
                                CE.Replace(offset + m.Index, m.Length, ReplacementText);
                                offset += ReplacementText.Length - m.Length;
                            }
                            CE.EndChange();
                        }

                        var nextdoc = GetNextDocument();
                        if(r.IsMatch(nextdoc.Text)) {
                            CE = GetCurrentEditor();
                            igonreCurrent = false;
                        } else
                            igonreCurrent = true;
                    } while(CurrentEditor != InitialEditor);
                }
            }

          
            public void FindNext(bool InvertLeftRight = false) {
                IEditor CE = GetCurrentEditor();
                if(CE == null) return;
                Regex r;
                if(InvertLeftRight) {
                    SearchUp = !SearchUp;
                    r = GetRegEx();
                    SearchUp = !SearchUp;
                } else
                    r = GetRegEx();

                Match m = r.Match(CE.Text, r.Options.HasFlag(RegexOptions.RightToLeft) ? CE.SelectionStart : CE.SelectionStart + CE.SelectionLength);
                if(m.Success) {
                    CE.Select(m.Index, m.Length);
                } else {
                    // we have reached the end of the document
                    // start again from the beginning/end,
                    object OldEditor = CurrentEditor;
                    do {
                        var nextdoc = GetNextDocument();

                        //check if this doc contains atleast one match
                        if(r.Options.HasFlag(RegexOptions.RightToLeft))
                            m = r.Match(nextdoc.Text, nextdoc.Text.Length - 1);
                        else
                            m = r.Match(nextdoc.Text, 0);

                        if(m.Success) {
                            if(ShowSearchIn) {
                                CE = GetCurrentEditor();
                                if(CE == null) return;
                            }

                            if(r.Options.HasFlag(RegexOptions.RightToLeft))
                                m = r.Match(CE.Text, CE.Text.Length - 1);
                            else
                                m = r.Match(CE.Text, 0);

                            if(m.Success) {
                                CE.Select(m.Index, m.Length);
                                break;
                            } else {
                                // Failed to find the text
                                //MessageBox.Show("No occurence found.", "Search");
                            }
                        }
                    } while(CurrentEditor != OldEditor);
                }
            }

            public void FindPrevious() {
                FindNext(true);
            }

            public void Replace() {
                IEditor CE = GetCurrentEditor();
                if(CE == null) return;
                // if currently selected text matches -> replace; anyways, find the next match
                Regex r = GetRegEx();
                string s = CE.Text.Substring(CE.SelectionStart, CE.SelectionLength); // CE.SelectedText;
                Match m = r.Match(s);
                if(m.Success && m.Index == 0 && m.Length == s.Length) {
                    CE.Replace(CE.SelectionStart, CE.SelectionLength, ReplacementText);
                }
                FindNext();
            }

            #endregion

            #region Commands

            ICommand _findNextCommand;
            public ICommand FindNextCommand {
                get {
                    if(_findNextCommand == null) {
                        _findNextCommand = new RelayCommand(x => {
                            FindNext();
                        });
                    }
                    return _findNextCommand;
                }
            }

            ICommand _replaceCommand;
            public ICommand ReplaceCommand {
                get {
                    if(_replaceCommand == null) {
                        _replaceCommand = new RelayCommand(x => {
                            Replace();
                        });
                    }
                    return _replaceCommand;
                }
            }

            ICommand _replaceAllCommand;
            public ICommand ReplaceAllCommand {
                get {
                    if(_replaceAllCommand == null) {
                        _replaceAllCommand = new RelayCommand(x => {
                            ReplaceAll();
                        });
                    }
                    return _replaceAllCommand;
                }
            }

            #endregion
        }



    
}
