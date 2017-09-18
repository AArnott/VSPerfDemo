using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace VSIXProject1
{
    internal class MyService : IMyService
    {
        public MyService(IServiceProvider sp)
        {
            ErrorHandler.ThrowOnFailure(VsShellUtilities.ShowMessageBox(sp, $"{nameof(MyService)} construction start", nameof(VSPackage1), OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST));

            Task.Delay(5000).Wait();

            ErrorHandler.ThrowOnFailure(VsShellUtilities.ShowMessageBox(sp, $"{nameof(MyService)} construction end", nameof(VSPackage1), OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST));
        }

        public int Add(int a, int b) => a + b;
    }
}
