using System;

namespace Redirect
{
    public class ProcessRequestEventArgs : EventArgs
    {
        public ProcessRequestEventArgs(Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception { get; }
    }
}