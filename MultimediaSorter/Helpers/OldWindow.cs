using System;
using System.Windows.Forms;

namespace PhotoSorter
{
    internal class OldWindow : IWin32Window
    {
        readonly IntPtr _handle;

        public OldWindow(IntPtr handle)
        {
            _handle = handle;
        }

        IntPtr IWin32Window.Handle => _handle;
    }
}
