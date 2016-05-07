using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RawInput_dll
{
    public class PreMessageFilter : IMessageFilter
    {
        //private RawInput RawInput { get; }

        public PreMessageFilter()
        {
        }

        //public PreMessageFilter(RawInput rawInput)
        //{
        //    this.RawInput = rawInput;
        //}

        // true  to filter the message and stop it from being dispatched 
        // false to allow the message to continue to the next filter or control.
        public bool PreFilterMessage(ref Message m)
        {
            //switch (m.Msg)
            //{
            //    case Win32.WM_INPUT:
            //        {
            //            var result = RawInput._keyboardDriver.ProcessRawInput(m.LParam, RawInput.SuppressedDevices);
            //            result = result || RawInput._mouseDriver.ProcessRawInput(m.LParam, RawInput.SuppressedDevices);
            //            return result;
            //        }
            //}
            return false;
        }
    }
}
