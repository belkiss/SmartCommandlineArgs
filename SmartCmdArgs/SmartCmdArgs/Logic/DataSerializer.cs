using System.Collections.Generic;
using SmartCmdArgs.ViewModel;

namespace SmartCmdArgs.Logic
{
	class DataSerializer
    {
        protected static List<CmdArgumentJson> TransformCmdList(ICollection<CmdBase> items)
        {
            var result = new List<CmdArgumentJson>(items.Count);
            foreach (var item in items)
            {
                var newElement = new CmdArgumentJson
                {
                    Id = item.Id,
                    ProjectConfig = item.ProjectConfig,
                    LaunchProfile = item.LaunchProfile,

                    // not in JSON
                    Selected = item.IsSelected,
                };

                if (item is CmdArgument arg)
                {
                    newElement.Command = item.Value;

                    // not in JSON
                    newElement.Enabled = item.IsChecked ?? false;
                }

                if (item is CmdWorkingDir workingDir)
                {
                    newElement.WorkingDir = item.Value;
                }

                if (item is CmdContainer container)
                {
                    newElement.Command = item.Value; // LCTODO: the Command field is probably useless for containers, check if that is really the case

                    newElement.Items = TransformCmdList(container.Items);
                    newElement.ExclusiveMode = container.ExclusiveMode;

                    // not in JSON
                    newElement.Expanded = container.IsExpanded;
                }

                result.Add(newElement);
            }
            return result;
        }
    }
}
