using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TaskStatusCenter;
using IAsyncServiceProvider = Microsoft.VisualStudio.Shell.IAsyncServiceProvider;
using Task = System.Threading.Tasks.Task;

namespace VSIXProject1
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class Command1
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("3dc035c0-5e54-4db2-a36a-a75868a4fdda");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="Command1"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private Command1(AsyncPackage package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static Command1 Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            Instance = new Command1(package);

            OleMenuCommandService commandService = await Instance.ServiceProvider.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(Instance.MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        private void MenuItemCallback(object sender, EventArgs e)
        {
            this.package.JoinableTaskFactory.StartOnIdle(async delegate
            {
                var tsc = await this.package.GetServiceAsync(typeof(SVsTaskStatusCenterService)) as IVsTaskStatusCenterService;
                var taskHandler = tsc.PreRegister(
                    new TaskHandlerOptions
                    {
                        Title = "Calculating awesome value",
                        ActionsAfterCompletion = CompletionActions.RetainOnFaulted | CompletionActions.RetainOnRanToCompletion,
                        DisplayTaskDetails = t =>
                        {
                            Task<int> resultTask = (Task<int>)t;
                            if (resultTask.IsCompleted)
                            {
                                VsShellUtilities.ShowMessageBox(
                                    this.package,
                                    $"And here's the answer: 5 + 3 = {resultTask.Result}",
                                    "Solution",
                                    OLEMSGICON.OLEMSGICON_INFO,
                                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                            }
                        }
                    },
                    new TaskProgressData { CanBeCanceled = true });

                var longRunningTask = Task.Run(async delegate
                {
                    IMyService svc = (IMyService)await this.ServiceProvider.GetServiceAsync(typeof(IMyService));
                    taskHandler.UserCancellation.ThrowIfCancellationRequested();
                    int eight = svc.Add(5, 3);
                    return eight;
                });
                taskHandler.RegisterTask(longRunningTask);
                await longRunningTask;
            });
        }
    }
}
