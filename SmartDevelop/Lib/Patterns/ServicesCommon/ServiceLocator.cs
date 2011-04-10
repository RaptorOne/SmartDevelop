using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ServicesCommon
{
    /// <summary>
    /// Global service locator.
    /// Singleton
    /// </summary>
    public class ServiceLocator : ServiceLocatorInstance
    {
        private readonly static ServiceLocator _instance = new ServiceLocator();

        ServiceLocator() { }
        static ServiceLocator() { }

        public static ServiceLocator Instance {
            get { return _instance; }
        }
    }
}
