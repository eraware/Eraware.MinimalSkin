using System;

namespace Settings
{
    public record ThemeSettings
    {
        private string skinPath;
        private string containersPath;

        /// <summary>
        /// Gets or sets the package information;
        /// </summary>
        public PackageInfo Package { get; set; } = new PackageInfo();

        /// <summary>
        /// Gets or sets the package owner inforamtion.
        /// </summary>
        public OwnerInfo Owner { get; set; } = new OwnerInfo();

        /// <summary>
        /// Gets or sets the path to deploy the skin.
        /// </summary>
        public string SkinPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.skinPath))
                {
                    this.skinPath = $"Portals\\_default\\Skins\\{this.Package.Name}\\";
                }
                return this.skinPath;
            }

            set
            {
                this.skinPath = value;
            }
        }

        /// <summary>
        /// Gets or sets the path to deploy the containers.
        /// </summary>
        public string ContainersPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.containersPath))
                {
                    this.containersPath = $"Portals\\_default\\Containers\\{this.Package.Name}\\";
                }
                return this.containersPath;
            }

            set
            {
                this.containersPath = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that determines how much of bootstrap to include.
        /// </summary>
        public UseBootstrap UseBootstrap {get;set;} = UseBootstrap.No;

        /// <summary>
        /// Gets or sets a value indicating whether to use FontAwesome.
        /// </summary>
        public bool UseFontAwesome {get;set;} = false;

        /// <summary>
        /// Gets or sets the url of the local test site.
        /// </summary>
        public string TestSiteUrl {get;set;} = "http://dnn.localtest.me";
        public Version Version { get; set; } = new Version(0, 1, 0);
    }
}