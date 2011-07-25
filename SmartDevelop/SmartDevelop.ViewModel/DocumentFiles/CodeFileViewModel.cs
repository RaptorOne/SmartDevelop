using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Archimedes.Patterns.Services;
using Archimedes.Patterns.WPF.ViewModels;
using Archimedes.Services.WPF.WorkBenchServices;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using SmartDevelop.Model.Projecting;
using SmartDevelop.TokenizerBase.IA.Indentation;
using SmartDevelop.ViewModel.Folding;
using Archimedes.Patterns.WPF.Commands;
using Archimedes.Patterns.MVMV.ViewModels.PoolCache;
using SmartDevelop.ViewModel.CodeCompleting;
using System.CodeDom;
using System.Text;
using SmartDevelop.ViewModel.BackgroundRenderer;
using SmartDevelop.ViewModel.TextTransformators;
using SmartDevelop.Model.CodeContexts;
using Archimedes.Patterns.Utils;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using SmartDevelop.Model.DOM.Types;
using SmartDevelop.TokenizerBase;

namespace SmartDevelop.ViewModel.DocumentFiles
{
    public class CodeFileViewModel : WorkspaceViewModel, ICacheable
    {
        #region Fields

        readonly ProjectItemCode _projectitem;
        readonly TextEditor _texteditor = new TextEditor();
        FoldingManager _foldingManager;
        AbstractFoldingStrategy _foldingStrategy;
        readonly IWorkBenchService _workbenchservice = ServiceLocator.Instance.Resolve<IWorkBenchService>();
        CompletionDataProvider _completionDataProvider;
        bool foldingDirty = true;

        #endregion

        /// <summary>
        /// Raised when this Instance no longer is required in the cache
        /// </summary>
        public event EventHandler CacheExpired;

        #region Constructor

        public static CodeFileViewModel Create(ProjectItemCode projectitem) {
            var viewModelPoolService = ServiceLocator.Instance.Resolve<IViewModelPoolService>();
            CodeFileViewModel vm;
            vm = viewModelPoolService.Resolve<CodeFileViewModel>(projectitem);
            if(vm == null) {
                vm = new CodeFileViewModel(projectitem);
                viewModelPoolService.Register(projectitem, vm);
            }

            return vm;
        }


        CodeFileViewModel(ProjectItemCode projectitem) {

            if(projectitem == null)
                throw new ArgumentNullException("projectitem");
            _projectitem = projectitem;

            _projectitem.HasUnsavedChangesChanged += OnIsModifiedChanged;

            _texteditor.FontFamily = new System.Windows.Media.FontFamily("Consolas");
            _texteditor.FontSize = 15;

            _texteditor.Document = _projectitem.Document;
            
            _texteditor.Document.TextChanged += OnDocumentTextChanged;

            _texteditor.SyntaxHighlighting = projectitem.CodeLanguage.GetHighlighter();
            //_foldingStrategy = new IAFoldingStrategy(_projectitem.TokenService);
            if(_foldingStrategy != null) {
                if(_foldingManager == null)
                    _foldingManager = FoldingManager.Install(_texteditor.TextArea);
                _foldingStrategy.UpdateFoldings(_foldingManager, _texteditor.Document);
            }


            _completionDataProvider = new CompletionDataProvider(_texteditor, _projectitem);


            _texteditor.MouseHover += TextEditorMouseHover;
            _texteditor.MouseHoverStopped += TextEditorMouseHoverStopped;

            _texteditor.TextArea.TextEntered += (s, e) => {
                _completionDataProvider.OnTextEntered(s, e);
                };
            _texteditor.TextArea.TextEntering += OnTextEntering;

            _texteditor.TextArea.IndentationStrategy = new IAIndentationStrategy();

            var renderer = new CurrentLineHighlightRenderer(_texteditor, projectitem);
            _texteditor.TextArea.TextView.BackgroundRenderers.Add(renderer);

            //var contextTransformer = new ContextHighlightTransformator(projectitem);
            //_texteditor.TextArea.TextView.LineTransformers.Add(contextTransformer);

            projectitem.RequestTextInvalidation += (s, e) => {
                _texteditor.TextArea.TextView.Redraw();
            };

            DispatcherTimer foldingUpdateTimer = new DispatcherTimer();
            foldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
            foldingUpdateTimer.Tick += foldingUpdateTimer_Tick;
            foldingUpdateTimer.Start();
        }

        #endregion

        #region VM Properties

        public TextEditor Editor {
            get {
                return _texteditor;
            }
        }

        public override string DisplayName {
            get {
                return _projectitem.Name + (_projectitem.HasUnsavedChanges ? "*" : "");
            }
            set {
                _projectitem.Name = value;
            }
        }

        string _contextToolTip;
        bool _contextToolTipIsOpen;

        public bool ContextToolTipIsOpen {
            get { return _contextToolTipIsOpen; }
            set { 
                _contextToolTipIsOpen = value;
                OnPropertyChanged(() => ContextToolTipIsOpen);
            }
        }

        public string ContextToolTip {
            get { return _contextToolTip; }
            set { 
                _contextToolTip = value;
                OnPropertyChanged(() => ContextToolTip); 
            }
        }

        #endregion

        #region Commands

        #region Show Command

        RelayCommand _showCommand;
        public ICommand ShowCommand {
            get {
                if(_showCommand == null) {
                    _showCommand = new RelayCommand(x => 
                        {
                            _workbenchservice.ShowDockedDocument(this);
                        });
                }
                return _showCommand;
            }
        }

        #endregion

        #region Save item Command

        RelayCommand _saveCurrentFileCommand;

        public ICommand SaveCurrentFileCommand {
            get {
                if(_saveCurrentFileCommand == null) {
                    _saveCurrentFileCommand = new RelayCommand(x => {
                        _projectitem.Save(_projectitem.FilePath);
                    }, x => {
                        return !string.IsNullOrWhiteSpace(_projectitem.FilePath);
                    });
                }
                return _saveCurrentFileCommand;
            }
        }


        #endregion

        #endregion

        #region Event Handlers



        void OnDocumentTextChanged(object sender, EventArgs e) {
            foldingDirty = true;
        }

        void OnIsModifiedChanged(object sender, EventArgs e) {
            OnPropertyChanged(() => DisplayName);
        }

        static List<char> whitespacesNewLine = new List<char> { ' ', '\t', '\n', '\r' };
        static List<char> whitespaces = new List<char> { ' ', '\t' };
        static List<char> omitCodeCompletion = new List<char> { '(', ')', '[', ']', '{', '}', ';', ' ', '\t', };

        void OnTextEntering(object sender, TextCompositionEventArgs e) {
            if(e.Text.Length > 0 && _completionDataProvider.CompletionWindow != null) {
                if(!char.IsLetterOrDigit(e.Text[0])) {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    _completionDataProvider.CompletionWindow.CompletionList.RequestInsertion(e);
                }
            }
            _toolTip.IsOpen = false;
            // Do not set e.Handled=true.
            // We still want to insert the character that was typed.
        }

        ToolTip _toolTip = new ToolTip();

        void TextEditorMouseHover(object sender, MouseEventArgs e) {
            var pos = _texteditor.GetPositionFromPoint(e.GetPosition(_texteditor));

            if(pos != null) {

                var segment = _projectitem.SegmentService.QueryCodeSegmentAt(_projectitem.Document.GetOffset(pos.Value.Line, pos.Value.Column + 1));

                string msg;

                if(segment.HasError) {
                    msg = segment.ErrorContext.Message;
                } else
                 msg = string.Format("[{0}] {1} @ Line {2} Col {3} \n {4}", segment.Token, segment.TokenString, segment.LineNumber, segment.ColumnStart, segment.CodeDOMObject);
                
                
                _toolTip.PlacementTarget = _texteditor; // required for property inheritance
                _toolTip.Content = msg;
                _toolTip.Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse;
                _toolTip.IsOpen = true;
                e.Handled = true;
            }
        }

        void TextEditorMouseHoverStopped(object sender, MouseEventArgs e) {
            _toolTip.IsOpen = false;
        }


        void foldingUpdateTimer_Tick(object sender, EventArgs e) {
            if(foldingDirty && !_texteditor.Document.IsInUpdate && _foldingStrategy != null) {
                foldingDirty = false;
                _foldingStrategy.UpdateFoldings(_foldingManager, _texteditor.Document);
            }
        }

        #endregion

    }
}
