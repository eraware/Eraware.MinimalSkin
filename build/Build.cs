using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Npm;
using Nuke.Common.Utilities.Collections;
using Settings;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.Npm.NpmTasks;
using static Nuke.Common.Utilities.ConsoleUtility;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.StageFiles);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [ThemeSettings]
    readonly ThemeSettings ThemeSettings;

    Target Clean => _ => _
        .Before(Compile)
        .Executes(() =>
        {
            EnsureCleanDirectory(Directories.ArtifactsDirectory);
            EnsureCleanDirectory(Directories.DistDirectory);
        });

    Target Install => _ => _
        .Executes(() => {
            NpmInstall();
        });

    Target Compile => _ => _
        .DependsOn(Install)
        .Executes(() =>
        {
            NpmRun(s => s.SetCommand("build"));
        });

    Target Settings => _ => _
    .Executes(() =>{
        ThemeSettings.Package.Name = PromptForInput(
            "What should the package name be? Please use no spaces or special characters.",
            ThemeSettings.Package.Name
        );
        ThemeSettings.Package.FriendlyName = PromptForInput(
            "What should the package friendly name be? Displayed to the user in the extensions list.",
            ThemeSettings.Package.FriendlyName
        );
        ThemeSettings.Package.Description = PromptForInput(
            "What should be the package description?",
            ThemeSettings.Package.Description
        );
        ThemeSettings.Owner.Name = PromptForInput(
            "What is the name of the package owner?",
            ThemeSettings.Owner.Name
        );
        ThemeSettings.Owner.Organization = PromptForInput(
            "What organization owns this package?",
            ThemeSettings.Owner.Organization
        );
        ThemeSettings.Owner.Url = PromptForInput(
            "What is the url to the package owner website?",
            ThemeSettings.Owner.Url
        );
        ThemeSettings.Owner.Email = PromptForInput(
            "What is the email of the package owner?",
            ThemeSettings.Owner.Email
        );
        ThemeSettings.ContainersPath = PromptForInput(
            "Where would you like to deploy local containers?",
            ThemeSettings.ContainersPath
        );
        ThemeSettings.ContainersPath = PromptForInput(
            "Where would you like to deploy local skins?",
            ThemeSettings.SkinPath
        );
        ThemeSettings.UseBootstrap = PromptForChoice(
            "Would you like your theme to include bootstarp?", new[]{
                (UseBootstrap.No, "No"),
                (UseBootstrap.ResponsiveUtilitiesOnly, "Responsive utilities and grid only (44Kb)"),
                (UseBootstrap.All, "All of bootstrap (221Kb)"),
            }
        );
        ThemeSettings.UseFontAwesome = PromptForChoice(
            "Would you like to include FontAwesome (206Kb)", new[]{
                (false, "No"),
                (true, "Yes"),
            }
        );
        ThemeSettings.SaveSettings();
    });

    Target StageFiles => _ => _
    .DependsOn(Clean)
    .DependsOn(Compile)
    .Executes(() => {
        GlobFiles(RootDirectory / "src" / "containers", "*.ascx")
            .ForEach(file =>
                CopyFileToDirectory(
                    file,
                    Directories.DistDirectory / "containers",
                    FileExistsPolicy.Overwrite,
                    true));
        CopyFileToDirectory(
            RootDirectory / "LICENSE",
            Directories.DistDirectory,
            FileExistsPolicy.Overwrite,
            true);
        CopyFile(
            RootDirectory / "manifest.xml",
            Directories.DistDirectory / "manifest.dnn",
            FileExistsPolicy.Overwrite,
            true);
        CopyFileToDirectory(
            RootDirectory / "releaseNotes.txt",
            Directories.DistDirectory,
            FileExistsPolicy.Overwrite,
            true);
        CopyDirectoryRecursively(
            RootDirectory / "src" / "images",
            Directories.DistDirectory / "skin" / "Images",
            DirectoryExistsPolicy.Merge,
            FileExistsPolicy.Overwrite);
        CopyDirectoryRecursively(
            RootDirectory / "src" / "html",
            Directories.DistDirectory / "skin",
            DirectoryExistsPolicy.Merge,
            FileExistsPolicy.Overwrite);
    });
}
