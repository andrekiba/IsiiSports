using System;
using System.IO;
using System.Linq;

namespace IsiiSports.Droid.Utils
{
    internal static class ResourceManagerEx
    {
        internal static int IdFromTitle(string title, Type type)
        {
            var name = Path.GetFileNameWithoutExtension(title);
            var id = GetId(type, name);
            return id; // Resources.System.GetDrawable (Resource.Drawable.dashboard);
        }

        private static int GetId(Type type, string propertyName)
        {
            var props = type.GetFields();
            var prop = props.Select(p => p).FirstOrDefault(p => p.Name == propertyName);
            if (prop != null)
                return (int)prop.GetValue(type);
            return 0;
        }
    }
}