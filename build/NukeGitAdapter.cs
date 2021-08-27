using System.Collections.Generic;
using System.Linq;
using static Nuke.Common.Tools.Git.GitTasks;

namespace AWZhome.GutenTag.Nuke
{
    public class NukeGitAdapter : GitAdapter
    {
        public NukeGitAdapter(VersioningConfig versioningConfig) : 
            base(versioningConfig)
        {
        }

        public override IEnumerable<string> ExecuteGit(string commandLine) => 
            Git(commandLine, null, null, default, false, false, false).Select(o => o.Text);
    }
}
