using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;

namespace SmartDevelop.AHK.AHKv1.Projecting.Items
{
    public class NewProjectItemAHK : NewProjectItem
    {

        public NewProjectItemAHK() {

            this.Name = "Empty AHK File";
            this.FileName = "NewFile.ahk";
            this.Image = @"/SmartDevelop.AHK;component/Images/AHK_Icon_EmpyFile.png";

        }

        public override ProjectItem CreateNewItem(ProjectItem parent) {
            var doc = new ProjectItemCodeDocument(parent.Project.Language, parent);
            doc.Name = this.FileName;
            doc.Document.Text = string.Format(
@"
#NoEnv
; Empty Template

");
            parent.Add(doc);
            return doc;
        }

    }
}
