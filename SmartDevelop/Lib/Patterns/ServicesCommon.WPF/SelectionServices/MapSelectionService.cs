using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFCommon.SelectionServices;
using System.Windows;

namespace ServicesCommon.WPF.SelectionServices
{
    public class MapSelectionService : SelectionService, IMapSelectionService
    {
        public MapSelectionService() { }

        public MapSelectionService(IInputElement ucontainer)
            : base(ucontainer) { }

    }
}
