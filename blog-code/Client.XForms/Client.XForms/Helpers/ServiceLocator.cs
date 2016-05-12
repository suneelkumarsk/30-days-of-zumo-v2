using System;
using System.Collections.Generic;

namespace Client.XForms.Helpers
{
    public sealed class ServiceLocator
    {
        static readonly Lazy<ServiceLocator> instance = new Lazy<ServiceLocator>(() => new ServiceLocator());
        readonly Dictionary<Type, Lazy<object>> registeredServices = new Dictionary<Type, Lazy<object>>();

        public static ServiceLocator Instance => instance.Value;

        /// <summary>
        /// Add a new service to the service locator
        /// </summary>
        /// <typeparam name="TContract">The type of the service</typeparam>
        /// <typeparam name="TService">The implementation of the service</typeparam>
        public void Add<TContract, TService>() where TService : new()
        {
            registeredServices[typeof(TContract)] = new Lazy<object>(() => Activator.CreateInstance(typeof(TService)));
        }

        /// <summary>
        /// Obtain the singleton reference to the service object
        /// </summary>
        /// <typeparam name="T">The type of the service</typeparam>
        /// <returns>The resolved service</returns>
        public T Resolve<T>() where T : class
        {
            Lazy<object> service;

            if (registeredServices.TryGetValue(typeof(T), out service))
            {
                return (T)service.Value;
            }
            return null;
        }
    }
}
