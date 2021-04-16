using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class SemVersion : IComparable<SemVersion>
{
    public int Major { get; set; }
    public int? Minor { get; set; }
    public int? Patch { get; set; }
    public int? Revision { get; set; }
    public string Tag { get; set; }
    public bool IsDevMark { get; set; } = false;

    public int CompareTo(SemVersion other)
    {
        int majorCompare = Major.CompareTo(other.Major);
        if (majorCompare != 0)
            return majorCompare;

        int minorCompare = CompareNullableInt(Minor, other.Minor);
        if (minorCompare != 0)
            return minorCompare;

        int patchCompare = CompareNullableInt(Patch, other.Patch);
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
}

static class GitTagParser
{
    public static SemVersion Parse(string gitTag)
    {
        SemVersion resultVersion = new();

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

static class SemVersionFormatter
{
    public static string FormatAsSemVer(SemVersion version)
    {
        StringBuilder sb = new();
        sb.Append(version.Major);
        if (version.Minor.HasValue)
        {
            sb.Append('.');
            sb.Append(version.Minor.Value);
        }
        if (version.Patch.HasValue)
        {
            sb.Append('.');
            sb.Append(version.Patch.Value);
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

        // DEBUG
        if (version.IsDevMark)
        {
            sb.Append(" (dev mark)");
        }

        return sb.ToString();
    }

    public static string FormatAsAssemblyVersion(SemVersion version)
    {
        return $"{version.Major}.{version.Minor ?? 0}.{version.Patch ?? 0}.{version.Revision ?? 0}";
    }
}

static class Versioning
{

}
