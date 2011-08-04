using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Archimedes.Patterns.MVMV.ViewModels.PoolCache;
using Archimedes.Patterns.Services;
using Archimedes.Patterns.WPF.Commands;
using Archimedes.Patterns.WPF.ViewModels;
using Archimedes.Services.WPF.WorkBenchServices;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Folding;
using SmartDevelop.Model.Projecting;
using SmartDevelop.ViewModel.BackgroundRenderer;
using SmartDevelop.ViewModel.TextTransformators;
using SmartDevelop.Model.CodeLanguages.Extensions;
using SmartDevelop.TokenizerBase.IA.Indentation;
using SmartDevelop.Model.DOM.Types;
using Archimedes.Services.WPF.WorkBenchServices.MessageBox;
using SmartDevelop.Model.CodeLanguages;

namespace SmartDevelop.ViewModel.DocumentFiles
{
    /// <summary>
    /// Represents a CodeDocument in the Workspace
    /// </summary>
    public class CodeFileViewModel : WorkspaceViewModel, ICacheable
    {
        #region Fields

        readonly ProjectItemCodeDocument _projectitem;
        readonly TextEditor _texteditor = new TextEditor();
        FoldingManager _foldingManager;
        AbstractFoldingStrategy _foldingStrategy;
        readonly IWorkBenchService _workbenchservice = ServiceLocator.Instance.Resolve<IWorkBenchService>();
        bool _foldingDirty = true;
        IEnumerable<EditorDocumentExtension> _extensions;

        #endregion

        /// <summary>
        /// Raised when this Instance no longer is required in the cache
        /// </summary>
        public event EventHandler CacheExpired;

        #region Constructor

        /// <summary>
        /// Creates a new VM, or if there was already created it returns the cached VM
        /// </summary>
        /// <param name="projectitem"></param>
        /// <returns></returns>
        public static CodeFileViewModel Create(ProjectItemCodeDocument projectitem) {
            var viewModelPoolService = ServiceLocator.Instance.Resolve<IViewModelPoolService>();
            CodeFileViewModel vm;
            vm = viewModelPoolService.Resolve<CodeFileViewModel>(projectitem);
            if(vm == null) {
                vm = new CodeFileViewModel(projectitem);
                viewModelPoolService.Register(projectitem, vm);
            }
            return vm;
        }
        
        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="projectitem"></param>
        CodeFileViewModel(ProjectItemCodeDocument projectitem) {

            if(projectitem == null)
                throw new ArgumentNullException("projectitem");
            _projectitem = projectitem;

            _projectitem.HasUnsavedChangesChanged += OnIsModifiedChanged;

            _projectitem.RequestTextPosition += (s, e) => {
                    _texteditor.TextArea.Caret.Offset = e.Value;
                    _texteditor.TextArea.Caret.BringCaretToView();
                };

            _projectitem.RequestClosing += (s, e) => {
                    this.CloseCommand.Execute(e);
                };


            _projectitem.AST.Updated += (s, e) => {
                _workbenchservice.STADispatcher.Invoke(new Action(() => {
                        _texteditor.TextArea.TextView.Redraw(DispatcherPriority.ContextIdle);
                    }));
                };

            _texteditor.FontFamily = new System.Windows.Media.FontFamily("Consolas");
            _texteditor.FontSize = 15;
            _texteditor.ShowLineNumbers = true;

            _texteditor.Document = _projectitem.Document;
            
            _texteditor.Document.TextChanged += OnDocumentTextChanged;

            _texteditor.SyntaxHighlighting = projectitem.CodeLanguage.GetHighlighter();

            _foldingStrategy = projectitem.CodeLanguage.CreateFoldingStrategy(projectitem.SegmentService);
            if(_foldingStrategy != null) {
                if(_foldingManager == null)
                    _foldingManager = FoldingManager.Install(_texteditor.TextArea);
                _foldingStrategy.UpdateFoldings(_foldingManager, _texteditor.Document);
            }

            // load compeltion item strategy
            var _extensions = projectitem.CodeLanguage.CreateExtensionsForCodeDocument(_texteditor, _projectitem);

            _texteditor.MouseHover += TextEditorMouseHover;
            _texteditor.MouseHoverStopped += TextEditorMouseHoverStopped;


            _texteditor.TextArea.TextEntering += OnTextEntering;

            _texteditor.TextArea.IndentationStrategy = new IAIndentationStrategy();


            _texteditor.TextArea.TextView.BackgroundRenderers.Add(new CurrentLineHighlightRenderer(_texteditor, projectitem));
            _texteditor.TextArea.TextView.BackgroundRenderers.Add(new ErrorBackgroundRenderer(_texteditor, projectitem));


            var contextTransformer = new ContextHighlightTransformator(projectitem);
            _texteditor.TextArea.TextView.LineTransformers.Add(contextTransformer);

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
                        _projectitem.QuickSave();
                    }, x => {
                        return true; /*!string.IsNullOrWhiteSpace(_projectitem.FilePath);*/
                    });
                }
                return _saveCurrentFileCommand;
            }
        }


        #endregion

        #region Find Declaration Command

        ICommand _findDeclarationCommand;
        public ICommand FindDeclarationCommand {
            get {
                if(_findDeclarationCommand == null) {
                    _findDeclarationCommand = new RelayCommand(
                        x => FindDeclaration(),
                        x => CanFindDeclaration);
                }
                return _findDeclarationCommand;
            }
        }

        void FindDeclaration() {
            var segment = _projectitem.SegmentService
                .QueryCodeSegmentAt(_texteditor.TextArea.Caret.Offset);

            if(segment != null) {

                if(segment.CodeDOMObject is CodeMethodReferenceExpressionEx) {
                    var methodRef = segment.CodeDOMObject as CodeMethodReferenceExpressionEx;
                    var methodDeclaration = methodRef.ResolvedMethodMember; //.ResolveMethodDeclarationCache();

                    if(methodDeclaration != null) {
                        var s = methodDeclaration.TryFindSegment();
                        if(s != null)
                            s.BringIntoView();
                    }
                } else if(segment.CodeDOMObject is CodeTypeReferenceEx) {
                    var typeRef = segment.CodeDOMObject as CodeTypeReferenceEx;
                    var classDeclaration = typeRef.ResolvedTypeDeclaration; //.ResolveTypeDeclarationCache();

                    if(classDeclaration != null) {
                        var s = classDeclaration.TryFindSegment();
                        if(s != null)
                            s.BringIntoView();
                    }
                } else if(segment.CodeDOMObject is CodePropertyReferenceExpressionEx) {
                    var propRef = segment.CodeDOMObject as CodePropertyReferenceExpressionEx;
                    var propDeclaration = propRef.ResolvedPropertyMember; // .ResolvePropertyDeclarationCache();

                    if(propDeclaration != null) {
                        var s = propDeclaration.TryFindSegment();
                        if(s != null)
                            s.BringIntoView();
                    }
                } else if(segment.CodeDOMObject is IncludeDirective) {
                    var include = segment.CodeDOMObject as IncludeDirective;
                    var doc = include.ResolvedCodeDocument;
                    if(doc != null)
                        doc.ShowInWorkSpace();
                }

            }

        }

        bool CanFindDeclaration {
            get {
                var segment = _projectitem.SegmentService
                .QueryCodeSegmentAt(_texteditor.TextArea.Caret.Offset);

                if(segment != null &&
                    (segment.CodeDOMObject is CodeMethodReferenceExpressionEx || segment.CodeDOMObject is CodeTypeReferenceEx ||
                    segment.CodeDOMObject is CodePropertyReferenceExpressionEx || segment.CodeDOMObject is IncludeDirective)) {
                        return true;
                }
                return false;

            }
        }

        #endregion

        #endregion

        #region Event Handlers


        protected override void OnHasFocusChanged() {
            if(_projectitem.Project != null && _projectitem.Project.Solution != null) {
                if(this.HasFocus)
                    _projectitem.Project.Solution.DocumentGotFocus(_projectitem);
            }
            base.OnHasFocusChanged();
        }

        public override void OnClosing(System.ComponentModel.CancelEventArgs e) {

            if(e.Cancel)
                return;

            if(_projectitem.HasUnsavedChanges) {
                this.ShowCommand.CanExecute(null);
                this.ShowCommand.Execute(null);
                var res = _workbenchservice.MessageBox("This File has unsaved changes. Do you want to Save them before closing?", "File Closing"
                    , MessageBoxType.Information, MessageBoxWPFButton.YesNoCancel);
                if(res == DialogWPFResult.Yes) {
                    _projectitem.QuickSave();
                } else if ( res == DialogWPFResult.No) {
                    _projectitem.ReloadDocument();
                } else if(res == DialogWPFResult.Abort || res == DialogWPFResult.Cancel) {
                    e.Cancel = true;
                }
            }

            base.OnClosing(e);
        }

        public override void OnClosed() {
            this.Dispose();
        }

        protected virtual void OnCacheExpired() {
            if(CacheExpired != null)
                CacheExpired(this, EventArgs.Empty);
        }


        void OnDocumentTextChanged(object sender, EventArgs e) {
            _foldingDirty = true;
        }

        void OnIsModifiedChanged(object sender, EventArgs e) {
            OnPropertyChanged(() => DisplayName);
        }

        static List<char> whitespacesNewLine = new List<char> { ' ', '\t', '\n', '\r' };
        static List<char> whitespaces = new List<char> { ' ', '\t' };
        static List<char> omitCodeCompletion = new List<char> { '(', ')', '[', ']', '{', '}', ';', ' ', '\t', };

        void OnTextEntering(object sender, TextCompositionEventArgs e) {
            _toolTip.IsOpen = false;
            // Do not set e.Handled=true.
            // We still want to insert the character that was typed.
        }

        protected override void OnIsOnWorkspaceChanged() {
            _projectitem.IsOnWorkspace = this.IsOnWorkspace;
            base.OnIsOnWorkspaceChanged();
        }

        #region ToolTip

        ToolTip _toolTip = new ToolTip();

        void TextEditorMouseHover(object sender, MouseEventArgs e) {
            var pos = _texteditor.GetPositionFromPoint(e.GetPosition(_texteditor));

            if(pos != null) {

                var segment = _projectitem.SegmentService.QueryCodeSegmentAt(_projectitem.Document.GetOffset(pos.Value.Line, pos.Value.Column + 1));
                if(segment == null)
                    return;

                string msg;

                if(segment.HasError) {
                    msg = segment.ErrorContext.Description;
                } else
                 msg = string.Format("[{0}] {1} @ L{2} C{3}, {4}", segment.Token, segment.TokenString, segment.LineNumber, segment.ColumnStart, segment.CodeDOMObject);
                
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

        #endregion

        #region FoldingTimer

        void foldingUpdateTimer_Tick(object sender, EventArgs e) {
            try {
                if(_foldingDirty && !_texteditor.Document.IsInUpdate && _foldingStrategy != null) {
                    _foldingDirty = false;
                    _foldingStrategy.UpdateFoldings(_foldingManager, _texteditor.Document);
                }
            } catch {
                _foldingDirty = true;
            }
        }

        #endregion

        #endregion

        protected override void OnDispose() {
            OnCacheExpired();
            base.OnDispose();
        }

    }
}
