using System;
using System.Diagnostics;

namespace RevitExplorer
{
    class RevitWindowHandle
    {
        IntPtr revitWindowHandle;

        public RevitWindowHandle(IntPtr handle)
        {
            Debug.Assert(handle != IntPtr.Zero, "Expected non-null window hadle");

            revitWindowHandle = handle;
        }

        public IntPtr Handle
        {
            get { return revitWindowHandle; }
        }
    }
}
