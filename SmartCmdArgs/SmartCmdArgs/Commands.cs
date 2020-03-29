//------------------------------------------------------------------------------
// <copyright file="Commands.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SmartCmdArgs.Helper;

using Task = System.Threading.Tasks.Task;

namespace SmartCmdArgs
{
	/// <summary>
	/// Command handler
	/// </summary>
	internal sealed class Commands
    {
        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly CmdArgsPackage package;

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(CmdArgsPackage package)
        {
            if (package == null)
                throw new ArgumentNullException(nameof(package));

            var cmdService = await package.GetServiceAsync<IMenuCommandService, OleMenuCommandService>();

            // AddCommand needs to be run on main thread!
            await package.JoinableTaskFactory.SwitchToMainThreadAsync();

            Instance = new Commands(package, cmdService);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Commands"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private Commands(CmdArgsPackage package, OleMenuCommandService commandService)
        {
            this.package = package;

            if (commandService != null)
            {
                AddCommandToService(commandService, PackageGuids.guidVSMenuCmdSet, PackageIds.ToolWindowCommandId, this.ShowToolWindow);

                AddCommandToService(commandService, PackageGuids.guidCmdArgsToolBarCmdSet, PackageIds.ToolbarAddCommandId, package.ToolWindowViewModel.AddEntryCommand);
                AddCommandToService(commandService, PackageGuids.guidCmdArgsToolBarCmdSet, PackageIds.ToolbarAddGroupCommandId, package.ToolWindowViewModel.AddGroupCommand);
                AddCommandToService(commandService, PackageGuids.guidCmdArgsToolBarCmdSet, PackageIds.ToolbarRemoveCommandId, package.ToolWindowViewModel.RemoveEntriesCommand);
                AddCommandToService(commandService, PackageGuids.guidCmdArgsToolBarCmdSet, PackageIds.ToolbarMoveUpCommandId, package.ToolWindowViewModel.MoveEntriesUpCommand);
                AddCommandToService(commandService, PackageGuids.guidCmdArgsToolBarCmdSet, PackageIds.ToolbarMoveDownCommandId, package.ToolWindowViewModel.MoveEntriesDownCommand);
                AddCommandToService(commandService, PackageGuids.guidCmdArgsToolBarCmdSet, PackageIds.ToolbarCopyCommandlineCommandId, package.ToolWindowViewModel.CopyCommandlineCommand);

                AddToggleCommandToService(commandService, PackageGuids.guidCmdArgsToolBarCmdSet, PackageIds.ToolbarShowAllProjectsCommandId,
                    package.ToolWindowViewModel.ShowAllProjectsCommand, () => package.ToolWindowViewModel.TreeViewModel.ShowAllProjects);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static Commands Instance
        {
            get;
            private set;
        }

        private void AddCommandToService(OleMenuCommandService service, Guid cmdSet, int cmdId, EventHandler handler)
        {
            var commandId = new CommandID(cmdSet, cmdId);
            var menuCommand = new MenuCommand(handler, commandId);
            service.AddCommand(menuCommand);
        }

        private void AddCommandToService(OleMenuCommandService service, Guid cmdSet, int cmdId, RelayCommand relayCommand)
        {
            var commandId = new CommandID(cmdSet, cmdId);
            var menuCommand = new OleMenuCommand((sender, args) =>
            {
                relayCommand.SafeExecute();
            }, commandId);

            menuCommand.BeforeQueryStatus += (sender, args) =>
            {
                menuCommand.Enabled = relayCommand.CanExecute(null);
            };

            service.AddCommand(menuCommand);
        }

        private void AddToggleCommandToService(OleMenuCommandService service, Guid cmdSet, int cmdId, RelayCommand relayCommand, Func<bool> isChecked)
        {
            var commandId = new CommandID(cmdSet, cmdId);
            var menuCommand = new OleMenuCommand((sender, args) =>
            {
                relayCommand.SafeExecute();
            }, commandId);

            menuCommand.BeforeQueryStatus += (sender, args) =>
            {
                menuCommand.Enabled = relayCommand.CanExecute(null);
                menuCommand.Checked = isChecked();
            };

            service.AddCommand(menuCommand);
        }

        /// <summary>
        /// Shows the tool window when the menu item is clicked.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        private void ShowToolWindow(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            ToolWindowPane window = this.package.FindToolWindow(typeof(ToolWindow), 0, true);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException("Cannot create tool window");
            }

            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            windowFrame.SetProperty((int)__VSFPROPID.VSFPROPID_CmdUIGuid, PackageGuids.guidToolWindowString);
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }
    }
}
