using System;
using System.IO;
using System.Linq;
using MimeKit;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Npm;
using Nuke.Common.Tools.Git;
using Nuke.Common.Tools.GitHub;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using Octokit;
using Settings;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.IO.CompressionTasks;
using static Nuke.Common.Tools.Git.GitTasks;
using static Nuke.Common.Tools.GitHub.GitHubTasks;
using static Nuke.Common.Tools.Npm.NpmTasks;
using static Nuke.Common.Utilities.ConsoleUtility;
using System.Xml;
using System.Globalization;
using Nuke.Common.Git;
using System.Text;

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
            var actor = Environment.GetEnvironmentVariable("GITHUB_ACTOR");
            Git($"config --global user.name '{actor}'");
            Git($"config --global user.email '{actor}@github.com'");
            if (IsServerBuild)
            {
                Git($"remote set-url origin https://{actor}:{GithubToken}@github.com/{GitRepository.GetGitHubOwner()}/{GitRepository.GetGitHubName()}.git");
            }
            var gitHubClient = new GitHubClient(new ProductHeaderValue("Nuke"));
            var authToken = new Credentials(GithubToken);
            gitHubClient.Credentials = authToken;
            var releaseNotes = new StringBuilder();
            var milestone = GitHubTasks.GetGitHubMilestone(GitRepository, GitVersion.MajorMinorPatch).Result;
            if (milestone == null){
                Logger.Warn($"Milestone not found for v{GitVersion.MajorMinorPatch}");
                releaseNotes.Append("No release notes for this version.");
                return;
            }

            var pullRequests = gitHubClient.Repository.PullRequest.GetAllForRepository(
                GitRepository.GetGitHubOwner(),
                GitRepository.GetGitHubName(),
                new PullRequestRequest{
                    State = ItemStateFilter.All,
                })
                .Result
                .Where(pr =>
                    pr.Milestone?.Title == milestone.Title &&
                    pr.Merged == true &&
                    pr.Milestone?.Title == GitVersion.MajorMinorPatch);

            releaseNotes
                .AppendLine($"# {GitRepository.GetGitHubName()} {milestone.Title}")
                .AppendLine()
                .AppendLine($"A total of {pullRequests.Count()} pull requests where merged in this release.")
                .AppendLine();

            var groups = pullRequests
                .GroupBy(pr =>
                    pr.Labels.Aggregate("", (a,b) => $"{a}, {b.Name}"),
                    (labels, prs) => new { labels, prs });
            
            groups.ForEach(group =>
            {
                releaseNotes.AppendLine($"## {group.labels}");
                group.prs.ForEach(pr =>
                {
                    releaseNotes.AppendLine($"- {pr.Title}. #{pr.Number} Thanks @{pr.User.Login}");
                });
            });

            releaseNotes
                .AppendLine()
                .AppendLine("## Checksums")
                .AppendLine("| File | MD5 checksum |")
                .AppendLine("|------|--------------|");
                
            var files = GlobFiles(Directories.ArtifactsDirectory, "*");
            files.ForEach(file => {
                var fileInfo = new FileInfo(file);
                var fileName = fileInfo.Name;
                var hash = GetFileHash(file);
                releaseNotes.AppendLine($"| {fileName} | {hash} |");
            });
            releaseNotes.AppendLine();

            var version = GitRepository.IsOnMainOrMasterBranch() ? GitVersion.MajorMinorPatch : GitVersion.NuGetVersionV2;
            GitLogger = (type, output) => Logger.Info(output);
            Git($"tag v{version}");
            Git($"push --tags");

            var newRelease = new NewRelease($"v{version}")
            {
                Body = releaseNotes.ToString(),
                Draft = true,
                Name = $"v{version}",
                TargetCommitish = GitVersion.Sha,
                Prerelease = !GitRepository.IsOnMainOrMasterBranch()
            };
            var release = gitHubClient.Repository.Release.Create(
                GitRepository.GetGitHubOwner(),
                GitRepository.GetGitHubName(),
                newRelease).Result;
            Logger.Info($"{release.Name} released !");

            var artifactFiles = GlobFiles(Directories.ArtifactsDirectory, "*");
            artifactFiles.ForEach(artifactFile => {
                var artifact = File.OpenRead(artifactFile);
                var artifactInfo = new FileInfo(artifactFile);
                var assetUpload = new ReleaseAssetUpload()
                {
                    FileName = artifactInfo.Name,
                    ContentType = MimeKit.MimeTypes.GetMimeType(artifactInfo.Name),
                    RawData = artifact
                };
                var asset = gitHubClient.Repository.Release.UploadAsset(release, assetUpload).Result;
                Logger.Info($"Asset {asset.Name} published at {asset.BrowserDownloadUrl}");
            });

            if (GitRepository.IsOnMainOrMasterBranch()){
                GitHubTasks.CloseGitHubMilestone(GitRepository, GitVersion.MajorMinorPatch);
            }
        });
}
