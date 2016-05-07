using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using ODIF;
using RawInput_dll;
using InputDevice = ODIF.InputDevice;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace MouseInput
{
    public class MyMouseDevice : InputDevice
    {
        internal KeyValuePair<IntPtr, MouseEvent> Device;
        internal MouseInputDevice DeviceWrapper;

        public MyMouseDevice(RawInput rawInput, KeyValuePair<IntPtr, MouseEvent> device)
        {
            this.Device = device;
            this.DeviceWrapper = new MouseInputDevice();
            base.DeviceName = device.Value.Name ?? "Hid Mouse";
            AddChannels();
            RawInput = rawInput;
            //Win32.DeviceAudit();            // Writes a file DeviceAudit.txt to the current directory
            Subscribe();
        }

        private RawInput RawInput { get; set; }


        public void Subscribe()
        {
            //RawInput.KeyPressed += OnKeyPressed;
            RawInput.MouseEventRegistered += OnMouseEventRegistered;

            DeviceWrapper.SupressDeviceEvents.PropertyChanged += (s, e) =>
            {
                var supressThis = (bool)(s as DeviceChannel).Value;
                Debug.Write(supressThis ? "Device supressed" : "Device back to normal");
                if (supressThis)
                {
                    RawInput.SuppressedDevices.Add(Device.Key);
                }
                else
                {
                    RawInput.SuppressedDevices.Remove(Device.Key);
                }
            };
        }

        public void Unsubscribe()
        {

            //RawInput.KeyPressed -= OnKeyPressed;
            RawInput.MouseEventRegistered -= OnMouseEventRegistered;
        }

        private void OnMouseEventRegistered(object sender, RawInputEventArg e)
        {
            if (e.MouseEvent.DeviceHandle != Device.Key) return;
            if (e.MouseEvent.LeftDown) MouseDownHandler(sender, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
            if (e.MouseEvent.LeftUp) MouseUpHandler(sender, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
            if (e.MouseEvent.RightDown) MouseDownHandler(sender, new MouseEventArgs(MouseButtons.Right, 1, 0, 0, 0));
            if (e.MouseEvent.RightUp) MouseUpHandler(sender, new MouseEventArgs(MouseButtons.Right, 1, 0, 0, 0));
            if (e.MouseEvent.MiddleDown) MouseDownHandler(sender, new MouseEventArgs(MouseButtons.Middle, 1, 0, 0, 0));
            if (e.MouseEvent.MiddleUp) MouseUpHandler(sender, new MouseEventArgs(MouseButtons.Middle, 1, 0, 0, 0));
            if (e.MouseEvent.X1Down) MouseDownHandler(sender, new MouseEventArgs(MouseButtons.XButton1, 1, 0, 0, 0));
            if (e.MouseEvent.X1Up) MouseUpHandler(sender, new MouseEventArgs(MouseButtons.XButton1, 1, 0, 0, 0));
            if (e.MouseEvent.X2Down) MouseDownHandler(sender, new MouseEventArgs(MouseButtons.XButton2, 1, 0, 0, 0));
            if (e.MouseEvent.X2Up) MouseUpHandler(sender, new MouseEventArgs(MouseButtons.XButton2, 1, 0, 0, 0));
            if (e.MouseEvent.Wheel) MouseWheelHandler(sender, new MouseEventArgs(MouseButtons.None, e.MouseEvent.WheelValue, 0, 0, 0));
            if (e.MouseEvent.X2Up) MouseWheelHandler(sender, new MouseEventArgs(MouseButtons.None, e.MouseEvent.WheelValue, 0, 0, 0));
            MouseMoveHandler(sender, new MouseEventArgs(MouseButtons.None, 1, e.MouseEvent.X, e.MouseEvent.Y, 0));
        }

        private void MouseWheelHandler(object sender, MouseEventArgs e)
        {
            DeviceWrapper.WheelUp.Value = e.Clicks > 0;
            DeviceWrapper.WheelDown.Value = e.Clicks < 0;
        }

        private void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            var x = e.X * 1.0;
            var y = e.Y * 1.0;
            DeviceWrapper.XDelta.Value = x;
            DeviceWrapper.YDelta.Value = y;

            MaximumX = Math.Max(MaximumX, Math.Abs(x));
            MaximumY = Math.Max(MaximumY, Math.Abs(y));

            if (MaximumX == 0.0)
            {
                x = 0;
            }
            else {
                x = x / MaximumX;
            }
            if (MaximumY == 0)
            {
                y = 0.0;
            }
            else {
                y = y / MaximumY;
            }
            var factor = 1;
            DeviceWrapper.HorizontalAxis.Value = Math.Max(-1, Math.Min(1, x * factor));
            DeviceWrapper.VerticalAxis.Value = Math.Max(-1, Math.Min(1, y * factor));
        }

        public double MaximumY { get; set; }

        public double MaximumX { get; set; }

        private void MouseDownHandler(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    {
                        DeviceWrapper.LeftButton.Value = true;
                        break;
                    }
                case MouseButtons.Right:
                    {
                        DeviceWrapper.RightButton.Value = true;
                        break;
                    }
                case MouseButtons.Middle:
                    {
                        DeviceWrapper.MiddleMouse.Value = true;
                        break;
                    }
                case MouseButtons.XButton1:
                    {
                        DeviceWrapper.X1.Value = true;
                        break;
                    }
                case MouseButtons.XButton2:
                    {
                        DeviceWrapper.X2.Value = true;
                        break;
                    }
                default:
                    {
                        return;
                    }
            }
        }
        private void MouseUpHandler(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    {
                        DeviceWrapper.LeftButton.Value = false;
                        break;
                    }
                case MouseButtons.Right:
                    {
                        DeviceWrapper.RightButton.Value = false;
                        break;
                    }
                case MouseButtons.Middle:
                    {
                        DeviceWrapper.MiddleMouse.Value = false;
                        break;
                    }
                case MouseButtons.XButton1:
                    {
                        DeviceWrapper.X1.Value = false;
                        break;
                    }
                case MouseButtons.XButton2:
                    {
                        DeviceWrapper.X2.Value = false;
                        break;
                    }
                default:
                    {
                        return;
                    }
            }
        }


        private void AddChannels()
        {
            AddInputChannels();
            AddOutputChannels();

        }


        private void AddOutputChannels()
        {
            OutputChannels.Add(DeviceWrapper.SupressDeviceEvents);
        }

        private void AddInputChannels()
        {
            InputChannels.Add(DeviceWrapper.HorizontalAxis);
            InputChannels.Add(DeviceWrapper.VerticalAxis);
            //InputChannels.Add(DeviceWrapper.XPosition);
            //InputChannels.Add(DeviceWrapper.YPosition);

            InputChannels.Add(DeviceWrapper.XDelta);
            InputChannels.Add(DeviceWrapper.YDelta);

            InputChannels.Add(DeviceWrapper.LeftButton);
            InputChannels.Add(DeviceWrapper.RightButton);
            InputChannels.Add(DeviceWrapper.MiddleMouse);
            InputChannels.Add(DeviceWrapper.WheelUp);
            InputChannels.Add(DeviceWrapper.WheelDown);
            InputChannels.Add(DeviceWrapper.X1);
            InputChannels.Add(DeviceWrapper.X2);
        }


        protected override void Dispose(bool disposing)
        {
            Unsubscribe();
            base.Dispose(disposing);
        }

    }
}