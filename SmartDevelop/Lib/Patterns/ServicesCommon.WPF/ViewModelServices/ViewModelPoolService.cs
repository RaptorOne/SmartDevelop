using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServicesCommon.WPF.ViewModelServices
{


    /// <summary>
    /// Pool for common used ViewModels
    /// </summary>
    public class ViewModelPoolService : IViewModelPoolService
    {
        Dictionary<Type, object> _viewModels = new Dictionary<Type, object>();

        /// <summary>
        /// Resolve ViewModel Instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Resolve<T>() where T : class 
        { 
            if (_viewModels.ContainsKey(typeof(T))) {
                return _viewModels[typeof(T)] as T;
            } else
                return null;
        }

        /// <summary>
        /// Register a ViewModel Instance
        /// </summary>
        /// <param name="viewModelInstance"></param>
        public void Register(object viewModelInstance) {
            if (_viewModels.ContainsKey(viewModelInstance.GetType())) {
                _viewModels[viewModelInstance.GetType()] = viewModelInstance;
            } else {
                _viewModels.Add(viewModelInstance.GetType(), viewModelInstance);
            }
        }
    }
}
