using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SmartDevelop.ViewModel.CodeCompleting
{
    public class CompletionItemKeyword : CompletionItem
    {
        static ImageSource _imgsource;


        public CompletionItemKeyword(string text, string description) 
            : base(text, description) { }

        public override System.Windows.Media.ImageSource Image {
            get {
                if(_imgsource == null) {
                    BitmapImage logo = new BitmapImage();
                    logo.BeginInit();
                    logo.UriSource = new Uri("pack://application:,,,/SmartDevelop.View;component/Images/keyword.png");
                    logo.EndInit();
                    _imgsource = logo;
                }
                return _imgsource;
            }
        }
    }
}
