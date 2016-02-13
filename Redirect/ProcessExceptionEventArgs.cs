using System;

namespace Redirect
{
    public class ProcessExceptionEventArgs : EventArgs
    {
        public ProcessExceptionEventArgs(Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception { get; }
    }
}