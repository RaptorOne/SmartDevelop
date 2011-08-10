using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SmartDevelop.ViewModel.CodeCompleting
{
    public class CompletionItemProperty : CompletionItem
    {
        static ImageSource _imgsource;
        

        public CompletionItemProperty(string text, string description) 
            : base(text, description) { }

        public override System.Windows.Media.ImageSource Image {
            get {
                if(_imgsource == null) {
                    BitmapImage logo = new BitmapImage();
                    logo.BeginInit();
                    logo.UriSource = new Uri("pack://application:,,,/SmartDevelop.View;component/Images/MemberProperty.png");
                    logo.EndInit();
                    _imgsource = logo;
                }
                return _imgsource;
            }
        }
    }

    public class CompletionItemPropertyStatic : CompletionItem
    {
        static ImageSource _imgsource;


        public CompletionItemPropertyStatic(string text, string description) 
            : base(text, description) { }

        public override System.Windows.Media.ImageSource Image {
            get {
                if(_imgsource == null) {
                    BitmapImage logo = new BitmapImage();
                    logo.BeginInit();
                    logo.UriSource = new Uri("pack://application:,,,/SmartDevelop.View;component/Images/MemberPropertyStatic.png");
                    logo.EndInit();
                    _imgsource = logo;
                }
                return _imgsource;
            }
        }
    }
    
}
