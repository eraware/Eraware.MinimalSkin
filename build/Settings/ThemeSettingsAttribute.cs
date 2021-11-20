using Nuke.Common.ValueInjection;
using System.Reflection;

namespace Settings
{
    public class ThemeSettingsAttribute : ValueInjectionAttributeBase
    {
        public override object GetValue(MemberInfo member, object instance) => new ThemeSettings().GetSettings();
    }
}