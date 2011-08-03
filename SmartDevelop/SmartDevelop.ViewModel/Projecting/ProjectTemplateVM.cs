using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;

namespace SmartDevelop.ViewModel.Projecting
{
    public class ProjectTemplateVM
    {
        readonly ProjectTemplate _template;

        public ProjectTemplateVM(ProjectTemplate template) {
            _template = template;
        }


        public ProjectTemplate Template {
            get { return _template; }
        }

        public string LanguageName {
            get { return _template.Language.Name; }
        }

        public string Name {
            get { return _template.Name; }
        }

        public string Description {
            get { return _template.Description; }
        }

        public string Image {
            get { return _template.Image; }
        }
    }
}
