using PPBackup.Base.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPBackup.Gui.ViewModels
{
    public class NewStepTemplate
    {
        public string? Caption { get; set; }
        public Func<BackupStep>? Creator { get; set; }
    }

    public class NewStepTemplates
    {
        private List<NewStepTemplate> list;

        public IEnumerable<NewStepTemplate> List => list;

        public NewStepTemplates()
        {
            list = new()
            {
                new()
                {
                    Caption = "Add file sync",
                    Creator = () => new BackupStep("sync")
                    {
                        DefaultProperty = "file"
                    }
                },
                new()
                {
                    Caption = "Add directory sync",
                    Creator = () => new BackupStep("sync")
                    {
                        DefaultProperty = "dir"
                    }
                },
                new()
                {
                    Caption = "Add sub-directory sync",
                    Creator = () => new BackupStep("sync")
                    {
                        DefaultProperty = "sub-dirs"
                    }
                }
            };
        }
    }
}
