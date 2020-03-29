using System.Windows;
using System.Windows.Controls;

using SmartCmdArgs.ViewModel;

namespace SmartCmdArgs.View
{
    public class TreeItemStyleSelector : StyleSelector
    {
        public Style ProjectItemStyle { get; set; }
        public Style GroupItemStyle { get; set; }
        public Style ArgumentItemStyle { get; set; }
        public Style WorkingDirItemStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            switch (item)
            {
                case CmdProject _: return ProjectItemStyle;
                case CmdGroup _: return GroupItemStyle;
                case CmdArgument _: return ArgumentItemStyle;
                case CmdWorkingDir _: return WorkingDirItemStyle;
                default: return null;
            }
        }
    }

    public class TreeDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ProjectTemplate { get; set; }
        public DataTemplate GroupItemTemplate { get; set; }
        public DataTemplate ArgumentItemTemplate { get; set; }
        public DataTemplate WorkingDirectoryItemTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            switch (item)
            {
                case CmdProject _: return ProjectTemplate;
                case CmdGroup _: return GroupItemTemplate;
                case CmdArgument _: return ArgumentItemTemplate;
                case CmdWorkingDir _: return WorkingDirectoryItemTemplate;
                default: return null;
            }
        }
    }
}
