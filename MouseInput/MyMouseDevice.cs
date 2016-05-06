using System;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using Gma.System.MouseKeyHook;
using InputDevice = ODIF.InputDevice;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace MouseInput
{
    public class MyMouseDevice : InputDevice
    {
        internal MouseInputDevice DeviceWrapper;


        private Thread InputPoolingThread { get; }

        private bool StopThread { get; set; }

        public MyMouseDevice()
        {
            this.DeviceWrapper = new MouseInputDevice();
            base.DeviceName = "Mice, touchpads and stylus";
            AddChannels();
            MGlobalHook = Hook.GlobalEvents();
            Subscribe();
        }

        private IKeyboardMouseEvents MGlobalHook { get; set; }


        public void Subscribe()
        {
            //MGlobalHook.KeyPress
            //MGlobalHook.KeyDown
            //MGlobalHook.KeyUp
            //MGlobalHook.KeyPress

            MGlobalHook.MouseDownExt += MouseDownHandler;
            MGlobalHook.MouseUp += MouseUpHandler;
            //MGlobalHook.MouseClick
            //MGlobalHook.MouseDoubleClick

            MGlobalHook.MouseMove += MouseMoveHandler;
            MGlobalHook.MouseWheel += MouseWheelHandler;
            //MGlobalHook.MouseDragStarted
            //MGlobalHook.MouseDragFinished

        }

        public void Unsubscribe()
        {//MGlobalHook.KeyPress
            //MGlobalHook.KeyDown
            //MGlobalHook.KeyUp
            //MGlobalHook.KeyPress

            MGlobalHook.MouseDown -= MouseDownHandler;
            MGlobalHook.MouseUp -= MouseUpHandler;
            //MGlobalHook.MouseClick
            //MGlobalHook.MouseDoubleClick

            MGlobalHook.MouseMove -= MouseMoveHandler;
            MGlobalHook.MouseWheel -= MouseWheelHandler;
            //MGlobalHook.MouseDragStarted
            //MGlobalHook.MouseDragFinished
            MGlobalHook.Dispose();
        }

        private void MouseWheelHandler(object sender, MouseEventArgs e)
        {
        }

        private void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            DeviceWrapper.XPosition.Value = e.X;
            DeviceWrapper.YPosition.Value = e.Y;
        }

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

        }


        private void AddInputChannels()
        {
            InputChannels.Add(DeviceWrapper.HorizontalAxis);
            InputChannels.Add(DeviceWrapper.VerticalAxis);
            InputChannels.Add(DeviceWrapper.XPosition);
            InputChannels.Add(DeviceWrapper.YPosition);

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