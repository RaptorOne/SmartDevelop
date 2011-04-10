using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WPFCommon
{
    public interface IDrawingSurface : IInputElement
    {
        UIElementCollection Children { get; }
    }
}
