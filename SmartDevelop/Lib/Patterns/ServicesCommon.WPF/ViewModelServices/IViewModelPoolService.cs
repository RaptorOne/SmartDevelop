using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServicesCommon.WPF.ViewModelServices
{
    /// <summary>
    /// Pool for common used ViewModels
    /// </summary>
    public interface IViewModelPoolService
    {
        /// <summary>
        /// Resolve ViewModel Instance
        /// </summary>
        /// <typeparam name="T">Returns the Project Global Instance of the ViewModel Or Null if Type was not found</typeparam>
        /// <returns></returns>
        T Resolve<T>() where T : class;

        /// <summary>
        /// Register a ViewModel Instance
        /// If there is already an existing VM, its instance is overwritten with the new one.
        /// </summary>
        /// <param name="viewModelInstance"></param>
        void Register(object viewModelInstance);
    }
}
