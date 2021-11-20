using System;
using System.IO;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Npm;
using Nuke.Common.Tools.GitHub;
using Nuke.Common.Tools.GitReleaseManager;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using Settings;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.IO.CompressionTasks;
using static Nuke.Common.Tools.GitHub.GitHubTasks;
using static Nuke.Common.Tools.GitReleaseManager.GitReleaseManagerTasks;
using static Nuke.Common.Tools.Npm.NpmTasks;
using static Nuke.Common.Utilities.ConsoleUtility;
using System.Xml;
using System.Globalization;
using Nuke.Common.Git;

[GitHubActions(
    "PR_Validation",
    GitHubActionsImage.WindowsLatest,
    ImportGitHubTokenAs = "GithubToken",
    OnPullRequestBranches = new [] {"master", "main", "develop", "development", "release/*"},
    InvokedTargets = new[] { nameof(Package)}
)]
[GitHubActions(
    "Release",
    GitHubActionsImage.WindowsLatest,
    ImportGitHubTokenAs = "GithubToken",
    OnPushBranches = new [] {"master", "main", "release/*"},
    InvokedTargets = new[] { nameof(Release)}
)]
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
    [Parameter("Github Token")] readonly string GithubToken;

    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;
    [ThemeSettings] readonly ThemeSettings ThemeSettings;

    Target Clean => _ => _
        .Before(Compile)
        .Executes(() =>
        {
            EnsureCleanDirectory(Directories.ArtifactsDirectory);
            EnsureCleanDirectory(Directories.StagingDirectory);
        });

    Target Install => _ => _
        .Executes(() => {
            NpmInstall();
        });

    Target UpdateManifest => _ => _
        .Executes(() => {
            ThemeSettings.Version = new Version(GitVersion.MajorMinorPatch);
            ThemeSettings.SaveSettings();
            var manifest = RootDirectory / "manifest.xml";
            var doc = new XmlDocument();
                doc.Load(manifest);
                var packages = doc.SelectNodes("dotnetnuke/packages/package");
                foreach (XmlNode package in packages)
                {
                    var packageName = package.Attributes["Package"];
                    if (packageName != null){
                        packageName.Value = ThemeSettings.Package.Name;
                    }

                    var version = package.Attributes["version"];
                    version.Value =
                        GitVersion != null
                        ? $"{GitVersion.Major.ToString("00", CultureInfo.InvariantCulture)}.{GitVersion.Minor.ToString("00", CultureInfo.InvariantCulture)}.{GitVersion.Patch.ToString("00", CultureInfo.InvariantCulture)}"
                        : "00.01.00";

                    var packageFriendlyName = package.SelectSingleNode("friendlyName");
                    packageFriendlyName.InnerText = ThemeSettings.Package.FriendlyName;

                    var packageDescription = package.SelectSingleNode("description");
                    packageDescription.InnerText = ThemeSettings.Package.Description;

                    var owner = package.SelectSingleNode("owner");
                    owner.SelectSingleNode("name").InnerText = ThemeSettings.Owner.Name;
                    owner.SelectSingleNode("organization").InnerText = ThemeSettings.Owner.Organization;
                    owner.SelectSingleNode("url").InnerText = ThemeSettings.Owner.Url;
                    owner.SelectSingleNode("email").InnerText = ThemeSettings.Owner.Email;
                   
                    var components = package.SelectNodes("components/component");
                    foreach (XmlNode component in components)
                    {
                        if (component.Attributes["type"].Value == "ResourceFile"){
                            var resourceFiles = component.SelectSingleNode("resourceFiles");
                            var basePath = resourceFiles.SelectSingleNode("basePath");
                            var resourceFile = resourceFiles.SelectSingleNode("resourceFile");
                            var name = resourceFile.SelectSingleNode("name");
                            if (name.InnerText == "containersResources.zip"){
                                basePath.InnerText = ThemeSettings.ContainersPath;
                            }
                            if (name.InnerText == "skinResources.zip"){
                                basePath.InnerText = ThemeSettings.SkinPath;
                            }
                        }

                        if (component.Attributes["type"].Value == "Skin"){
                            var skinFiles = component.SelectSingleNode("skinFiles");
                            skinFiles.SelectSingleNode("skinName").InnerText = ThemeSettings.Package.Name;
                            skinFiles.SelectSingleNode("basePath").InnerText = ThemeSettings.SkinPath;
                        }
                    }
                    
                    Logger.Normal($"Updated package {package.Attributes["name"].Value} to version {version.Value}");
                }
                doc.Save(manifest);
                Logger.Normal($"Saved {manifest}");
        });

    Target Compile => _ => _
        .DependsOn(Install)
        .DependsOn(UpdateManifest)
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
        ThemeSettings.TestSiteUrl = PromptForInput(
            "What is the website url to use for local development?",
            ThemeSettings.TestSiteUrl
        );
        ThemeSettings.SaveSettings();
    });

    Target StageFiles => _ => _
    .DependsOn(Clean)
    .DependsOn(Compile)
    .Executes(() => {
        GlobFiles(RootDirectory / "containers", "*.ascx")
            .ForEach(file =>
                CopyFileToDirectory(
                    file,
                    Directories.StagingDirectory / "containers",
                    FileExistsPolicy.Overwrite,
                    true));
        CopyFileToDirectory(
            RootDirectory / "LICENSE",
            Directories.StagingDirectory,
            FileExistsPolicy.Overwrite,
            true);
        CopyFile(
            RootDirectory / "manifest.xml",
            Directories.StagingDirectory / "manifest.dnn",
            FileExistsPolicy.Overwrite,
            true);
        CopyFileToDirectory(
            RootDirectory / "releaseNotes.txt",
            Directories.StagingDirectory,
            FileExistsPolicy.Overwrite,
            true);
        CopyDirectoryRecursively(
            RootDirectory / "images",
            Directories.StagingDirectory / "skin" / "Images",
            DirectoryExistsPolicy.Merge,
            FileExistsPolicy.Overwrite);
        CopyDirectoryRecursively(
            RootDirectory / "html",
            Directories.StagingDirectory / "skin",
            DirectoryExistsPolicy.Merge,
            FileExistsPolicy.Overwrite);
    });

    Target Package => _ => _
        .DependsOn(StageFiles)
        .Produces(Directories.ArtifactsDirectory)
        .Executes(() => {
            Compress(
                Directories.StagingDirectory / "skin",
                Directories.StagingDirectory / "skinResources.zip");
            DeleteDirectory(Directories.StagingDirectory / "skin");
            Compress(
                Directories.StagingDirectory / "containers",
                Directories.StagingDirectory / "containersResources.zip");
            DeleteDirectory(Directories.StagingDirectory / "containers");
            var releaseFile = Directories.ArtifactsDirectory / $"{ThemeSettings.Package.Name}_{GitVersion.MajorMinorPatch}.zip";
            Compress(
                Directories.StagingDirectory,
                releaseFile);
        });

    Target Release => _ => _
        .DependsOn(Package)
        .Executes(() => {
            GitReleaseManagerCreate(s => s
                .SetProcessArgumentConfigurator(a => a
                    .Add($"--token {GithubToken}")
                    .When(GitRepository.IsOnReleaseBranch(), a => a.Add("--pre")))
                .SetRepositoryOwner(GitRepository.GetGitHubOwner())
                .SetRepositoryName(GitRepository.GetGitHubName())
                .AddAssetPaths(Directories.ArtifactsDirectory));
        });
}
