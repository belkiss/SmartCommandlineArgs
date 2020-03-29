using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartCmdArgs.ViewModel
{
	public class CmdGroup : CmdContainer
    {
        public override bool IsEditable => true;

        public new string ProjectConfig
        {
            get => base.ProjectConfig;
            set => base.ProjectConfig = value;
        }

        public new string LaunchProfile
        {
            get => base.LaunchProfile;
            set => base.LaunchProfile = value;
        }

        public CmdGroup(Guid id, string name, IEnumerable<CmdBase> items, bool isExpanded, bool exclusiveMode, string projConf, string launchProfile)
            : base(id, name, items, isExpanded, exclusiveMode)
        {
            base.ProjectConfig = projConf;
            base.LaunchProfile = launchProfile;
        }

        public CmdGroup(string name, IEnumerable<CmdBase> items = null, bool isExpanded = true, bool exclusiveMode = false, string projConf = null, string launchProfile = null)
            : this(Guid.NewGuid(), name, items, isExpanded, exclusiveMode, projConf, launchProfile)
        { }

        public override CmdBase Copy()
        {
            return new CmdGroup(Value, Items.Select(cmd => cmd.Copy()), isExpanded, ExclusiveMode);
        }
    }
}
