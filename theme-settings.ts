/**
 * Represents the theme package settings
 */
export class ThemeSettings {

    /********************************************************************/
    /*                                                                  */
    /*   DO NOT PUT YOUR SETTINGS HERE, SCROLL DOWN TO THE CONSTRUCTOR   */
    /*                                                                  */
    /********************************************************************/

    packageName:string;
    friendlyName:string;
    description:string;
    ownerName:string;
    ownerOrganization:string;
    ownerUrl:string;
    ownerEmail:string;
    skinpath:string;
    containersPath:string;
    useBootstrap:boolean;
    useFontAwesome:boolean;
    testSiteUrl:string;
    
    constructor() {
        // The package name must be unique and be a valid folder name, so avoid spaces and special charaters 
        // It is recommended to use your company or own name as a prefix to avoid name claches with other developers
        this.packageName = "eraware.ca.themes.minimal";

        // The friendly name is a human readable theme name and shows in the 
        // extension panel, themes panel, theme selection dropdowns, etc.
        this.friendlyName = "Eraware Minimal Theme";

        // The theme description is a longer description for your theme and shows in the extensions panel and it theme selection tooltips
        // Be carefull to keep this file as valid xml if you need to use special characters, see https://www.w3.org/People/mimasa/test/xhtml/entities/entities.xml
        this.description = `
            <![CDATA[
            The Eraware &quot;Minimal&quot; theme is built for speed and simplicity. 
            It has a single minified css and js file. 
            It also avoids loading the Dnn default.css and uses a single variables file to allow easy customization from a single place.
            ]]>
            `;

        // The owner information of the theme (required for all Dnn packages)
        this.ownerName = "Daniel Valadas";
        this.ownerOrganization = "Eraware";
        this.ownerUrl = "https://eraware.ca";
        this.ownerEmail = "info@danielvaladas.com";

        // This section is derived from the previous settings, but feel free to customize if needed
        this.skinpath = `Portals\\_default\\Skins\\${this.packageName}\\`;
        this.containersPath = `Portals\\_default\\Containers\\${this.packageName}\\`;

        // This makes build decisions depending on bootstrap and fontawesome preferences.
        this.useBootstrap = true;
        this.useFontAwesome = false;

        // This saves the last used test site url.
        this.testSiteUrl = "https://dnn.localtest.me";
    }
}