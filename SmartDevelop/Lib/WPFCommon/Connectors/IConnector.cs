using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WPFCommon.Connectors
{
    public interface IConnector
    {
        Point StartPoint { get; set; }
        Point EndPoint { get; set; }
    }
}
