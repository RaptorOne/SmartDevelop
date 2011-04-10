using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WPFCommon.SelectionServices
{
    /// <summary>
    /// Marks implementing Class as Selectable
    /// </summary>
    public interface ISelectable
    {
        bool IsSelected { get; set; }

        bool CanSelect { get; }

        void Select();

        void DeSelect();
    }
}
