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
        public MyService()
        {
        }

        public int Add(int a, int b) => a + b;

        internal static async Task<MyService> CreateAsync(AsyncPackage sp)
        {
            var service = new MyService();

            // Our CPU intensive activity requires the UI thread,
            // so divide the work up into small chunks of 50ms each so the user can still interact.
            await sp.JoinableTaskFactory.SwitchToMainThreadAsync();
            for (int i = 0; i < 100; i++)
            {
                SpinWait.SpinUntil(() => false, 50);
                await Task.Yield();
            }

            await service.LotsOfIOAsync();

            return service;
        }

        private Task LotsOfIOAsync()
        {
            return Task.Delay(1000);
        }
    }
}
