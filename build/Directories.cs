using Nuke.Common.IO;
using static Nuke.Common.NukeBuild;

public class Directories{
    public static AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    public static AbsolutePath DistDirectory => RootDirectory / "dist";
}