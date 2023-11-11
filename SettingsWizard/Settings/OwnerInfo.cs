namespace Settings
{
    public record OwnerInfo
    {
        /// <summary>
        /// Gets or sets the owner name.
        /// </summary>
        public string Name { get; set; } = "Daniel Valadas";

        /// <summary>
        /// Gets or sets the organization name.
        /// </summary>
        public string Organization { get; set; } = "Eraware";

        /// <summary>
        /// Gets or sets the owner url.
        /// </summary>
        public string Url { get; set; } = "https://eraware.ca";

        /// <summary>
        /// Gets or sets the owner email.
        /// </summary>
        public string Email { get; set; } = "info@danielvaladas.com";
    }
}