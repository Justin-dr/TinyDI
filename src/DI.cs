using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using TinyDI;

namespace __TINYDI_NAMESPACE__
{
    internal class __TINYDI_CONTAINER__
    {
        private IDictionary<Type, Type> _registry = new Dictionary<Type, Type>();
        private IDictionary<Type, object> _singletons = new Dictionary<Type, object>();
        
        public static __TINYDI_CONTAINER__ Container { get; } = new __TINYDI_CONTAINER__();
        
        public IReadOnlyCollection<Type> RegisteredTypes => new ReadOnlyCollection<Type>(_registry.Keys.ToList());
        
        public __TINYDI_CONTAINER__ AddSingleton<T, I>() where I : T
        {
            Type serviceType = typeof(T);
            Type concreteType = typeof(I);

            if (_registry.ContainsKey(serviceType))
            {
                throw new InvalidOperationException($"A type of {serviceType} has already been registered.");
            }

            if (IsStatic(concreteType))
            {
                throw new ArgumentException($"Concrete type {concreteType.FullName} may not be static.");
            }

            if (concreteType.IsInterface || concreteType.IsAbstract)
            {
                throw new ArgumentException($"{concreteType.FullName} must be a concrete type.");
            }
            
            GetConstructor(concreteType);

            _registry[serviceType] = concreteType;
            return this;
        }

        public __TINYDI_CONTAINER__ AddSingleton<T>() where T : class
        {
            return AddSingleton<T, T>();
        }

        public __TINYDI_CONTAINER__ AddSingleton<T>(T instance) where T : class
        {
            if (instance is null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            
            Type serviceType = typeof(T);
            if (_registry.ContainsKey(serviceType))
            {
                throw new InvalidOperationException($"A type of {serviceType} has already been registered.");
            }

            _registry[serviceType] = instance.GetType();
            _singletons[serviceType] = instance;

            return this;
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T), new List<Type>());
        }

        private object Resolve(Type serviceType, IList<Type> resolutionPath)
        {
            if (_singletons.TryGetValue(serviceType, out object? singleton))
            {
                return singleton;
            }
            
            int index = resolutionPath.IndexOf(serviceType);
            if (index >= 0)
            {
                string cycle = string.Join(
                    " -> ",
                    resolutionPath.Skip(index).Append(serviceType).Select(type => type.Name));
                
                throw new InvalidOperationException($"Circular dependency detected: {cycle}");
            }

            if (!_registry.TryGetValue(serviceType, out Type? concreteType))
            {
                throw new ArgumentException($"Could not find registered implementation for {serviceType.FullName}.");
            }
            
            ConstructorInfo constructor = GetConstructor(concreteType)!;
            
            resolutionPath.Add(serviceType);
            try
            {
                object[] constructorArguments = constructor
                    .GetParameters()
                    .Select(parameter => Resolve(parameter.ParameterType, resolutionPath))
                    .ToArray();

                object instance = constructor.Invoke(constructorArguments);
                _singletons[serviceType] = instance;
                return instance;
            }
            finally
            {
                resolutionPath.RemoveAt(resolutionPath.Count - 1);
            }
            
        }

        private static ConstructorInfo GetConstructor(Type concreteType)
        {
            ConstructorInfo[] constructors = concreteType.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

            ConstructorInfo[] injectConstructors = constructors.Where(c => c.IsDefined(typeof(InjectAttribute))).ToArray();

            if (injectConstructors.Length > 1)
            {
                throw new ArgumentException($"Type {concreteType.FullName} has multiple constructors marked with [Inject].");
            }

            if (injectConstructors.Length == 1)
            {
                return injectConstructors[0];
            }
            
            return constructors.OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();
        }
        
        private static bool IsStatic(Type type)
        {
            return type is { IsClass: true, IsSealed: true, IsAbstract: true };
        }
    }
}