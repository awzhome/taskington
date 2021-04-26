using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using static Nuke.Common.IO.TextTasks;
using static Nuke.Common.Tools.Git.GitTasks;

class BuildVersion : IComparable<BuildVersion>
{
    public int Major { get; set; }
    public int Minor { get; set; }
    public int Patch { get; set; }
    public int Revision { get; set; }
    public int AssemblyRevision { get; set; }
    public string Tag { get; set; }
    public bool IsDevMark { get; set; } = false;
    public string BasedOnGitTag { get; set; }

    public int CompareTo(BuildVersion other)
    {
        if ((Revision == 0) && (other.Revision == 0))
        {
            if (!IsDevMark && other.IsDevMark)
            {
                return -1;
            }
            else if (IsDevMark && !other.IsDevMark)
            {
                return 1;
            }
        }

        int majorCompare = Major.CompareTo(other.Major);
        if (majorCompare != 0)
            return majorCompare;

        int minorCompare = Minor.CompareTo(other.Minor);
        if (minorCompare != 0)
            return minorCompare;

        int patchCompare = Patch.CompareTo(other.Patch);
        if (patchCompare != 0)
            return patchCompare;

        int revisionCompare = Revision.CompareTo(other.Revision);
        if (revisionCompare != 0)
            return revisionCompare;

        string normTag = Tag ?? string.Empty;
        string normOtherTag = other.Tag ?? string.Empty;
        if (normTag != normOtherTag)
            return normTag.CompareTo(normOtherTag);

        return 0;
    }

    public static BuildVersion DefaultInitial => new() { Minor = 1, IsDevMark = true };
}

static class GitTagParser
{
    public static BuildVersion Parse(string gitTag)
    {
        if ((gitTag == null) || gitTag.StartsWith("fatal"))
        {
            return null;
        }

        BuildVersion resultVersion = new();

        /*
         * git describe tag formats:
         * 
         * v2.0 (simple tag)
         * v2.0-sometag (tag with "tag" in semver understanding)
         * v2.0-sometag-30-g41e086cb (tag with semver-tag and number of commits after the tag)
         */
        var parts = gitTag.Split('-');
        if (parts.Length > 0)
        {
            resultVersion.BasedOnGitTag = parts[0];
        }

        if ((parts.Length > 1) && (parts[0] == "dev"))
        {
            resultVersion.IsDevMark = true;
            parts = parts[1..];
            resultVersion.BasedOnGitTag += "-" + parts[0];
        }

        if (parts.Length > 0)
        {
            var baseVersionParts = parts[0].Split('.');
            if (baseVersionParts.Length > 0)
            {
                string major = baseVersionParts[0];
                if (major.StartsWith("v"))
                {
                    major = major[1..];
                }
                if (int.TryParse(major, out int majorNum))
                {
                    resultVersion.Major = majorNum;
                }
            }
            if (baseVersionParts.Length > 1)
            {
                if (int.TryParse(baseVersionParts[1], out int minorNum))
                {
                    resultVersion.Minor = minorNum;
                }
            }
            if (baseVersionParts.Length > 2)
            {
                if (int.TryParse(baseVersionParts[2], out int patchNum))
                {
                    resultVersion.Patch = patchNum;
                }
            }
        }
        if ((parts.Length == 2) || (parts.Length == 4))
        {
            resultVersion.Tag = parts[1];
            resultVersion.BasedOnGitTag += "-" + parts[1];
        }
        if (parts.Length > 2)
        {
            if (int.TryParse(parts[^2], out int revisionNum))
            {
                resultVersion.Revision = revisionNum;
            }
        }

        resultVersion.AssemblyRevision = resultVersion.Revision;

        return resultVersion;
    }
}

static class SemVersionFormatterExtensions
{
    public static string AsString(this BuildVersion version)
    {
        StringBuilder sb = new();
        sb.Append(version.Major);
        sb.Append('.');
        sb.Append(version.Minor);
        if (version.Patch != 0)
        {
            sb.Append('.');
            sb.Append(version.Patch);
        }
        if (version.Tag != null)
        {
            sb.Append('-');
            sb.Append(version.Tag);
            if (version.Revision != 0)
            {
                sb.Append('.');
                sb.Append(version.Revision);
            }
        }

        return sb.ToString();
    }

    public static string AsAssemblyVersion(this BuildVersion version)
    {
        return $"{version.Major}.{version.Minor}.{version.Patch}.{version.AssemblyRevision}";
    }
}

class BranchVersioning
{
    public string Tag { get; set; }
    public bool IncrementPatch { get; set; } = true;
}

static class Versioning
{
    public static void WriteVersionToFiles(string template, string version, params string[] files)
    {
        WriteVersionToFiles(template, version, (IEnumerable<string>) files);
    }

    public static void WriteVersionToFiles(string template, string version, IEnumerable<string> files)
    {
        foreach (var file in files)
        {
            string origText = ReadAllText(file, Encoding.UTF8);
            string newText = Regex.Replace(origText,
                template.Replace("$$$", "(.*)"),
                template.Replace("$$$", version));
            WriteAllText(file, newText, Encoding.UTF8);
        }
    }

    public static BuildVersion ProjectVersion(Func<string, BranchVersioning> branchConfig)
    {
        string currentBranch = GitCurrentBranch();
        var branchVersioning = branchConfig(currentBranch) ?? new();
        var currentVersion = GitDescribe(new[] { "dev-*", "v*" });

        var correctedMatchPatterns = branchVersioning.IncrementPatch ?
                new[] { "dev-[0-9999]", "v[0-9999]", "dev-[0-9999].[0-9999]", "v[0-9999].[0-9999]", "dev-[0-9999].[0-9999].[0-9999]", "v[0-9999].[0-9999].[0-9999]", } :
                new[] { "dev-[0-9999]", "v[0-9999]", "dev-[0-9999].[0-9999]", "v[0-9999].[0-9999]", "dev-[0-9999].[0-9999].0", "v[0-9999].[0-9999].0" };
        var correctedVersion = GitDescribe(correctedMatchPatterns);

        if (currentVersion.Revision != 0)
        {
            currentVersion = correctedVersion;

            currentVersion.Tag ??= branchVersioning.Tag ?? currentBranch;

            if (!currentVersion.IsDevMark)
            {
                if (branchVersioning.IncrementPatch)
                {
                    currentVersion.Patch = currentVersion.Patch + 1;
                }
                else
                {
                    currentVersion.Minor = currentVersion.Minor + 1;
                }
            }
        }
        else
        {
            var versionWithoutCurrentTag = GitDescribe(correctedMatchPatterns, new[] { currentVersion.BasedOnGitTag });

            if ((versionWithoutCurrentTag.Major == currentVersion.Major)
                && (versionWithoutCurrentTag.Minor == currentVersion.Minor)
                && (versionWithoutCurrentTag.Patch == currentVersion.Patch))
            {
                currentVersion.AssemblyRevision = versionWithoutCurrentTag.Revision + 1;
            }

        }

        return currentVersion;
    }

    private static BuildVersion GitDescribe(string[] matches = null, string[] excludes = null)
    {
        string matchParam = "";
        if (matches != null)
        {
            matchParam += string.Join(' ', matches.Select(m => $"--match \"{m}\""));
        }
        string excludeParam = "";
        if (excludes != null)
        {
            excludeParam += string.Join(' ', excludes.Select(e => $"--exclude \"{e}\""));
        }
        return GitTagParser.Parse(
            Git($"describe --first-parent {matchParam} {excludeParam}", null, null, default(int?), false, false, false)
                .Select(o => o.Text).FirstOrDefault());
    }
}
