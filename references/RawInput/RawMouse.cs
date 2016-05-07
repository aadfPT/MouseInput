using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

namespace RawInput_dll
{
    public sealed class RawMouse
    {
        private readonly Dictionary<IntPtr, MouseEvent> _deviceList = new Dictionary<IntPtr, MouseEvent>();
        public delegate void DeviceEventHandler(object sender, RawInputEventArg e);
        public event DeviceEventHandler MouseEvent;
        readonly object _padLock = new object();
        public int NumberOfMice { get; private set; }
        public Dictionary<IntPtr, MouseEvent> Devices => _deviceList;
        static InputData _rawBuffer;

        public RawMouse(IntPtr hwnd, bool captureOnlyInForeground)
        {
            var rid = new RawInputDevice[1];

            rid[0].UsagePage = HidUsagePage.GENERIC;
            rid[0].Usage = HidUsage.Mouse;
            rid[0].Flags = (captureOnlyInForeground ? RawInputDeviceFlags.NONE : RawInputDeviceFlags.INPUTSINK) | RawInputDeviceFlags.DEVNOTIFY;
            rid[0].Target = hwnd;

            if (!Win32.RegisterRawInputDevices(rid, (uint)rid.Length, (uint)Marshal.SizeOf(rid[0])))
            {
                throw new ApplicationException("Failed to register raw input mice devices.");
            }
        }

        public void EnumerateDevices()
        {
            lock (_padLock)
            {
                _deviceList.Clear();

                var mouseNumber = 0;

                var numberOfDevices = 0;
                uint deviceCount = 0;
                var dwSize = (Marshal.SizeOf(typeof(Rawinputdevicelist)));

                if (Win32.GetRawInputDeviceList(IntPtr.Zero, ref deviceCount, (uint)dwSize) != 0)
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                var pRawInputDeviceList = Marshal.AllocHGlobal((int)(dwSize * deviceCount));
                Win32.GetRawInputDeviceList(pRawInputDeviceList, ref deviceCount, (uint)dwSize);

                for (var i = 0; i < deviceCount; i++)
                {
                    uint pcbSize = 0;

                    // On Window 8 64bit when compiling against .Net > 3.5 using .ToInt32 you will generate an arithmetic overflow. Leave as it is for 32bit/64bit applications
                    var rid = (Rawinputdevicelist)Marshal.PtrToStructure(new IntPtr((pRawInputDeviceList.ToInt64() + (dwSize * i))), typeof(Rawinputdevicelist));

                    Win32.GetRawInputDeviceInfo(rid.hDevice, RawInputDeviceInfo.RIDI_DEVICENAME, IntPtr.Zero, ref pcbSize);

                    if (pcbSize <= 0) continue;

                    var pData = Marshal.AllocHGlobal((int)pcbSize);
                    Win32.GetRawInputDeviceInfo(rid.hDevice, RawInputDeviceInfo.RIDI_DEVICENAME, pData, ref pcbSize);
                    var deviceName = Marshal.PtrToStringAnsi(pData);

                    if (rid.dwType == DeviceType.RimTypemouse)
                    {
                        var deviceDesc = Win32.GetDeviceDescription(deviceName);

                        var dInfo = new MouseEvent()
                        {
                            DeviceName = Marshal.PtrToStringAnsi(pData),
                            DeviceHandle = rid.hDevice,
                            DeviceType = Win32.GetDeviceType(rid.dwType),
                            Name = deviceDesc,
                            Source = mouseNumber++.ToString(CultureInfo.InvariantCulture)
                        };

                        if (!_deviceList.ContainsKey(rid.hDevice))
                        {
                            numberOfDevices++;
                            _deviceList.Add(rid.hDevice, dInfo);
                        }
                    }

                    Marshal.FreeHGlobal(pData);
                }

                Marshal.FreeHGlobal(pRawInputDeviceList);

                NumberOfMice = numberOfDevices;
                Debug.WriteLine("EnumerateDevices() found {0} mice", NumberOfMice);
                return;
            }

            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        public bool ProcessRawInput(IntPtr hdevice, IEnumerable<IntPtr> suppressedDevices)
        {

            if (_deviceList.Count == 0) return false;

            var dwSize = 0;
            Win32.GetRawInputData(hdevice, DataCommand.RID_INPUT, IntPtr.Zero, ref dwSize, Marshal.SizeOf(typeof(Rawinputheader)));

            if (dwSize != Win32.GetRawInputData(hdevice, DataCommand.RID_INPUT, out _rawBuffer, ref dwSize, Marshal.SizeOf(typeof(Rawinputheader))))
            {
                Debug.WriteLine("Error getting the rawinput buffer");
                return false;
            }
            var isMouse = _rawBuffer.header.dwType == DeviceType.RimTypemouse;
            if (!isMouse) return false;
            var rawMouse = _rawBuffer.data.mouse;
            var flags = rawMouse.usFlags;
            if (rawMouse.usButtonFlags != 0)
            {
                Debug.WriteLine("usButtonFlags: " + rawMouse.usButtonFlags
                    + " usButtonData: " + (short)rawMouse.usButtonData);
            }
            var mouseMovedRelative = flags == (ushort)MouseFlags.MOUSE_MOVE_RELATIVE;


            if (!_deviceList.ContainsKey(_rawBuffer.header.hDevice)) return false;
            lock (_padLock)
            {
                var mouseEvent = _deviceList[_rawBuffer.header.hDevice];
                if (mouseMovedRelative)
                {
                    mouseEvent.X = rawMouse.lLastX;
                    mouseEvent.Y = rawMouse.lLastY;
                }
                else
                {
                    mouseEvent.X = mouseEvent.Y = 0;
                }
                mouseEvent.LeftDown = (rawMouse.usButtonFlags & (ushort)MouseButtonFlags.RI_MOUSE_LEFT_BUTTON_DOWN) != 0;
                mouseEvent.LeftUp = (rawMouse.usButtonFlags & (ushort)MouseButtonFlags.RI_MOUSE_LEFT_BUTTON_UP) != 0;
                mouseEvent.RightDown = (rawMouse.usButtonFlags & (ushort)MouseButtonFlags.RI_MOUSE_RIGHT_BUTTON_DOWN) != 0;
                mouseEvent.RightUp = (rawMouse.usButtonFlags & (ushort)MouseButtonFlags.RI_MOUSE_RIGHT_BUTTON_UP) != 0;
                mouseEvent.MiddleDown = (rawMouse.usButtonFlags & (ushort)MouseButtonFlags.RI_MOUSE_MIDDLE_BUTTON_DOWN) != 0;
                Debug.WriteLineIf(mouseEvent.MiddleDown, "Middle button down");
                mouseEvent.MiddleUp = (rawMouse.usButtonFlags & (ushort)MouseButtonFlags.RI_MOUSE_MIDDLE_BUTTON_UP) != 0;
                Debug.WriteLineIf(mouseEvent.MiddleUp, "Middle button up");
                mouseEvent.X1Down = (rawMouse.usButtonFlags & (ushort)MouseButtonFlags.RI_MOUSE_BUTTON_4_DOWN) != 0;
                mouseEvent.X1Up = (rawMouse.usButtonFlags & (ushort)MouseButtonFlags.RI_MOUSE_BUTTON_4_UP) != 0;
                mouseEvent.X2Down = (rawMouse.usButtonFlags & (ushort)MouseButtonFlags.RI_MOUSE_BUTTON_5_DOWN) != 0;
                mouseEvent.X2Up = (rawMouse.usButtonFlags & (ushort)MouseButtonFlags.RI_MOUSE_BUTTON_5_UP) != 0;
                mouseEvent.Wheel = (rawMouse.usButtonFlags & (ushort)MouseButtonFlags.RI_MOUSE_WHEEL) != 0;

                if (mouseEvent.Wheel)
                {
                    mouseEvent.WheelValue = (short)rawMouse.usButtonData;
                }

                MouseEvent?.Invoke(this, new RawInputEventArg(mouseEvent));
                return suppressedDevices.Contains(_rawBuffer.header.hDevice);
            }
        }

        private enum MouseFlags : ushort
        {
            MOUSE_ATTRIBUTES_CHANGED = 0x4,
            MOUSE_MOVE_RELATIVE = 0x0,
            MOUSE_MOVE_ABSOLUTE = 0x1,
            MOUSE_VIRTUAL_DESKTOP = 0x2

        }
        private enum MouseButtonFlags : ushort
        {
            RI_MOUSE_LEFT_BUTTON_DOWN = 0x0001,
            RI_MOUSE_LEFT_BUTTON_UP = 0x0002,
            RI_MOUSE_MIDDLE_BUTTON_DOWN = 0x0010,
            RI_MOUSE_MIDDLE_BUTTON_UP = 0x0020,
            RI_MOUSE_RIGHT_BUTTON_DOWN = 0x0004,
            RI_MOUSE_RIGHT_BUTTON_UP = 0x0008,
            RI_MOUSE_BUTTON_4_DOWN = 0x0040,
            RI_MOUSE_BUTTON_4_UP = 0x0080,
            RI_MOUSE_BUTTON_5_DOWN = 0x0100,
            RI_MOUSE_BUTTON_5_UP = 0x0200,
            RI_MOUSE_WHEEL = 0x0400,

        }
    }
}