using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace IsiiSports.Droid.Utils
{
    public class ReflectedProxy<T> where T : class
    {
        private readonly object target;

        private readonly Dictionary<string, PropertyInfo> cachedPropertyInfo;
        private readonly Dictionary<string, MethodInfo> cachedMethodInfo;

        private readonly IEnumerable<PropertyInfo> targetPropertyInfoList;
        private readonly IEnumerable<MethodInfo> targetMethodInfoList;

        public ReflectedProxy(T target)
        {
            this.target = target;

            cachedPropertyInfo = new Dictionary<string, PropertyInfo>();
            cachedMethodInfo = new Dictionary<string, MethodInfo>();

            var typeInfo = typeof(T).GetTypeInfo();
            targetPropertyInfoList = typeInfo.GetRuntimeProperties();
            targetMethodInfoList = typeInfo.GetRuntimeMethods();
        }

        public void SetPropertyValue(object value, [CallerMemberName] string propertyName = "")
        {
            GetPropertyInfo(propertyName).SetValue(target, value);
        }

        public TPropertyValue GetPropertyValue<TPropertyValue>([CallerMemberName] string propertyName = "")
        {
            return (TPropertyValue)GetPropertyInfo(propertyName).GetValue(target);
        }

        public object Call([CallerMemberName] string methodName = "", object[] parameters = null)
        {

            if (!cachedMethodInfo.ContainsKey(methodName))
            {
                cachedMethodInfo[methodName] = targetMethodInfoList.Single(mi => mi.Name == methodName || mi.Name.Contains("." + methodName));
            }

            return cachedMethodInfo[methodName].Invoke(target, parameters);
        }

        private PropertyInfo GetPropertyInfo(string propertyName)
        {
            if (!cachedPropertyInfo.ContainsKey(propertyName))
            {
                cachedPropertyInfo[propertyName] = targetPropertyInfoList.Single(pi => pi.Name == propertyName || pi.Name.Contains("." + propertyName));
            }

            return cachedPropertyInfo[propertyName];
        }
    }
}