using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.CodeLanguages;
using ICSharpCode.AvalonEdit.Document;
using SmartDevelop.AHK.AHKv1.Tokenizing;
using SmartDevelop.Model.DOM;
using ICSharpCode.AvalonEdit.Highlighting;
using System.CodeDom;
using SmartDevelop.Model.DOM.Types;
using SmartDevelop.AHK.AHKv1.DOM.Types;
using SmartDevelop.AHK.AHKv1.Folding;
using SmartDevelop.Model.Tokening;
using ICSharpCode.AvalonEdit.Folding;
using System.IO;
using System.Xml;
using System.Windows.Media;
using SmartDevelop.Model.Highlighning;
using System.Text.RegularExpressions;
using SmartDevelop.AHK.AHKv1.CodeCompletion;
using ICSharpCode.AvalonEdit;
using SmartDevelop.Model.Projecting;
using SmartDevelop.Model.CodeLanguages.Extensions;
using SmartDevelop.Model.Tokenizing;
using System.Windows;

namespace SmartDevelop.AHK.AHKv1
{
    public class CodeLanguageAHKv1 : CodeLanguage
    {

        #region Constructor

        public CodeLanguageAHKv1() 
            : base("ahk-v1.1")
        {

            SELFREF_CAN_BE_OMITTED = false;

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

            var buildins = CodeLanguageAHKBuildinMethods.ReadMembers();
            BuildInMembers.AddRange(buildins);

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

        public override Tokenizer CreateTokenizer(ProjectItemCode codeitem, ITextSource source) {
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

        public override IEnumerable<EditorDocumentExtension> CreateExtensionsForCodeDocument(TextEditor texteditor, ProjectItemCode projectitem) {
            var extes = new List<EditorDocumentExtension>();
            extes.Add(new CompletionDataProviderAHK(texteditor, projectitem));
            return extes;
        }

    }
}
