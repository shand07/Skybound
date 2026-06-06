using System;
using System.Collections.Generic;
using Skybound.Core.Diagnostics;

namespace Skybound.Core.Services
{
    public static class SkyboundServiceRegistry
    {
        private static readonly Dictionary<Type, object> services = new();

        public static void Register<T>(T service)
        {
            Type type = typeof(T);

            if (services.ContainsKey(type))
            {
                SkyboundDebug.Warning(
                    $"Service {type.Name} already registered. Replacing existing instance."
                );
            }

            services[type] = service;
        }

        public static void Unregister<T>()
        {
            services.Remove(typeof(T));
        }

        public static bool TryGet<T>(out T service)
        {
            if (services.TryGetValue(typeof(T), out object value))
            {
                service = (T)value;
                return true;
            }

            service = default;
            return false;
        }

        public static T Get<T>()
        {
            if (TryGet(out T service))
                return service;

            throw new InvalidOperationException(
                $"Service {typeof(T).Name} is not registered."
            );
        }
    }
}