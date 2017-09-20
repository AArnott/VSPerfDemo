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
            SpinWait.SpinUntil(() => false, 5000);
        }

        public int Add(int a, int b) => a + b;
    }
}
