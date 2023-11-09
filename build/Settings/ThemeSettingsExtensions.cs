using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Utilities;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.SerializationTasks;
using static Nuke.Common.NukeBuild;

namespace Settings
{
    public static class ThemeSettingsExtensions
    {
        static string settingsFileName = "theme-settings.json";

        /// <summary>
        /// Gets the settings from the settings file or creates new default one.
        /// </summary>
        /// <param name="themeSettings">The current theme settings.</param>
        /// <returns>The theme settings.</returns>
        public static ThemeSettings GetSettings(this ThemeSettings themeSettings){
            if (!(RootDirectory / settingsFileName).FileExists()){
                var settings = new ThemeSettings();
                settings.UseBootstrap = UseBootstrap.No;
                (RootDirectory / settingsFileName).WriteJson(settings);
                return themeSettings;
            }

            return (RootDirectory / settingsFileName).ReadJson<ThemeSettings>();
        }

        /// <summary>
        /// Saves the current settings to a file.
        /// </summary>
        /// <param name="themeSettings">The current theme settings.</param>
        public static void SaveSettings(this ThemeSettings themeSettings)
        {
            (RootDirectory / settingsFileName).WriteJson(themeSettings);
        }
    }
}