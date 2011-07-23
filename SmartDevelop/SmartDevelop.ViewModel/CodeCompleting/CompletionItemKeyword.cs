using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SmartDevelop.Model.CodeLanguages;

namespace SmartDevelop.ViewModel.CodeCompleting
{
    public class CompletionItemKeyword : CompletionItem
    {
        static ImageSource IMAGE_SOURCE;
        CodeKeyWord _codeKeyWord;

        public CompletionItemKeyword(CodeKeyWord keyword) 
            : base(keyword.Name, keyword.Description) {
            _codeKeyWord = keyword;
        }

        public CompletionItemKeyword(string text, string description) 
            : base(text, description) { }

        public override System.Windows.Media.ImageSource Image {
            get {
                if(CompletionItemKeyword.IMAGE_SOURCE == null) {
                    BitmapImage logo = new BitmapImage();
                    logo.BeginInit();
                    logo.UriSource = new Uri("pack://application:,,,/SmartDevelop.View;component/Images/keyword.png");
                    logo.EndInit();
                    IMAGE_SOURCE = logo;
                }
                return CompletionItemKeyword.IMAGE_SOURCE;
            }
        }
    }
}
