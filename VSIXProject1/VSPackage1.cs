using System;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
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

        protected override void Initialize()
        {
            base.Initialize();

            ThreadHelper.ThrowIfNotOnUIThread();
            this.sbm = this.GetService(typeof(SVsSolutionBuildManager)) as IVsSolutionBuildManager5;
            if (this.sbm != null)
            {
                this.sbm.AdviseUpdateSolutionEvents4(new SolutionEventReceiver(this), out sbmCookie);
            }

            SpinWait.SpinUntil(() => false, 5000); // some intense CPU activity
            this.LotsOfIO(); // some I/O

            ((IServiceContainer)this).AddService(typeof(IMyService), (sc, st) => new MyService(this));
            Command1.Initialize(this);
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

        private void LotsOfIO()
        {
#pragma warning disable VSTHRD002 // it's just a demo
            Task.Delay(1000).Wait();
#pragma warning restore VSTHRD002
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
