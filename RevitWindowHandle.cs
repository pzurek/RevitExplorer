using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace RevitExplorer
{
    class RevitWindowHandle : IWin32Window
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
