using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;

namespace SmartDevelop.Model.DOM
{
    public class DependencyTreeException : Exception
    {
        public DependencyTreeException(string message, ProjectItemCodeDocument doc)
            : base(message) { Document = doc; }

        public ProjectItemCodeDocument Document { get; protected set; }

    }
}
