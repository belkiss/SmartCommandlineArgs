using System;

namespace SmartCmdArgs.ViewModel
{
    public class CmdWorkingDir : CmdBase
    {
        public override bool IsEditable => true;

        // only one working dir selectable at a time
        public override bool InExclusiveModeContainer => true;

        public new bool IsChecked
        {
            get => base.IsChecked == true;
            set => base.IsChecked = value;
        }

        public CmdWorkingDir(Guid id, string workingDir, bool isChecked = false)
            : base(id, workingDir, isChecked)
        { }

        public CmdWorkingDir(string workingDir, bool isChecked = false)
            : this(Guid.NewGuid(), workingDir, isChecked)
        { }

        public override CmdBase Copy()
        {
            return new CmdWorkingDir(Value, IsChecked);
        }
    }
}
