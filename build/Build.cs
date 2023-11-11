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
using Serilog;
using Settings;

[GitHubActions(
    "PR_Validation",
    GitHubActionsImage.UbuntuLatest,
    ImportSecrets = new[] { nameof(GitHubToken) },
    OnPullRequestBranches = new [] {"master", "main", "develop", "development", "release/*"},
    InvokedTargets = new[] { nameof(Package)},
    FetchDepth = 0
)]
[GitHubActions(
    "Release",
    GitHubActionsImage.UbuntuLatest,
    ImportSecrets = new[] { nameof(GitHubToken) },
    OnPushBranches = new [] {"master", "main", "release/*"},
    InvokedTargets = new[] { nameof(Release)},
    FetchDepth = 0
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
    [Parameter("Github Token")] readonly string GitHubToken;

    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;
    [ThemeSettings] readonly ThemeSettings ThemeSettings;

    Target Clean => _ => _
        .Before(Compile)
        .Executes(() =>
        {
            Directories.ArtifactsDirectory.CreateOrCleanDirectory();
            Directories.StagingDirectory.CreateOrCleanDirectory();
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
                    Log.Debug($"Updated package {package.Attributes["name"].Value} to version {version.Value}");
                }
                doc.Save(manifest);
                Log.Debug($"Saved {manifest}");
        });

    Target Compile => _ => _
        .DependsOn(Install)
        .DependsOn(UpdateManifest)
        .Executes(() =>
        {
            NpmRun(s => s.SetCommand("build"));
        });

    Target StageFiles => _ => _
    .DependsOn(Clean)
    .DependsOn(Compile)
    .Executes(() => {
        (RootDirectory / "containers").GlobFiles("*.ascx")
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
            (Directories.StagingDirectory / "skin").CompressTo(Directories.StagingDirectory / "skinResources.zip");
            (Directories.StagingDirectory / "skin").DeleteDirectory();
            (Directories.StagingDirectory / "containers").CompressTo(Directories.StagingDirectory / "containersResources.zip");
            (Directories.StagingDirectory / "containers").DeleteDirectory();
            var releaseFile = Directories.ArtifactsDirectory / $"{ThemeSettings.Package.Name}_{GitVersion.MajorMinorPatch}.zip";
            Directories.StagingDirectory.CompressTo(releaseFile);
        });

    Target Release => _ => _
        .DependsOn(Package)
        .Executes(() => {
            var actor = Environment.GetEnvironmentVariable("GITHUB_ACTOR");
            Git($"config --global user.name '{actor}'");
            Git($"config --global user.email '{actor}@github.com'");
            if (IsServerBuild)
            {
                Git($"remote set-url origin https://{actor}:{GitHubToken}@github.com/{GitRepository.GetGitHubOwner()}/{GitRepository.GetGitHubName()}.git");
            }
            var gitHubClient = new GitHubClient(new ProductHeaderValue("Nuke"));
            var authToken = new Credentials(GitHubToken);
            gitHubClient.Credentials = authToken;
            var releaseNotes = new StringBuilder();
            var milestone = GitHubTasks.GetGitHubMilestone(GitRepository, GitVersion.MajorMinorPatch).Result;
            if (milestone == null){
                Log.Warning($"Milestone not found for v{GitVersion.MajorMinorPatch}");
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
                    pr.Labels.Aggregate("", (a,b) => $"{a} {b.Name}"),
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
                
            var files = Directories.ArtifactsDirectory.GlobFiles("*");
            files.ForEach(file => {
                var fileInfo = new FileInfo(file);
                var fileName = fileInfo.Name;
                var hash = file.GetFileHash();
                releaseNotes.AppendLine($"| {fileName} | {hash} |");
            });
            releaseNotes.AppendLine();

            var version = GitRepository.IsOnMainOrMasterBranch() ? GitVersion.MajorMinorPatch : GitVersion.NuGetVersionV2;
            GitLogger = (type, output) => Log.Information(output);
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
            Log.Information($"{release.Name} released !");

            var artifactFiles = Directories.ArtifactsDirectory.GlobFiles("*");
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
                Log.Information($"Asset {asset.Name} published at {asset.BrowserDownloadUrl}");
            });

            if (GitRepository.IsOnMainOrMasterBranch()){
                GitHubTasks.CloseGitHubMilestone(GitRepository, GitVersion.MajorMinorPatch);
            }
        });
}
