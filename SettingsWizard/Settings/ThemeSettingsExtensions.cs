using System.Text.Json;

namespace Settings
{
    public static class ThemeSettingsExtensions
    {
        static string settingsFileName = "theme-settings.json";
        static string settingsFilePath = FindSettingsFile(settingsFileName);

        /// <summary>
        /// Gets the settings from the settings file or creates new default one.
        /// </summary>
        /// <param name="themeSettings">The current theme settings.</param>
        /// <returns>The theme settings.</returns>
        public static ThemeSettings GetSettings(this ThemeSettings themeSettings){
            ThemeSettings settings;
            if (!File.Exists(settingsFilePath))
            {
                settings = new ThemeSettings
                {
                    UseBootstrap = UseBootstrap.No
                };

                string json = JsonSerializer.Serialize(settings);
                File.WriteAllText(settingsFilePath, json);
            }
            else
            {
                string json = File.ReadAllText(settingsFilePath);
                settings = JsonSerializer.Deserialize<ThemeSettings>(json);
            }

            themeSettings = settings;
            return settings;
        }

        /// <summary>
        /// Saves the current settings to a file.
        /// </summary>
        /// <param name="themeSettings">The current theme settings.</param>
        public static void SaveSettings(this ThemeSettings themeSettings)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            File.WriteAllText(settingsFilePath, JsonSerializer.Serialize(themeSettings, options));
        }

        private static string FindSettingsFile(string settingsFileName)
        {
            string? currentDirectory = Directory.GetCurrentDirectory();

            while (currentDirectory != null)
            {
                string filePath = Path.Combine(currentDirectory, settingsFileName);

                if (File.Exists(filePath))
                {
                    return filePath;
                }

                // Move up to the parent directory
                currentDirectory = Directory.GetParent(currentDirectory)?.FullName;
                if (currentDirectory is null)
                {
                    throw new FileNotFoundException("Could not locate the settings file in any parent directory.");
                }
            }

            return string.Empty;
        }
    }
}