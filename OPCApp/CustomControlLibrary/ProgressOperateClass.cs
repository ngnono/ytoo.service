using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CustomControlLibrary
{

    public class ProgressOperateClass

    {
    public delegate void OnWorkerMethodCompleteDelegate(string message);
    public event OnWorkerMethodCompleteDelegate OnWorkerComplete;


        public void WorkerMethod()
        {
            Thread.Sleep(500);
            OnWorkerComplete("The processing is complete");
        }
    }
}
