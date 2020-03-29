using System;

namespace SmartCmdArgs.ViewModel
{
	public class CmdArgument : CmdBase
    {
        public override bool IsEditable => true;

        public new bool IsChecked
        {
            get => base.IsChecked == true;
            set => base.IsChecked = value;
        }

        public CmdArgument(Guid id, string arg, bool isChecked = false)
            : base(id, arg, isChecked)
        { }

        public CmdArgument(string arg, bool isChecked = false)
            : this(Guid.NewGuid(), arg, isChecked)
        { }

        public override CmdBase Copy()
        {
            return new CmdArgument(Value, IsChecked);
        }
    }
}
