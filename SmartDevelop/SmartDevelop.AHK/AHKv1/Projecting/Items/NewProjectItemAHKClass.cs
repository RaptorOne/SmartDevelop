using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;
using System.IO;

namespace SmartDevelop.AHK.AHKv1.Projecting.Items
{
    public class NewProjectItemAHKClass : NewProjectItem
    {
        public NewProjectItemAHKClass() {
            this.Name = "AHK Class";
            this.FileName = "Class1.ahk";
            this.Image = @"/SmartDevelop.AHK;component/Images/AHK_Icon_ClassFile.png";
        }


        public override ProjectItem CreateNewItem(ProjectItem parent) {

            var doc = new ProjectItemCodeDocument(parent.Project.Language, parent);
            doc.Name = this.FileName;

            doc.Document.Text = string.Format(
@"
class {0}
{{




}}", Path.GetFileNameWithoutExtension(this.FileName));
            parent.Add(doc);
            return doc;
        }
    }
}
