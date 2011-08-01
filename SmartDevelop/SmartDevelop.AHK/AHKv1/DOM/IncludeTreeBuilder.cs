using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;
using Archimedes.Patterns.Data.Tree;
using SmartDevelop.Model.DOM;

namespace SmartDevelop.AHK.AHKv1.DOM
{
    public class IncludeTreeBuilder
    {
        Dictionary<ProjectItemCodeDocument, List<ProjectItemCodeDocument>> _codeDocuments;
        SimpleTree<ProjectItemCodeDocument> _treeRoot;
        


        public IncludeTreeBuilder() {

        }


        /// <summary>
        /// Generates the dependency hirarchy of the include files
        /// </summary>
        /// <param name="document"></param>
        /// <param name="parentNode"></param>
        /// <returns></returns>
        /// <exception cref="DependencyTreeException" />
        public List<ProjectItemCodeDocument> BuildHirarchy(Dictionary<ProjectItemCodeDocument, List<ProjectItemCodeDocument>> data, ProjectItemCodeDocument startDoc) {
            // init globals
            _codeDocuments = data;
            _treeRoot = new SimpleTree<ProjectItemCodeDocument>() { Value = startDoc };

            return GenerateDependcyHirarchy(startDoc, _treeRoot);
        }


        List<ProjectItemCodeDocument> GenerateDependcyHirarchy(ProjectItemCodeDocument document, SimpleTreeNode<ProjectItemCodeDocument> parentNode) {
            List<ProjectItemCodeDocument> hirarchy = new List<ProjectItemCodeDocument>();

            SimpleTreeNode<ProjectItemCodeDocument> currentNode;

            // for each include in this document
            foreach(var doc in _codeDocuments[document]) {

                #region Ignore Selfincludes

                if(document.Equals(doc)) {
                    //throw new DependencyTreeException(
                    //    string.Format("Selfincldues won't work my dear!", doc.Name));
                    continue;
                }

                #endregion

                SimpleTreeNode<ProjectItemCodeDocument> parent = parentNode;
                while((parent = parent.Parent) != null) {
                    if(parent.Value.Equals(doc)) {
                        throw new DependencyTreeException(
                            string.Format("Cyclyc reference flow detected in the dependency Tree. --> {0}", doc.Name), doc);
                    }
                }

                currentNode = new SimpleTreeNode<ProjectItemCodeDocument>(doc);
                parentNode.Children.Add(currentNode);

                foreach(var doctoAdd in GenerateDependcyHirarchy(doc, currentNode)) {
                    if(!hirarchy.Contains(doctoAdd))
                        hirarchy.Add(doctoAdd);
                }
                if(!hirarchy.Contains(doc))
                    hirarchy.Add(doc);
            }
            return hirarchy;
        }

    }

}
