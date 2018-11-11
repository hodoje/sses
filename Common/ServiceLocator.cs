using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>(20);

        public static void RegisterService<T>(T service)
        {
            if(_services.ContainsKey(typeof(T)))
                throw new ServiceLocatorException("Service of given type is already registered.");

            _services[typeof(T)] = service;
        }

        public static T GetInstance<T>()
        {
            if(!typeof(T).IsInterface)
                throw new ServiceLocatorException("Invalid generic T is not interface type.");

            if(!_services.TryGetValue(typeof(T), out object retVal))
            {
                throw new ServiceLocatorException("Service of given type is not registered.");
            }

            return (T)retVal;
        }

        public static void ResetState()
        {
            _services.Clear();
        }
    }
}
