using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;
using Archimedes.Patterns.Utils;

namespace SmartDevelop.ViewModel.Projecting
{
    public class NewItemViewModel
    {
        NewProjectItem _item;

        public NewItemViewModel(NewProjectItem item) {
            ThrowUtil.ThrowIfNull(item);
            _item = item;
        }

        public string Name {
            get { return _item.Name; }
        }

        public string Image {
            get {
                return _item.Image;
            }
        }

        public ProjectItem Create(ProjectItem parent) {
            return _item.CreateNewItem(parent);
        }

        public override string ToString() {
            return this.Name;
        }


        public string FileName {
            get { return _item.FileName; }
            set { _item.FileName = value; }
        }
    }

}
