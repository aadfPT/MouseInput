using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace RawInput_dll
{
    public class RawInput : NativeWindow
    {
        static RawKeyboard _keyboardDriver;
        static RawMouse _mouseDriver;
        readonly IntPtr _devNotifyHandle;
        static readonly Guid DeviceInterfaceHid = new Guid("4D1E55B2-F16F-11CF-88CB-001111000030");
        private List<PreMessageFilter> Filters { get; set; } = new List<PreMessageFilter>();

        public event RawKeyboard.DeviceEventHandler KeyPressed
        {
            add { _keyboardDriver.KeyPressed += value; }
            remove { _keyboardDriver.KeyPressed -= value; }
        }
        public event RawMouse.DeviceEventHandler MouseEventRegistered
        {
            add { _mouseDriver.MouseEvent += value; }
            remove { _mouseDriver.MouseEvent -= value; }
        }
        public int NumberOfKeyboards => _keyboardDriver.NumberOfKeyboards;

        public int NumberOfMice => _mouseDriver.NumberOfMice;
        public Dictionary<IntPtr, MouseEvent> MiceDevices => _mouseDriver.Devices;
        public Dictionary<IntPtr, KeyPressEvent> KeyboardsDeviceHandles => _keyboardDriver.DevicesHandles;

        public void AddMessageFilter(IntPtr deviceHandle)
        {
            var preMessageFilter = new PreMessageFilter(deviceHandle);
            Filters.Add(preMessageFilter);
            Application.AddMessageFilter(preMessageFilter);
        }

        private void RemoveMessageFilters()
        {
            if (null == Filters) return;
            foreach (var preMessageFilter in Filters)
            {
                Application.RemoveMessageFilter(preMessageFilter);
            }
        }

        public RawInput(IntPtr parentHandle, bool captureOnlyInForeground)
        {
            AssignHandle(parentHandle);

            _keyboardDriver = new RawKeyboard(parentHandle, captureOnlyInForeground);
            _keyboardDriver.EnumerateDevices();
            _mouseDriver = new RawMouse(parentHandle, captureOnlyInForeground);
            _mouseDriver.EnumerateDevices();
            _devNotifyHandle = RegisterForDeviceNotifications(parentHandle);
        }

        static IntPtr RegisterForDeviceNotifications(IntPtr parent)
        {
            var usbNotifyHandle = IntPtr.Zero;
            var bdi = new BroadcastDeviceInterface();
            bdi.DbccSize = Marshal.SizeOf(bdi);
            bdi.BroadcastDeviceType = BroadcastDeviceType.DBT_DEVTYP_DEVICEINTERFACE;
            bdi.DbccClassguid = DeviceInterfaceHid;

            var mem = IntPtr.Zero;
            try
            {
                mem = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BroadcastDeviceInterface)));
                Marshal.StructureToPtr(bdi, mem, false);
                usbNotifyHandle = Win32.RegisterDeviceNotification(parent, mem, DeviceNotification.DEVICE_NOTIFY_WINDOW_HANDLE);
            }
            catch (Exception e)
            {
                Debug.Print("Registration for device notifications Failed. Error: {0}", Marshal.GetLastWin32Error());
                Debug.Print(e.StackTrace);
            }
            finally
            {
                Marshal.FreeHGlobal(mem);
            }

            if (usbNotifyHandle == IntPtr.Zero)
            {
                Debug.Print("Registration for device notifications Failed. Error: {0}", Marshal.GetLastWin32Error());
            }

            return usbNotifyHandle;
        }

        protected override void WndProc(ref Message message)
        {
            switch (message.Msg)
            {
                case Win32.WM_INPUT:
                    {
                        var result = _keyboardDriver.ProcessRawInput(message.LParam, SuppressedDevices);
                        result = result || _mouseDriver.ProcessRawInput(message.LParam, SuppressedDevices);
                        if (!result)
                        {
                            base.WndProc(ref message);
                        }
                        return;
                    }

                case Win32.WM_USB_DEVICECHANGE:
                    {
                        Debug.WriteLine("USB Device Arrival / Removal");
                        _keyboardDriver.EnumerateDevices();
                        _mouseDriver.EnumerateDevices();
                        base.WndProc(ref message);
                        return;
                    }
            }
            base.WndProc(ref message);

        }

        public List<IntPtr> SuppressedDevices { get; set; } = new List<IntPtr>();

        ~RawInput()
        {
            Win32.UnregisterDeviceNotification(_devNotifyHandle);
            RemoveMessageFilters();
        }
    }
}
