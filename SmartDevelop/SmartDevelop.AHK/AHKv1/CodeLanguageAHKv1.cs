﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using Archimedes.Patterns.Serializing;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using SmartDevelop.AHK.AHKv1.CodeCompletion;
using SmartDevelop.AHK.AHKv1.DOM;
using SmartDevelop.AHK.AHKv1.DOM.Types;
using SmartDevelop.AHK.AHKv1.Tokenizing;
using SmartDevelop.Model.CodeLanguages;
using SmartDevelop.Model.CodeLanguages.Extensions;
using SmartDevelop.Model.DOM;
using SmartDevelop.Model.Highlighning;
using SmartDevelop.Model.Projecting;
using SmartDevelop.Model.Tokening;
using SmartDevelop.Model.Tokenizing;
using SmartDevelop.AHK.AHKv1.Projecting.Items;
using Archimedes.Patterns.Services;
using Archimedes.Services.WPF.WindowViewModelMapping;
using SmartDevelop.AHK.View;
using SmartDevelop.AHK.ViewModel;
using Archimedes.Services.WPF.WorkBenchServices;

namespace SmartDevelop.AHK.AHKv1
{
    public class CodeLanguageAHKv1 : CodeLanguage
    {
        const string PROJECTEXTENSION = ".AHKproj";

        #region Fields

        AHKSettings _settings;
        CodeLanguageSettingsViewModel _settingsVM;

        static string AHKSettingsFolder = AppSettingsFolder;
        string _settingsFilePath = Path.Combine(AHKSettingsFolder, "ahksettings.xml");
        IWorkBenchService _workbenchservice = ServiceLocator.Instance.Resolve<IWorkBenchService>();

        #endregion

        #region Constructor

        public CodeLanguageAHKv1() 
            : base("ahk-v1.1")
        {

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
                    new CodeKeyWord("global"), new CodeKeyWord("static"), new CodeKeyWord("local"),
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

            //// Add custom but static highligning rule
            //customHighlighting.MainRuleSet.Rules.Add(new HighlightingRule()
            //{
            //    Color = commandColor,
            //    Regex = GetRegexForCommand(regexstr.TrimEnd('|'))
            //});



            HighlightingManager.Instance.RegisterHighlighting("ahk-v1.1", new string[] { ".ahk" }, customHighlighting);

            #endregion
        }
        
        #endregion

        public AHKSettings Settings {
            get { return _settings; }
        }

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


        static readonly string[] _extensions = { ".ahk" };

        public override string[] Extensions {
            get {
                return _extensions;
            }
        }

        public override Tokenizer CreateTokenizer(ProjectItemCodeDocument codeitem, ITextSource source) {
            return new SimpleTokinizerIA(codeitem, source);
        }

        public override Model.DOM.CodeDOMService CreateDOMService(Model.Projecting.SmartCodeProject codeProject) {
            return new CodeDOMServiceIA(codeProject);
        }

        public override AbstractFoldingStrategy CreateFoldingStrategy( DocumentCodeSegmentService segmentService) {
            return null; /* new FoldingStrategyAHKv1(segmentService); */
        }

        public override StringComparison NameComparisation {
            get { return StringComparison.InvariantCultureIgnoreCase; }
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

        
        public override string ProjectExtension {
            get { return PROJECTEXTENSION; }
        }
    }
}
