using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.TokenizerBase;
using SmartDevelop.Model.Errors;
using System.IO;


namespace SmartDevelop.ViewModel.Errors
{
    public class ErrorListItemVM
    {
        ErrorItem _errorItem;

        public ErrorListItemVM(ErrorItem errorItem) {
            _errorItem = errorItem;
        }


        public ErrorItem ErrorItem { get { return _errorItem; } }

        public string Icon {
            get {
                return "@../Images/erroricon.png";
            }
        }

        
        public string Description {
            get { 

                return _errorItem.Error.Description;
            }
        }

        public int Line {
            get {
                if(_errorItem.Segment != null)
                    return _errorItem.Segment.LineNumber;
                return 0;
            }
        }

        public int Column {
            get {
                if(_errorItem.Segment != null)
                    return _errorItem.Segment.ColumnStart;
                return 0;
            }
        }

        public string File {
            get { return Path.GetFileName(_errorItem.CodeItem.FilePath); }
        }

        public string Project {
            get { return _errorItem.CodeItem.Project.Name; }
        }

    }
}
