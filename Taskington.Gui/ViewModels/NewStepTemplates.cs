using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taskington.Base.Steps;

namespace Taskington.Gui.ViewModels
{
    public class NewStepTemplate
    {
        public string? Caption { get; set; }
        public Func<PlanStep>? Creator { get; set; }
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
                    Caption = "Synchronize file",
                    Creator = () => new PlanStep("sync")
                    {
                        DefaultProperty = "file"
                    }
                },
                new()
                {
                    Caption = "Synchronize directory",
                    Creator = () => new PlanStep("sync")
                    {
                        DefaultProperty = "dir"
                    }
                },
                new()
                {
                    Caption = "Synchronize sub-directories",
                    Creator = () => new PlanStep("sync")
                    {
                        DefaultProperty = "sub-dirs"
                    }
                }
            };
        }
    }
}
