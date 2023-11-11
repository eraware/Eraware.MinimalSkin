using System.Text;

namespace Settings
{
    public record PackageInfo
    {
        /// <summary>
        /// Gets or sets the package name. Avoid using spaces or special characters.
        /// </summary>
        public string Name { get; set; } = "ca.eraware.themes.minimal";

        /// <summary>
        /// Gets or sets the friendly name for the package.
        /// </summary>
        public string FriendlyName { get; set; } = "Eraware minimal theme";

        /// <summary>
        /// Gets or sets the package description.
        /// </summary>
        public string Description { get; set; } = new StringBuilder()
            .AppendLine("The Eraware minimal theme is built for speed and simplicity.")
            .AppendLine("It has a single js and css minimied file.")
            .AppendLine("It also avoids loading the DNN default.css and uses a single variables file to allow theme customization.")
            .ToString();
    }
}