using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class ProxyPool
    {
        private static readonly Dictionary<Type, object> _proxies = new Dictionary<Type, object>(20);

        public static void RegisterProxy<T>(T proxy)
        {
            if (_proxies.ContainsKey(typeof(T)))
                throw new ServiceLocatorException("Service of given type is already registered.");

            _proxies[typeof(T)] = proxy;
        }

        public static T GetProxy<T>()
        {
            if (!typeof(T).IsInterface)
                throw new ServiceLocatorException("Invalid generic T is not interface type.");

            if (!_proxies.TryGetValue(typeof(T), out object retVal))
            {
                throw new ServiceLocatorException("Service of given type is not registered.");
            }

            return (T)retVal;
        }

        public static void ResetState()
        {
            _proxies.Clear();
        }
    }
}
