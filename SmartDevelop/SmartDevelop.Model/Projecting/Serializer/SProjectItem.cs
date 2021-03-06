﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archimedes.Patterns.Services;
using SmartDevelop.Model.CodeLanguages;
using System.Xml.Serialization;

namespace SmartDevelop.Model.Projecting.Serializer
{

    /// <summary>
    /// Serializable Project Item
    /// </summary>
    [Serializable]
    public abstract class SProjectItem
    {
        public SProjectItem() {}

        public SProjectItem(ProjectItem item) {
            this.Name = item.Name;

            foreach(var c in item.Children)
                this.Children.Add(SProjectItem.Build(c));
        }

        public string Name;
        public List<SProjectItem> Children = new List<SProjectItem>();


        public static SProjectItem Build(ProjectItem item) {

            if(item is SmartCodeProject)
                return new SSmartCodeProject(item as SmartCodeProject);

            if(item is ProjectItemFolder)
                return new SProjectItemFolder(item as ProjectItemFolder);

            if(item is ProjectItemCodeDocument)
                return new SProjectItemCodeDocument(item as ProjectItemCodeDocument);

            throw new NotSupportedException("Don't know how to serialize type: " + item.GetType().Name);
        }


        public abstract ProjectItem CreateObj(ProjectItem parent);
    }


    /// <summary>
    /// Project
    /// </summary>
    [XmlInclude(typeof(SProjectItemFolder))]
    [XmlInclude(typeof(SProjectItemCodeDocument))]
    [Serializable]
    public class SSmartCodeProject : SProjectItem
    {
        string _folderPath;

        public SSmartCodeProject() { }

        public SSmartCodeProject(SmartCodeProject project)
            : base(project) {
                this.CodeLanguageID = project.Language.LanguageID;
                this.DisplayName = project.DisplayName;
        }


        public string CodeLanguageID;
        public string DisplayName;

        public ProjectItem CreateProject(string projectFolder) {
            _folderPath = projectFolder;
            return this.CreateObj(null);
        }

        public override ProjectItem CreateObj(ProjectItem parent) {

            var serviceLang = ServiceLocator.Instance.Resolve<ICodeLanguageService>();
            var language = serviceLang.GetById(this.CodeLanguageID);

            var p = language.Create(this.DisplayName, this.Name, _folderPath);
            p.BeginProjectUpdate();
            {
                foreach(var c in this.Children)
                    p.Add(c.CreateObj(p));

                foreach(var doc in p.CodeDocuments) {
                    doc.ReloadDocument();
                }
            } p.EndProjectUpdate();

            return p;
        }
    }


    /// <summary>
    /// Folder
    /// </summary>
    [Serializable]
    public class SProjectItemFolder : SProjectItem
    {
        public SProjectItemFolder() { }
        public SProjectItemFolder(ProjectItemFolder folder)
            : base(folder) { }

        public override ProjectItem CreateObj(ProjectItem parent) {

            var folder = new ProjectItemFolder(this.Name, parent);

            foreach(var c in this.Children)
                folder.Add(c.CreateObj(folder));
            return folder;
        }
    }

    /// <summary>
    /// Document
    /// </summary>
    [Serializable]
    public class SProjectItemCodeDocument : SProjectItem
    {
        public SProjectItemCodeDocument() { }
        public SProjectItemCodeDocument(ProjectItemCodeDocument doc) 
            : base(doc) {
                this.OverrideFilePath = doc.OverrideFilePath;
                this.CodeLanguageID = doc.CodeLanguage.LanguageID;
                this.IsStartupDocument = doc.IsStartUpDocument;
        }

        public string OverrideFilePath;
        public string CodeLanguageID;
        public bool IsStartupDocument;

        public override ProjectItem CreateObj(ProjectItem parent) {

            var serviceLang = ServiceLocator.Instance.Resolve<ICodeLanguageService>();
            var language = serviceLang.GetById(this.CodeLanguageID);

            var doc = new ProjectItemCodeDocument(this.Name, language, parent)
            {
                OverrideFilePath = this.OverrideFilePath,
                IsStartUpDocument = this.IsStartupDocument
            };

            return doc;
        }
    }

}
