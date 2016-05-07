using System;

namespace RawInput_dll
{
    public class MouseEvent
    {
        public string DeviceName;       // i.e. \\?\HID#VID_045E&PID_00DD&MI_00#8&1eb402&0&0000#{884b96c3-56ef-11d1-bc8c-00a0c91405dd}
        public string DeviceType;       // KEYBOARD, MOUSE or HID
        public IntPtr DeviceHandle;     // Handle to the device that sent the input
        public string Name;             // i.e. Microsoft USB Comfort Curve Keyboard 2000 (Mouse and Keyboard Center)
        private string _source;         // Keyboard_XX
                                        //public int VButton;                // Virtual Key. Corrected for L/R keys(i.e. LSHIFT/RSHIFT) and Zoom
                                        //public string VButtonName;         // Virtual Key Name. Corrected for L/R keys(i.e. LSHIFT/RSHIFT) and Zoom
                                        //public uint Message;            // WM_KEYDOWN or WM_KEYUP        
                                        // public string ButtonPressState;    // MAKE or BREAK

        public string Source
        {
            get { return _source; }
            set { _source = string.Format("Mouse_{0}", value.PadLeft(2, '0')); }
        }

        public short WheelValue { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool LeftDown { get; set; }
        public bool LeftUp { get; set; }
        public bool RightDown { get; set; }
        public bool RightUp { get; set; }
        public bool MiddleDown { get; set; }
        public bool MiddleUp { get; set; }
        public bool X1Down { get; set; }
        public bool X1Up { get; set; }
        public bool X2Down { get; set; }
        public bool X2Up { get; set; }
        public bool Wheel { get; set; }

        public override string ToString()
        {
            return string.Format("Device\n DeviceName: {0}\n DeviceType: {1}\n DeviceHandle: {2}\n Name: {3}\n", DeviceName, DeviceType, DeviceHandle.ToInt64().ToString("X"), Name);
        }
    }
}