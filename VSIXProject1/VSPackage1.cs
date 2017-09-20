using System;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;
using Task = System.Threading.Tasks.Task;

namespace VSIXProject1
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideService(typeof(IMyService), IsAsyncQueryable = true)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string, PackageAutoLoadFlags.BackgroundLoad)]
    [Guid(VSPackage1.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class VSPackage1 : AsyncPackage
    {
        public const string PackageGuidString = "3f23421c-df28-4856-9427-761c3447581e";
        private uint sbmCookie = VSConstants.VSCOOKIE_NIL;
        private IVsSolutionBuildManager5 sbm;

        public VSPackage1()
        {
            // DON'T PUT ANYTHING HERE. JUST DELETE IT.
        }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);

            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            this.sbm = await this.GetServiceAsync(typeof(SVsSolutionBuildManager)) as IVsSolutionBuildManager5;
            if (this.sbm != null)
            {
                this.sbm.AdviseUpdateSolutionEvents4(new SolutionEventReceiver(this), out sbmCookie);
            }

            this.AddService(typeof(IMyService), async (sc, ct, st) => await MyService.CreateAsync(this));
            await Command1.InitializeAsync(this);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                if (this.sbmCookie != VSConstants.VSCOOKIE_NIL && this.sbm != null)
                {
                    this.sbm.UnadviseUpdateSolutionEvents4(this.sbmCookie);
                    this.sbmCookie = VSConstants.VSCOOKIE_NIL;
                }
            }

            base.Dispose(disposing);
        }

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
                SpinWait.SpinUntil(() => false, 5000);
            }
        }
    }
}
