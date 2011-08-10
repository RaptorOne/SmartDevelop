using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using Archimedes.Patterns.Serializing;
using Archimedes.Patterns.Services;
using Archimedes.Services.WPF.WindowViewModelMapping;
using Archimedes.Services.WPF.WorkBenchServices;
using Archimedes.Services.WPF.WorkBenchServices.MessageBox;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using SmartDevelop.AHK.AHKv1.CodeCompletion;
using SmartDevelop.AHK.AHKv1.DOM;
using SmartDevelop.AHK.AHKv1.DOM.Types;
using SmartDevelop.AHK.AHKv1.Projecting;
using SmartDevelop.AHK.AHKv1.Projecting.Items;
using SmartDevelop.AHK.AHKv1.Projecting.ProjectTemplates;
using SmartDevelop.AHK.AHKv1.Tokenizing;
using SmartDevelop.AHK.View;
using SmartDevelop.AHK.ViewModel;
using SmartDevelop.Model.CodeLanguages;
using SmartDevelop.Model.CodeLanguages.Extensions;
using SmartDevelop.Model.DOM;
using SmartDevelop.Model.Highlighning;
using SmartDevelop.Model.Projecting;
using SmartDevelop.Model.Tokening;
using SmartDevelop.Model.Tokenizing;
using SmartDevelop.AHK.AHKv1.Folding;

namespace SmartDevelop.AHK.AHKv1
{
    /// <summary>
    /// CodeLanguage Implementation for AHK v 1.1
    /// </summary>
    public class CodeLanguageAHKv1 : CodeLanguage
    {
        const string PROJECTEXTENSION = ".AHKproj";

        #region Fields

        AHKSettings _settings;
        CodeLanguageSettingsViewModel _settingsVM;

        static string AHKSettingsFolder = AppSettingsFolder;
        string _settingsFilePath = Path.Combine(AHKSettingsFolder, "ahksettings.xml");
        IWorkBenchService _workbenchservice = ServiceLocator.Instance.Resolve<IWorkBenchService>();

        readonly ProjectTemplate[] _templates;
        ProjectTemplate _emptyTemplate;
        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new CodeLanguage Implementation for AHK v 1.1
        /// </summary>
        public CodeLanguageAHKv1() 
            : base("ahk-v1.1")
        {
            Name = "AHK v 1.1";


            SELFREF_CAN_BE_OMITTED = false;
            SUPPORTS_STARTUP_CODEDOCUMENT = true;

            if(File.Exists(_settingsFilePath)){
                _settings = SerializerHelper.DeserializeObjectFromFile<AHKSettings>(_settingsFilePath);
                _settings.SettingsSerialisationPath = _settingsFilePath;
            }else{
                // load default settings
                _settings = new AHKSettings(_settingsFilePath);
                _settings.Save();
            }

            var viewmodelMapping = ServiceLocator.Instance.Resolve<IWindowViewModelMappings>();

            viewmodelMapping.RegisterMapping(typeof(CodeLanguageSettingsViewModel), typeof(CodeLanguageSettingsView));


            #region Define Language Syntax
            // todo
            // those data is actually thougt to be read out of confic files
            //

            this.LanguageKeywords.AddRange(new CodeKeyWord[]
                { 
                    new CodeKeyWord("if"), new CodeKeyWord("else"),
                    new CodeKeyWord("class"), new CodeKeyWord("var"), new CodeKeyWord("new"), new CodeKeyWord("this"),new CodeKeyWord("base"), new CodeKeyWord("extends"),
                    new CodeKeyWord("return"), new CodeKeyWord("break"), new CodeKeyWord("continue"),
                    new CodeKeyWord("global"), new CodeKeyWord("static"), new CodeKeyWord("local"), new CodeKeyWord("byref"),
                     new CodeKeyWord("GoTo"), new CodeKeyWord("GoSub"),
                    new CodeKeyWord("loop"), new CodeKeyWord("for"), new CodeKeyWord("while"), new CodeKeyWord("in")
                });

            this.LanguageDirectives.AddRange(CodeLanguageAHKBuildinMethods.GetDirectives());

            var buildins = CodeLanguageAHKBuildinMethods.ReadMembers();
            BuildInMembers.AddRange(buildins);

            #endregion

            #region Load Syntax Definition of AHK

            IHighlightingDefinition customHighlighting;

            Brush b = new SolidColorBrush(Colors.GreenYellow);
            var greenYellowBrush = new HighlightingBrushStaticColor(b);
            var orangeBrush = new HighlightingBrushStaticColor(new SolidColorBrush(Colors.Orange));


            var sr = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Syntax\\Autohotkey.xshd"));
            using(var reader = new XmlTextReader(sr)) {
                customHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                    HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }


            var commandColor = new HighlightingColor()
                {
                    Foreground = orangeBrush,
                    FontWeight = FontWeights.Bold
                };
            var directiveColor = new HighlightingColor()
                {
                    Foreground = greenYellowBrush
                };


            string regexstr = "";
            foreach(var m in buildins) {
                var command = m as CodeMemberMethodExAHK;
                if(command != null && command.IsTraditionalCommand && !command.IsFlowCommand) {
                    //regexstr += command.Name.ToLowerInvariant() + "|";
                    // Add custom but static highligning rule
                    customHighlighting.MainRuleSet.Rules.Add(new HighlightingRule()
                    {
                        Color = commandColor,
                        Regex = GetRegexForCommand(command.Name.ToLowerInvariant())
                    });
                } 
            }


            HighlightingManager.Instance.RegisterHighlighting("ahk-v1.1", new string[] { ".ahk" }, customHighlighting);

            #endregion

            _emptyTemplate = new ProjectTemplateEmpty(this);
            _templates = new ProjectTemplate[] { _emptyTemplate, new ProjectTemplateDemo(this) };

        }
        
        #endregion

        #region Helper Methods

        Regex GetRegexForCommand(string name) {
            var preregex = @"^[\s]*\b";
            var sufregex = @"(?=[\s|,|$][^=])";
            var regex = preregex + name + sufregex;
            var r = new Regex(regex, RegexOptions.IgnoreCase | RegexOptions.Multiline );
            return r;
        }

        Regex GetRegexForDirective(string name) {
            var preregex = "^[\\s]*\\b";
            var sufregex = "\\b[\\s|,]+";
            var r = new Regex(preregex + name + sufregex, RegexOptions.IgnoreCase | RegexOptions.ECMAScript);
            return r;
        }

        #endregion

        #region Public Properties

        public AHKSettings Settings {
            get { return _settings; }
        }

        static readonly string[] _extensions = { ".ahk" };

        public override string[] Extensions {
            get {
                return _extensions;
            }
        }

        public override StringComparison NameComparisation {
            get { return StringComparison.InvariantCultureIgnoreCase; }
        }

        public override string ProjectExtension {
            get { return PROJECTEXTENSION; }
        }

        #endregion
  
        #region Public Methods

        public override Tokenizer CreateTokenizer(ProjectItemCodeDocument codeitem, ITextSource source) {
            return new SimpleTokinizerIA(codeitem, source);
        }

        public override AbstractFoldingStrategy CreateFoldingStrategy( DocumentCodeSegmentService segmentService) {
            return new FoldingStrategyAHKv1(segmentService);
        }



        public override IEnumerable<EditorDocumentExtension> CreateExtensionsForCodeDocument(TextEditor texteditor, ProjectItemCodeDocument projectitem) {
            var extes = new List<EditorDocumentExtension>();
            extes.Add(new CompletionDataProviderAHK(texteditor, projectitem));
            return extes;
        }


        public override CodeDocumentDOMService CreateDOMService(ProjectItemCodeDocument document) {
            return new CodeDOMDocumentServiceAHK(document);
        }

        public override ASTManager CreateASTManager(SmartCodeProject project) {
            return new ASTManagerAHK(project);
        }

        static readonly NewProjectItem[] _avaiableItems = { new NewProjectItemAHK(), new NewProjectItemAHKClass()};

        public override IEnumerable<NewProjectItem> GetAvaiableItemsForNew(ProjectItem contextItem) {
            return _avaiableItems;
        }


        public override void ShowLanguageSettings() {
            if(_settingsVM == null) {
                _settingsVM = new CodeLanguageSettingsViewModel(_settings)
                {
                    DisplayName = "Settings of " + this.LanguageID
                };
            }
            _workbenchservice.ShowDialog(_settingsVM, SizeToContent.WidthAndHeight);
        }

        public override void GetHelpFor(ProjectItemCodeDocument document, Model.CodeContexts.CodeContext ctx) {

            if(File.Exists(_settings.HelpFilePath)) {
                Process p = new Process();
                p.StartInfo.FileName = _settings.HelpFilePath; // todo: run hh.exe and open correct theme if avaiable
                p.Start();
            } else
                _workbenchservice.MessageBox(string.Format("Missing Helpfile.\n{0}", _settings.HelpFilePath), "Help Error", 
                    MessageBoxType.Error);
        }

        #endregion

        public override IEnumerable<ProjectTemplate> GetProjectTemplates() {
            return _templates;
        }

        public override SmartCodeProject Create(string displayname, string name, string location) {
            var p = new SmartCodeProjectAHK(name, location, this);
            p.DisplayName = displayname;
            return p; //_emptyTemplate.Create(displayname, name, location);
        }


    }
}
