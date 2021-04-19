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
    public int? Revision { get; set; }
    public int? GuessedRevision { get; set; }
    public string Tag { get; set; }
    public bool IsDevMark { get; set; } = false;

    public int CompareTo(BuildVersion other)
    {
        if (!Revision.HasValue && !other.Revision.HasValue)
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

        int revisionCompare = CompareNullableInt(Revision, other.Revision);
        if (revisionCompare != 0)
            return revisionCompare;

        string normTag = Tag ?? string.Empty;
        string normOtherTag = other.Tag ?? string.Empty;
        if (normTag != normOtherTag)
            return normTag.CompareTo(normOtherTag);

        return 0;
    }

    private int CompareNullableInt(int? my, int? other)
    {
        if (my.HasValue && !other.HasValue)
            return 1;
        else if (!my.HasValue && other.HasValue)
            return -1;
        else if (my.HasValue && other.HasValue)
            if (my.Value == other.Value)
                return my.Value.CompareTo(other.Value);

        return 0;
    }

    public static BuildVersion DefaultInitial => new() { Minor = 1, IsDevMark = true };
}

static class GitTagParser
{
    public static BuildVersion Parse(string gitTag)
    {
        BuildVersion resultVersion = new();

        if (gitTag.StartsWith("fatal"))
        {
            return BuildVersion.DefaultInitial;
        }

        /*
         * git describe tag formats:
         * 
         * v2.0 (simple tag)
         * v2.0-sometag (tag with "tag" in semver understanding)
         * v2.0-sometag-30-g41e086cb (tag with semver-tag and number of commits after the tag)
         */
        var parts = gitTag.Split('-');
        if ((parts.Length > 1) && (parts[0] == "dev"))
        {
            resultVersion.IsDevMark = true;
            parts = parts[1..];
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
        }
        if (parts.Length > 2)
        {
            if (int.TryParse(parts[^2], out int revisionNum))
            {
                resultVersion.Revision = revisionNum;
            }
        }

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
            if (version.Revision.HasValue)
            {
                sb.Append('.');
                sb.Append(version.Revision.Value);
            }
        }

        return sb.ToString();
    }

    public static string AsAssemblyVersion(this BuildVersion version)
    {
        return $"{version.Major}.{version.Minor}.{version.Patch}.{version.Revision ?? 0}";
    }
}

class BranchVersioning
{
    public string Tag { get; set; }
    public bool? IncrementPatch { get; set; }
}

static class Versioning
{
    public static void WriteVersionToFile(string template, string version, params string[] files)
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

    public static BuildVersion ProjectVersion(Func<string, BranchVersioning> branchConfig, string commit = "HEAD")
    {
        string currentBranch = GitCurrentBranch();
        var currentVersion =
            Git($"describe --first-parent {commit}", null, null, default(int?), false, false, false)
            .Select(o => GitTagParser.Parse(o.Text)).OrderByDescending(tag => tag).FirstOrDefault();
        var branchVersioning = branchConfig(currentBranch) ?? new();

        Console.WriteLine($"currentBranch = {currentBranch ?? "(null)"}");
        Console.WriteLine($"currentVersion.Revision = {currentVersion.Revision?.ToString() ?? "(null)"}");
        Console.WriteLine($"currentVersion.Tag = {currentVersion.Tag?.ToString() ?? "(null)"}");
        Console.WriteLine($"currentVersion.Patch = {currentVersion.Patch.ToString() ?? "(null)"}");

        if (currentVersion.Revision.HasValue)
        {
            currentVersion.Tag ??= branchVersioning.Tag ?? currentBranch;

            if (!currentVersion.IsDevMark)
            {
                if (branchVersioning.IncrementPatch == true)
                {
                    currentVersion.Patch = currentVersion.Patch + 1;
                }
                else
                {
                    currentVersion.Minor = currentVersion.Minor + 1;
                }
            }
        }

        if (!currentVersion.Revision.HasValue && (!commit.EndsWith("~1")))
        {
            var prevCommitVersion = ProjectVersion(branchConfig, $"{commit}~1");
            if (prevCommitVersion.Revision.HasValue
                && (currentVersion.Major == prevCommitVersion.Major)
                && (currentVersion.Minor == prevCommitVersion.Minor)
                && (currentVersion.Patch == prevCommitVersion.Patch)
                && (currentVersion.Tag == prevCommitVersion.Tag))
            {
                currentVersion.GuessedRevision = prevCommitVersion.Revision + 1;
            }
        }

        return currentVersion;
    }
}
