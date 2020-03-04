using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace Planet.MongoDbConsoleAppSample.Conventions {
    //https://stackoverflow.com/questions/39604820/serialize-get-only-properties-on-mongodb
    public class ImmutableAggregationConvention : ConventionBase, IClassMapConvention {
        private readonly BindingFlags _bindingFlags;

        public ImmutableAggregationConvention () : this (BindingFlags.Instance | BindingFlags.Public) { }

        public ImmutableAggregationConvention (BindingFlags bindingFlags) {
            _bindingFlags = bindingFlags | BindingFlags.DeclaredOnly;
        }

        public void Apply (BsonClassMap classMap) {
            if (classMap.ClassType.IsAbstract)
                return;

            var readOnlyProperties = classMap
                .ClassType
                .GetTypeInfo ()
                .GetProperties (_bindingFlags)
                .Where (p => IsReadOnlyProperty (classMap, p))
                .ToList ();

            foreach (var method in classMap.ClassType.GetMethods ()) {
                var matchProperties = GetMatchingProperties (method, readOnlyProperties);
                if (matchProperties.Any ()) {
                    // Map properties
                    foreach (var p in matchProperties)
                        classMap.MapMember (p);
                }
            }
        }

        private static List<PropertyInfo> GetMatchingProperties (MethodInfo method, List<PropertyInfo> properties) {
            var matchProperties = new List<PropertyInfo> ();

            var methodParameters = method.GetParameters ();
            properties.ForEach (property => {
                var matchProperty = ParameterMatchProperty (methodParameters, property);
                if (matchProperty)
                    matchProperties.Add (property);
            });

            return matchProperties;
        }

        private static PropertyInfo[] GetPropertyTypeProperties (PropertyInfo property) {
            var propertyType = property.PropertyType;
            return propertyType.GetGenericArguments ()
                .FirstOrDefault ()
                .GetProperties ();
        }

        private static bool ParameterMatchProperty (ParameterInfo[] parameters, PropertyInfo property) {
            var readonlyTypeProperties = GetPropertyTypeProperties (property);
            if (readonlyTypeProperties.Count () != parameters.Count ())
                return false;

            foreach (var parameter in parameters) {
                var result = readonlyTypeProperties.Any (rot => rot.PropertyType == parameter.ParameterType && string.Equals (rot.Name, parameter.Name, System.StringComparison.InvariantCultureIgnoreCase));
                if (!result)
                    return false;
            }
            return true;
        }

        private static bool IsReadOnlyProperty (BsonClassMap classMap, PropertyInfo propertyInfo) {
            // we can't read 
            if (!propertyInfo.CanRead) return false;

            // we can write (already handled by the default convention...)
            if (propertyInfo.CanWrite) return false;

            // skip indexers
            if (propertyInfo.GetIndexParameters ().Length != 0) return false;

            // skip overridden properties (they are already included by the base class)
            var getMethodInfo = propertyInfo.GetMethod;
            if (getMethodInfo.IsVirtual && getMethodInfo.GetBaseDefinition ().DeclaringType != classMap.ClassType)
                return false;

            return true;
        }
    }
}