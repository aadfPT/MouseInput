using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RawInput_dll
{
    public class PreMessageFilter : IMessageFilter
    {

        public PreMessageFilter(IntPtr deviceHandle)
        {
            DeviceHandle = deviceHandle;
        }

        public IntPtr DeviceHandle { get; set; }

        // true  to filter the message and stop it from being dispatched 
        // false to allow the message to continue to the next filter or control.
        public bool PreFilterMessage(ref Message m)
        {
            return false; // m.Msg == Win32.WM_KEYDOWN;
        }
    }
}
