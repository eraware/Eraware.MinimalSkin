using Settings;
using Spectre.Console;
using System.Xml.Serialization;

namespace SettingsWizard
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var settings = new Settings.ThemeSettings();
            settings =  settings.GetSettings();
            settings.Package.Name = AnsiConsole.Ask(
                "What should the [underline]package name[/] be? Please use no spaces or special characters.",
                settings.Package.Name);
            settings.Package.FriendlyName = AnsiConsole.Ask(
                "What should the [underline]friendly name[/] be? This is the name that will be displayed in the UI.",
                settings.Package.FriendlyName);
            settings.Package.Description = AnsiConsole.Ask(
                "What should the [underline]description[/] be? This is the description that will be displayed in the UI.",
                settings.Package.Description);
            settings.Owner.Name = AnsiConsole.Ask(
                "What should the [underline]owner name[/] be? This is the name of the person or organization that owns the package.",
                settings.Owner.Name);
            settings.Owner.Organization = AnsiConsole.Ask(
                "What should the [underline]organization[/] be? This is the organization that owns the package.",
                settings.Owner.Organization);
            settings.Owner.Url = AnsiConsole.Ask(
                "What should the [underline]url[/] be? This is the url of the person or organization that owns the package.",
                settings.Owner.Url);
            settings.Owner.Email = AnsiConsole.Ask(
                "What should the [underline]email[/] be? This is the email of the person or organization that owns the package.",
                settings.Owner.Email);
            settings.ContainersPath = AnsiConsole.Ask(
                "Where would you like to deploy local [underline]containers[/]?",
                settings.ContainersPath);
            settings.SkinPath = AnsiConsole.Ask(
                "Where would you like to deploy local [underline]skins[/]?",
                settings.SkinPath);
            settings.UseBootstrap = AnsiConsole.Prompt(
                new SelectionPrompt<UseBootstrap>()
                .Title("Would you like to use [underline]Bootstrap[/]?")
                .AddChoices(
                    UseBootstrap.No,
                    UseBootstrap.ResponsiveUtilitiesOnly,
                    UseBootstrap.All)
                .UseConverter(choice => choice switch
                {
                    UseBootstrap.No => "No",
                    UseBootstrap.ResponsiveUtilitiesOnly => "Responsive Utilities Only",
                    UseBootstrap.All => "All",
                    _ => "No"
                }));
            settings.UseFontAwesome = AnsiConsole.Confirm(
                "Would you like to use [underline]FontAwesome[/]?",
                settings.UseFontAwesome);
            settings.TestSiteUrl = AnsiConsole.Ask(
                "What is the [underline]test site url[/]?",
                settings.TestSiteUrl);

            settings.SaveSettings();
        }
    }
}