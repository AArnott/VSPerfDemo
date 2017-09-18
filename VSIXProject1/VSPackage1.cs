using System;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace VSIXProject1
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideService(typeof(IMyService))]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
    [Guid(VSPackage1.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class VSPackage1 : Package
    {
        public const string PackageGuidString = "3f23421c-df28-4856-9427-761c3447581e";
        private uint sbmCookie = VSConstants.VSCOOKIE_NIL;
        private IVsSolutionBuildManager5 sbm;

        public VSPackage1()
        {
            // DON'T PUT ANYTHING HERE. JUST DELETE IT.
        }

        #region Package Members

        protected override void Initialize()
        {
            ErrorHandler.ThrowOnFailure(VsShellUtilities.ShowMessageBox(this, "Package.Initialize start", nameof(VSPackage1), OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST));

            base.Initialize();

            this.sbm = this.GetService(typeof(SVsSolutionBuildManager)) as IVsSolutionBuildManager5;
            if (this.sbm != null)
            {
                this.sbm.AdviseUpdateSolutionEvents4(new SolutionEventReceiver(this), out sbmCookie);
            }

            ((IServiceContainer)this).AddService(typeof(IMyService), (sc, st) => new MyService(this));
            Command1.Initialize(this);

            Task.Delay(5000).Wait();

            ErrorHandler.ThrowOnFailure(VsShellUtilities.ShowMessageBox(this, "Package.Initialize end", nameof(VSPackage1), OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.sbmCookie != VSConstants.VSCOOKIE_NIL && this.sbm != null)
                {
                    this.sbm.UnadviseUpdateSolutionEvents4(this.sbmCookie);
                    this.sbmCookie = VSConstants.VSCOOKIE_NIL;
                }
            }

            base.Dispose(disposing);
        }

        #endregion

        private class SolutionEventReceiver : IVsUpdateSolutionEvents4
        {
            private VSPackage1 package;
            private bool firstBuildHappened;

            public SolutionEventReceiver(VSPackage1 package)
            {
                this.package = package;
            }

            public void UpdateSolution_QueryDelayFirstUpdateAction(out int pfDelay)
            {
                pfDelay = 0;
            }

            public void UpdateSolution_BeginFirstUpdateAction()
            {
            }

            public void UpdateSolution_EndLastUpdateAction()
            {
            }

            public void UpdateSolution_BeginUpdateAction(uint dwAction)
            {
                this.firstBuildHappened = ((VSSOLNBUILDUPDATEFLAGS)dwAction & VSSOLNBUILDUPDATEFLAGS.SBF_OPERATION_BUILD) == VSSOLNBUILDUPDATEFLAGS.SBF_OPERATION_BUILD;
            }

            public void UpdateSolution_EndUpdateAction(uint dwAction)
            {
            }

            public void OnActiveProjectCfgChangeBatchBegin()
            {
            }

            public void OnActiveProjectCfgChangeBatchEnd()
            {
                if (!this.firstBuildHappened)
                {
                    return;
                }

                // Clear and refresh some cache I have.
                // And update the error list.
                // And send telemetry.
                // And walk the dog.
                Task.Delay(5000).Wait();

                VsShellUtilities.ShowMessageBox(
                    this.package,
                    $"This delay brought to you by {nameof(VSPackage1)} in response to a config change.",
                    nameof(VSPackage1),
                    OLEMSGICON.OLEMSGICON_INFO,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
        }
    }
}
